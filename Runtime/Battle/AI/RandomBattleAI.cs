using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Stupid AI that does everything randomly.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/Random", fileName = "RandomBattleAI")]
    public class RandomBattleAI : BattleAI
    {
        /// <summary>
        /// Just a placeholder property to display the infobox.
        /// </summary>
        [InfoBox("This AI chooses a random action between performing a random move or switching to a random monster. Should only be used as an ultimate fallback.")]
        [SerializeField]
        [ReadOnly]
        // ReSharper disable once NotAccessedField.Local
        #pragma warning disable CS0414
        private string NoFallback = "No more fallbacks.";
        #pragma warning restore CS0414

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
            BattleAction action = new()
                                  {
                                      BattlerType = type,
                                      Index = inBattleIndex
                                  };

            bool chosen = false;

            List<BattleAction.Type> typeOptions = Utils.GetAllItems<BattleAction.Type>().ToList();

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            do
            {
                action.ActionType = battleManager.RandomProvider.RandomElement(typeOptions);

                switch (action.ActionType)
                {
                    case BattleAction.Type.Move:

                        List<MoveSlot> usableMoves = battler.GetUsableMoves(battleManager);

                        List<int> parameters;

                        if (usableMoves.Count == 0)
                        {
                            // Struggle time - 4, enemy, enemyIndex - 0.

                            (BattlerType targetType, int target) =
                                RandomTarget(battleManager, type, inBattleIndex, settings.NoPPMove);

                            parameters = new List<int>
                                         {
                                             4,
                                             (int) targetType,
                                             target
                                         };

                            action.Parameters = parameters.ToArray();
                            chosen = true;
                            break;
                        }

                        MoveSlot randomMove = battleManager.RandomProvider.RandomElement(usableMoves);

                        parameters = new List<int> {battler.CurrentMoves.IndexOf(randomMove)};

                        if (BattleUtils.TryAutoGenerateMoveTargets(battleManager,
                                                                   randomMove.Move,
                                                                   type,
                                                                   inBattleIndex,
                                                                   out List<int> autoParameters))
                            parameters.AddRange(autoParameters);
                        else
                        {
                            (BattlerType targetType, int target) =
                                RandomTarget(battleManager, type, inBattleIndex, randomMove.Move);

                            parameters.Add((int) targetType);
                            parameters.Add(target);
                        }

                        action.Parameters = parameters.ToArray();

                        chosen = true;

                        break;
                    case BattleAction.Type.Switch:

                        if (!battler.CanSwitch(battleManager, type, inBattleIndex, null, false, null, false, false))
                            break;

                        (int rosterIndex, int _) =
                            battleManager.Rosters.InBattleIndexToRosterIndex(type, inBattleIndex);

                        List<Battler> notFightingAndNotFainted =
                            battleManager.Battlers.GetBattlersNotFighting(type, rosterIndex)
                                         .Where(candidate => candidate.CanBattle)
                                         .ToList();

                        if (notFightingAndNotFainted.Count == 0) break;

                        Battler newBattler = battleManager.RandomProvider.RandomElement(notFightingAndNotFainted);

                        action.Parameters = new[]
                                            {
                                                battleManager.Rosters.GetRoster(type, inBattleIndex)
                                                             .IndexOf(newBattler)
                                            };

                        chosen = true;

                        break;
                    case BattleAction.Type.Item:

                        // No items in random.
                        break;
                    case BattleAction.Type.Run:
                        // Run has already been calculated.
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            while (!chosen);

            callback.Invoke(action);
            yield break;
        }

        /// <summary>
        /// Request the AI to send a new monster after a monster has fainted.
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
                              List<Battler> forbiddenBattlers)
        {
            List<Battler> nonFighting = battleManager.Battlers.GetBattlersNotFighting(type, inBattleIndex)
                                                     .Where(battler => battler.CanBattle
                                                                    && !forbiddenBattlers.Contains(battler))
                                                     .ToList();

            if (nonFighting.Count == 0)
            {
                Logger.Warn("There are no battlers available in roster " + type + " " + inBattleIndex + ".");
                return -1;
            }

            List<Battler> checkedCandidates = new();

            Battler candidate;

            do
            {
                candidate = battleManager.RandomProvider.RandomElement(nonFighting);

                if (!checkedCandidates.Contains(candidate)) checkedCandidates.Add(candidate);

                if ((candidate.CanBattle && !forbiddenBattlers.Contains(candidate))
                 || checkedCandidates.Count != nonFighting.Count)
                    continue;

                Logger.Warn("There are no battlers available in roster " + type + " " + inBattleIndex + ".");
                return -1;
            }
            while (!candidate.CanBattle || forbiddenBattlers.Contains(candidate));

            return battleManager.Rosters.GetRoster(type, inBattleIndex).IndexOf(candidate);
        }

        /// <summary>
        /// Get a random target for a move when it has a single target..
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the move.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <param name="move">Move to use.</param>
        private static (BattlerType, int) RandomTarget(BattleManager battleManager,
                                                       BattlerType userType,
                                                       int inBattleIndex,
                                                       Move move)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (move.MovePossibleTargets)
            {
                case Move.PossibleTargets.AllButSelf:
                case Move.PossibleTargets.Adjacent:
                    if (move is DamageMove || battleManager.BattleType == BattleType.SingleBattle)
                        return
                            (userType == BattlerType.Ally ? BattlerType.Enemy : BattlerType.Ally,
                             battleManager.RandomProvider.Range(0, battleManager.Battlers.GetNumberOfBattlers()));

                    return (userType, inBattleIndex == 0 ? 1 : 0);
                case Move.PossibleTargets.Enemies:
                    return
                        (userType == BattlerType.Ally ? BattlerType.Enemy : BattlerType.Ally,
                         battleManager.RandomProvider.Range(0, battleManager.Battlers.GetNumberOfBattlers()));
                case Move.PossibleTargets.All:
                    return (battleManager.RandomProvider.RandomElement(Utils.GetAllItems<BattlerType>().ToList()),
                            battleManager.RandomProvider.Range(0, battleManager.Battlers.GetNumberOfBattlers()));
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}