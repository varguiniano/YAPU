using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that uses an ML Agent to make decisions.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/MLBattleAI", fileName = "MLBattleAI")]
    public class MLBattleAI : BattleAI
    {
        /// <summary>
        /// Prefab to use when spawning agents.
        /// </summary>
        [InfoBox("This AI uses an ML Agent to make decisions.")]
        [SerializeField]
        private YAPUAgent AgentPrefab;

        /// <summary>
        /// Fallback to go to.
        /// </summary>
        [SerializeField]
        private BattleAI Fallback;

        /// <summary>
        /// Dictionary that keep track of the spawned agents.
        /// </summary>
        private readonly Dictionary<(BattlerType Type, int Index), YAPUAgent> agents = new();

        /// <summary>
        /// Request to choose to perform an action.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="callback">Callback stating the action to take along with its parameters.</param>
        /// <returns>The action taken along with its parameters.</returns>
        public override IEnumerator RequestPerformAction(YAPUSettings settings,
                                                         BattleManager battleManager,
                                                         BattlerType type,
                                                         int inBattleIndex,
                                                         Action<BattleAction> callback)
        {
            YAPUAgent agent;

            if (agents.ContainsKey((type, inBattleIndex)))
                agent = agents[(type, inBattleIndex)];
            else
            {
                agent = Instantiate(AgentPrefab);
                yield return WaitAFrame;
                DontDestroyOnLoad(agent.gameObject);

                agents[(type, inBattleIndex)] = agent;

                // Disable collecting observations each fixed frame.
                Academy.Instance.AutomaticSteppingEnabled = false;
            }

            CollectObservations(battleManager, type, inBattleIndex, agent);

            // Manually trigger the step for the brain to collect the observations.
            Academy.Instance.EnvironmentStep();

            yield return WaitAFrame;

            agent.RequestDecisionAsync();

            // Manually trigger the step so it makes a decision.
            Academy.Instance.EnvironmentStep();

            yield return new WaitUntil(() => agent.DecisionTaken);

            yield return BuildActionFromDecisions(type, inBattleIndex, agent.Decisions, battleManager, callback);
        }

        /// <summary>
        /// Request the AI to send a new monster after a monster has fainted.
        /// This doesn't actually use Machine Learning, just the fallback.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index of the monster that just fainted..</param>
        /// <param name="forbiddenBattlers"></param>
        /// <returns>The index of the monster to send from the AI's roster.</returns>
        public override int
            RequestNewMonster(YAPUSettings settings,
                              BattleManager battleManager,
                              BattlerType type,
                              int inBattleIndex,
                              List<Battler> forbiddenBattlers) =>
            Fallback.RequestNewMonster(settings, battleManager, type, inBattleIndex, forbiddenBattlers);

        /// <summary>
        /// Add a reward when an opponent faints, subtract when an ally faints.
        /// </summary>
        public override void OnBattlerFainted(BattleManager battleManager,
                                              BattlerType ownType,
                                              int ownIndex,
                                              BattlerType faintedType,
                                              int faintedIndex)
        {
            float reward;

            List<List<Battler>> allyRosters = ownType == BattlerType.Ally
                                                  ? battleManager.Rosters.AllyRosters
                                                  : battleManager.Rosters.EnemyRosters;

            List<List<Battler>> opponentRosters = ownType != BattlerType.Ally
                                                      ? battleManager.Rosters.AllyRosters
                                                      : battleManager.Rosters.EnemyRosters;

            if (ownType == faintedType)
                reward = -10f
                       / allyRosters.Sum(roster => roster.Count
                                                 + roster.Sum(battler => battler.TimesRevivedThisBattle));
            else
                reward = 100f
                       / opponentRosters.Sum(roster => roster.Count
                                                     + roster.Sum(battler => battler.TimesRevivedThisBattle));

            agents[(ownType, ownIndex)].AddReward(reward);
        }

        /// <summary>
        /// Called when the battle ends.
        /// </summary>
        public override void OnBattleEnded(BattlerType ownType,
                                           int ownIndex,
                                           BattlerType winners,
                                           BattleManager battleManager) =>
            agents[(ownType, ownIndex)].EndEpisode();

        /// <summary>
        /// Collect all the observations and save them into the agent.
        /// A total of 630 observations.
        /// </summary>
        private static void CollectObservations(BattleManager battleManager,
                                                BattlerType type,
                                                int inBattleIndex,
                                                YAPUAgent agent)
        {
            List<int> observations = new()
                                     {
                                         (int) battleManager.BattleType,
                                         (int) battleManager.EnemyType,
                                         (int) type,
                                         inBattleIndex
                                     };

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            (BattlerType Type, int RosterIndex, int BattlerIndex) rosterData =
                battleManager.Battlers.GetTypeAndRosterIndexOfBattler(battler);

            List<Battler> roster = battleManager.Rosters.GetRoster(type, rosterData.RosterIndex);

            CollectObservationsForScenario(battleManager, ref observations);
            CollectObservationsForSideStatuses(battleManager, BattlerType.Ally, ref observations);
            CollectObservationsForSideStatuses(battleManager, BattlerType.Enemy, ref observations);

            CollectObservationsForBattler(battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, 0),
                                          ref observations);

            CollectObservationsForBattler(battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, 1),
                                          ref observations);

            CollectObservationsForBattler(battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 0),
                                          ref observations);

            CollectObservationsForBattler(battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 1),
                                          ref observations);

            // Observations for each of its team members.
            foreach (Battler rosterMember in roster.Where(rosterMember => rosterMember != battler))
                CollectObservationsForBattler(rosterMember, ref observations);

            // Missing members to sum 6.
            for (int i = roster.Count; i <= 6; i++)
                for (int j = 0; j < 63; ++j)
                    observations.Add(0);

            agent.Observations = observations;
        }

        /// <summary>
        /// Collect all the observations of the scenario and save them to the agent.
        /// 23 observations in total.
        /// </summary>
        private static void CollectObservationsForScenario(BattleManager battleManager,
                                                           ref List<int> observations)
        {
            observations.Add(battleManager.Scenario.Terrain != null
                                 ? battleManager.Scenario.Terrain.name.GetHashCode()
                                 : 0);

            observations.Add(battleManager.Scenario.GetWeather(out Weather weather) ? weather.name.GetHashCode() : 0);

            observations.Add((int) battleManager.Scenario.EncounterType);

            // We only observe the first 10 global statuses.
            int statusObserved = 0;

            foreach (KeyValuePair<GlobalStatus, int> pair in battleManager.Statuses.GetGlobalStatuses())
            {
                observations.Add(pair.Key.name.GetHashCode());
                observations.Add(pair.Value);
                statusObserved++;
            }

            while (statusObserved < 10)
            {
                observations.Add(0);
                observations.Add(0);
                statusObserved++;
            }
        }

        /// <summary>
        /// Collect the observations for the status of a side.
        /// 20 observations in total.
        /// </summary>
        private static void CollectObservationsForSideStatuses(BattleManager battleManager,
                                                               BattlerType battlerType,
                                                               ref List<int> observations)
        {
            // We only observe the first 10 statuses.
            int statusObserved = 0;

            foreach (KeyValuePair<SideStatus, int> pair in battleManager.Statuses.GetSideStatuses(battlerType))
            {
                observations.Add(pair.Key.name.GetHashCode());
                observations.Add(pair.Value);
                statusObserved++;
            }

            while (statusObserved < 10)
            {
                observations.Add(0);
                observations.Add(0);
                statusObserved++;
            }
        }

        /// <summary>
        /// Collect all the observations of a battler and save them to the agent.
        /// 63 observations.
        /// </summary>
        private static void CollectObservationsForBattler(Battler battler, ref List<int> observations)
        {
            if (battler == null)
            {
                for (int i = 0; i < 63; ++i) observations.Add(0);

                return;
            }

            // 5 Observations.
            observations.Add(battler.Species.name.GetHashCode());
            observations.Add(battler.Form.name.GetHashCode());
            observations.Add((int) battler.PhysicalData.Gender);
            observations.Add(battler.StatData.Level);
            observations.Add(battler.StatData.Nature.GetHashCode());

            // 6 Observations.
            foreach (byte value in battler.StatData.IndividualValues.Values) observations.Add(value);

            // 6 Observations.
            foreach (byte value in battler.StatData.EffortValues.Values) observations.Add(value);

            // 4 Observations.
            observations.Add(battler.GetAbility().name.GetHashCode());
            observations.Add(battler.Friendship);
            observations.Add((int) battler.CurrentHP);
            observations.Add(battler.GetStatus() != null ? battler.GetStatus().name.GetHashCode() : 0);

            // 8 Observations.
            foreach (MoveSlot moveSlot in battler.CurrentMoves)
            {
                observations.Add(moveSlot.Move != null ? moveSlot.Move.name.GetHashCode() : 0);
                observations.Add(moveSlot.CurrentPP);
            }

            // 2 Observations.
            observations.Add(battler.HeldItem != null ? battler.HeldItem.name.GetHashCode() : 0);
            observations.Add(battler.CanBattle ? 1 : 0);

            // 6 Observations.
            foreach (short value in battler.StatStage.Values) observations.Add(value);

            // 3 Observations.
            foreach (short value in battler.BattleStatStage.Values) observations.Add(value);

            // 3 Observations.
            observations.Add(battler.CriticalStage);
            observations.Add(battler.Substitute.SubstituteEnabled ? 1 : 0);
            observations.Add((int) battler.Substitute.CurrentHP);

            // 20 observations.
            // We only observe the first 10 statuses.
            int statusObserved = 0;

            foreach (KeyValuePair<VolatileStatus, int> pair in battler.VolatileStatuses)
            {
                observations.Add(pair.Key.name.GetHashCode());
                observations.Add(pair.Value);
                statusObserved++;
            }

            while (statusObserved < 10)
            {
                observations.Add(0);
                observations.Add(0);
                statusObserved++;
            }
        }

        /// <summary>
        /// Build a battle action based on the decisions made by the agent.
        /// This method needs to do a lot of sanitizing.
        /// This uses a total of 12 decisions.
        /// </summary>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="decisions">List of the decisions taken.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating the battle action to perform.</param>
        private IEnumerator BuildActionFromDecisions(BattlerType type,
                                                     int inBattleIndex,
                                                     List<int> decisions,
                                                     BattleManager battleManager,
                                                     Action<BattleAction> callback)
        {
            BattleAction action = new()
                                  {
                                      BattlerType = type,
                                      Index = inBattleIndex,
                                      ActionType = (BattleAction.Type) decisions[0],
                                      TriggerMegaForm = decisions[1] != 0
                                  };

            Logger.Info("Received ML Agent decision.");

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            switch (action.ActionType)
            {
                case BattleAction.Type.Move:
                    callback(BuildMoveActionFromDecisions(type,
                                                          inBattleIndex,
                                                          battler,
                                                          decisions,
                                                          battleManager,
                                                          action));

                    yield break;

                case BattleAction.Type.Switch:
                    yield return BuildSwitchActionFromDecisions(type,
                                                                inBattleIndex,
                                                                battler,
                                                                decisions,
                                                                battleManager,
                                                                action,
                                                                callback);

                    yield break;
                case BattleAction.Type.Item:
                    yield return BuildItemActionFromDecisions(type,
                                                              inBattleIndex,
                                                              battler,
                                                              decisions,
                                                              battleManager,
                                                              action,
                                                              callback);

                    yield break;
                case BattleAction.Type.Run: // The ML Agent cannot choose running away.
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Build a battle action for using a move based on the decisions made by the agent.
        /// This method needs to do a lot of sanitizing.
        /// This uses a total of 12 decisions.
        /// </summary>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="decisions">List of the decisions taken.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="action">Action object to save the information in.</param>
        /// <returns>The battle action to perform.</returns>
        private BattleAction BuildMoveActionFromDecisions(BattlerType type,
                                                          int inBattleIndex,
                                                          Battler battler,
                                                          List<int> decisions,
                                                          BattleManager battleManager,
                                                          BattleAction action)
        {
            Logger.Info("ML Agent decision is use a move.");

            List<int> parameters = new();

            int moveIndex = Mathf.Clamp(decisions[2], 0, 3);

            List<int> checkedMoves = new();

            while (moveIndex != 4
                && (battler.CurrentMoves[moveIndex].Move == null || battler.CurrentMoves[moveIndex].CurrentPP == 0))
            {
                checkedMoves.AddIfNew(moveIndex);

                do
                {
                    Logger.Warn("ML Agent chose " + moveIndex + " as move index, which is not valid, trying another.");
                    moveIndex = checkedMoves.Count >= 4 ? 4 : battleManager.RandomProvider.Range(0, 4);
                }
                while (checkedMoves.Contains(moveIndex) && moveIndex != 4);
            }

            Move move = moveIndex == 4 ? battleManager.YAPUSettings.NoPPMove : battler.CurrentMoves[moveIndex].Move;

            parameters.Add(moveIndex);

            // TODO: Triple battle.
            int maxTargetIndex = battleManager.BattleType == BattleType.SingleBattle ? 0 : 1;

            List<Battler> validTargets =
                MoveUtils.GenerateValidTargetsForMove(battleManager, type, inBattleIndex, move, Logger);

            List<(BattlerType, int)> targets = new();

            if (validTargets != null)
                for (int i = 3; i < 11; i += 2)
                {
                    BattlerType targetType = (BattlerType) Mathf.Clamp(decisions[i], 0, 1);
                    int targetIndex = Mathf.Clamp(decisions[i + 1], 0, maxTargetIndex);
                    Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                    if (validTargets.Contains(target) && !targets.Contains((targetType, targetIndex)))
                        targets.Add((targetType, targetIndex));
                }

            if (targets.Count == 0)
            {
                Logger.Warn("ML Agent failed to determine valid targets for move "
                          + move.name
                          + ", selecting targets heuristically.");

                return GenerateActionForMove(battleManager, type, inBattleIndex, battler, move);
            }

            foreach ((BattlerType Type, int Index) target in targets)
            {
                parameters.Add((int) target.Type);
                parameters.Add(target.Index);
            }

            action.Parameters = parameters.ToArray();

            return action;
        }

        /// <summary>
        /// Build a battle action for switching to another mon based on the decisions made by the agent.
        /// This doesn't need sanitizing since the State Machine already does it (hopefully).
        /// This uses a total of 12 decisions.
        /// </summary>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="decisions">List of the decisions taken.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="action">Action object to save the information in.</param>
        /// <param name="callback">Callback stating the action to perform.</param>
        private IEnumerator BuildSwitchActionFromDecisions(BattlerType type,
                                                           int inBattleIndex,
                                                           Battler battler,
                                                           List<int> decisions,
                                                           BattleManager battleManager,
                                                           BattleAction action,
                                                           Action<BattleAction> callback)
        {
            Logger.Info("ML Agent decision is switch.");

            List<Battler> nonFighting = battleManager.Battlers.GetBattlersNotFighting(type, inBattleIndex)
                                                     .Where(candidate => candidate.CanBattle)
                                                     .ToList();

            List<Battler> roster = battleManager.Rosters.GetRoster(type, inBattleIndex);

            if (nonFighting.Count == 0)
            {
                Logger.Warn("There are no battlers available in roster "
                          + type
                          + " "
                          + inBattleIndex
                          + ". ML failed when choosing switching, falling back to heuristic.");

                yield return Fallback.RequestPerformAction(battleManager.YAPUSettings,
                                                           battleManager,
                                                           type,
                                                           inBattleIndex,
                                                           callback);

                yield break;
            }

            int battlerIndex = Mathf.Clamp(decisions[3], 0, roster.Count - 1);
            List<int> checkedBattlers = new();

            while (!nonFighting.Contains(roster[battlerIndex]))
            {
                checkedBattlers.Add(battlerIndex);

                if (checkedBattlers.Count == 0 || checkedBattlers.Count >= nonFighting.Count)
                {
                    Logger.Warn("There are no battlers available in roster "
                              + type
                              + " "
                              + inBattleIndex
                              + ". ML failed when choosing switching, falling back to heuristic.");

                    yield return Fallback.RequestPerformAction(battleManager.YAPUSettings,
                                                               battleManager,
                                                               type,
                                                               inBattleIndex,
                                                               callback);

                    yield break;
                }

                battlerIndex++;

                if (battlerIndex >= roster.Count) battlerIndex = 0;
            }

            action.Parameters = new[] {battlerIndex};
            callback(action);
        }

        /// <summary>
        /// Build a battle action for using an item based on the decisions made by the agent.
        /// This method needs to do a lot of sanitizing.
        /// This uses a total of 12 decisions.
        /// </summary>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="decisions">List of the decisions taken.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="action">Action object to save the information in.</param>
        /// <param name="callback">Callback stating the action to perform.</param>
        private IEnumerator BuildItemActionFromDecisions(BattlerType type,
                                                         int inBattleIndex,
                                                         Battler battler,
                                                         List<int> decisions,
                                                         BattleManager battleManager,
                                                         BattleAction action,
                                                         Action<BattleAction> callback)
        {
            Logger.Info("ML Agent decision is use an item.");

            action.Parameters = new int[6];

            Bag bag = battleManager.Items.GetBag(type, inBattleIndex);

            int itemIndex = Mathf.Clamp(decisions[11], 0, bag.AvailableDistinctItems - 1);
            Item item = bag.GetItemFromIndex(itemIndex);

            action.Parameters[1] = itemIndex;

            (BattlerType _, int rosterIndex, int _) =
                battleManager.Battlers.GetTypeAndRosterIndexOfBattler(battler);

            List<Battler> roster = battleManager.Rosters.GetRoster(type, rosterIndex);

            int inRosterIndex = Mathf.Clamp(decisions[3], 0, roster.Count - 1);

            if (item != null && item.CanBeUsedInBattle)
                callback(action);
            else if (item != null && item.CanBeUsedInBattleOnTarget)
            {
                action.Parameters[2] = (int) type;
                action.Parameters[3] = rosterIndex;
                action.Parameters[4] = inRosterIndex;
                callback(action);
            }
            else if (item != null && item.CanBeUsedInBattleOnTargetMove)
            {
                action.Parameters[2] = (int) type;
                action.Parameters[3] = rosterIndex;
                action.Parameters[4] = inRosterIndex;

                int moveIndex = Mathf.Clamp(decisions[4],
                                            0,
                                            battleManager.Battlers
                                                         .GetBattlerFromRosterAndIndex(type, rosterIndex, inRosterIndex)
                                                         .CurrentMoves.Count(slot => slot.Move != null)
                                          - 1);

                action.Parameters[5] = moveIndex;
                callback(action);
            }
            else
            {
                Logger.Warn("Item "
                          + (item != null ? item.name : "null")
                          + " chosen by the ML Agent can't be used in battle. Falling back to heuristic AI for another action.");

                yield return Fallback.RequestPerformAction(battleManager.YAPUSettings,
                                                           battleManager,
                                                           type,
                                                           inBattleIndex,
                                                           callback);
            }
        }
    }
}