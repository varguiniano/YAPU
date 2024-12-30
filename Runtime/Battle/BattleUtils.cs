using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Core.Runtime.Logger;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Class with battle utilities.
    /// </summary>
    public static class BattleUtils
    {
        /// <summary>
        /// Change a list of battlers to a list of monster instances.
        /// </summary>
        /// <param name="battlers">List of battlers.</param>
        /// <returns>A list of monster instances.</returns>
        public static List<MonsterInstance> ToMonsterInstances(this IEnumerable<Battler> battlers) =>
            battlers.Select(battler => battler.ToMonsterInstance()).ToList();

        /// <summary>
        /// Get the number of not fainted monsters left in a roster.
        /// </summary>
        /// <param name="battlers">Roster to check.</param>
        /// <returns>The number of not fainted monsters.</returns>
        public static int NotFaintedLeft(this IEnumerable<Battler> battlers) =>
            battlers.Count(battler => battler.CanBattle);

        /// <summary>
        /// Calculates the priority of an action taken in battle.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="owner">Move owner.</param>
        /// <param name="action">Action that will take.</param>
        /// <returns>An int representing the priority.</returns>
        public static int CalculatePriority(YAPUSettings settings,
                                            BattleManager battleManager,
                                            Battler owner,
                                            BattleAction action)
        {
            int priority;

            switch (action.ActionType)
            {
                case BattleAction.Type.Move:

                    List<(BattlerType Type, int Index)> targetIndexes = new();

                    for (int i = action.Index < 5 ? 1 : 0; i < action.Parameters.Length; i += 2)
                        targetIndexes.Add(((BattlerType) action.Parameters[i], action.Parameters[i + 1]));

                    List<Battler> targets = targetIndexes
                                           .Select(pair => battleManager.Battlers.GetBattlerFromBattleIndex(pair))
                                           .ToList();

                    if (action.Parameters[0] == 4)
                        priority = settings.NoPPMove.GetPriority(owner, targets, battleManager);
                    else
                    {
                        Battler user =
                            battleManager.Battlers.GetBattlerFromBattleIndex(action.BattlerType, action.Index);

                        if (user.CurrentMoves.Length <= action.Parameters[0])
                            priority = 0;
                        else
                            priority = battleManager
                                      .Battlers.GetBattlerFromBattleIndex(action.BattlerType, action.Index)
                                      .CurrentMoves[action.Parameters[0]]
                                      .Move.GetPriority(owner, targets, battleManager);
                    }

                    break;

                case BattleAction.Type.Switch:
                case BattleAction.Type.Item:
                    priority = 6;
                    break;
                case BattleAction.Type.Run: priority = action.BattlerType == BattlerType.Ally ? 8 : 7; break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (battleManager.Scenario.Terrain != null)
                battleManager.Scenario.Terrain.ModifyActionPriority(owner, action, battleManager, ref priority);

            return priority;
        }

        /// <summary>
        /// Based on gen IV formula because of lack of more updated info: https://bulbapedia.bulbagarden.net/wiki/Escape
        /// Also, only uses the speed of the first monsters, even in double battles.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The chance of running away.</returns>
        public static float CalculateRunChance(BattleManager battleManager)
        {
            Battler ally = battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, 0);
            Battler enemy = battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 0);

            float allySpeed = MonsterMathHelper.CalculateStat(ally, Stat.Speed, battleManager);
            float enemySpeed = MonsterMathHelper.CalculateStat(enemy, Stat.Speed, battleManager);

            int multiplier = Mathf.FloorToInt(allySpeed * 128f / enemySpeed);

            return (int) (multiplier + 30 * battleManager.RunAwayAttempts) / 255f;
        }

        /// <summary>
        /// Calculate the probability of a ball to shake when catching a battler.
        /// Magic numbers come from: https://bulbapedia.bulbagarden.net/wiki/Catch_rate
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to catch.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <param name="ball">Ball to use.</param>
        /// <param name="callback">Callback stating the probability to shake.</param>
        public static IEnumerator CalculateShakeProbability(BattleManager battleManager,
                                                            Battler battler,
                                                            Battler ownBattler,
                                                            Ball ball,
                                                            Action<float> callback)
        {
            float normalizedProbability;
            float difficulty = battleManager.GlobalGameData.GetCatchDifficultyMultiplier();

            if (ball.NeverFails || difficulty < 0)
                normalizedProbability = 1;
            else
            {
                float catchRate = 0;

                yield return CalculateModifiedCatchRate(battleManager,
                                                        battler,
                                                        ownBattler,
                                                        ball,
                                                        rate => catchRate = Mathf.Max(rate, 1));

                StaticLogger.Info("The modified catch rate is " + catchRate + ".");

                float probability = Mathf.Floor(65536 / Mathf.Pow(255 / catchRate, 0.1875f));

                normalizedProbability = probability / 65535f;
            }

            StaticLogger.Info("The shake probability is " + normalizedProbability + ".");

            callback.Invoke(normalizedProbability * difficulty);
        }

        /// <summary>
        /// Calculate the modified catch rate for a battler and a ball.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <param name="ball">Ball to use.</param>
        /// <param name="callback">Callback stating the modified catch rate for that battler.</param>
        private static IEnumerator CalculateModifiedCatchRate(BattleManager battleManager,
                                                              Battler battler,
                                                              Battler ownBattler,
                                                              Ball ball,
                                                              Action<float> callback)
        {
            int maxHP = (int) MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager);
            int currentHP = (int) battler.CurrentHP;
            int rate = battler.GetCatchRateInBattle(battleManager);

            float catchMultiplier = 0;

            yield return ball.GetCatchMultiplier(battleManager,
                                                 battler,
                                                 ownBattler,
                                                 multiplier => catchMultiplier = multiplier);

            float catchAddition = ball.GetCatchAddition(battleManager, battler, ownBattler);

            float statusBonus = battler.GetStatus() != null ? battler.GetStatus().CatchMultiplier : 1f;

            float levelMultiplier = Mathf.Max((30 - battler.StatData.Level) * 0.1f, 1);

            float tripleMaxHP = maxHP * 3;

            float numeratorHP = tripleMaxHP - 2 * currentHP;
            float numerator = numeratorHP * rate * catchMultiplier + catchAddition;

            callback.Invoke(numerator / tripleMaxHP * statusBonus * levelMultiplier);
        }

        /// <summary>
        /// Calculate the catch probability of a monster out of battle.
        /// This is only useful for dex info.
        /// </summary>
        /// <param name="target">Reference to the battler.</param>
        /// <param name="ball">Ball to use.</param>
        /// <param name="globalGameData">Reference to the global game data.</param>
        /// <returns>The chance to catch it.</returns>
        public static float CalculateCatchProbabilityOutOfBattle(DataByFormEntry target,
                                                                 Ball ball,
                                                                 GlobalGameData globalGameData) =>
            Mathf.Pow(CalculateShakeProbabilityOutOfBattle(target, ball, globalGameData), 4);

        /// <summary>
        /// Calculate the probability of a ball to shake when catching a monster out of battle.
        /// Magic numbers come from: https://bulbapedia.bulbagarden.net/wiki/Catch_rate
        /// This is only useful for dex info.
        /// </summary>
        /// <param name="target">Reference to the battler.</param>
        /// <param name="ball">Ball to use.</param>
        /// <param name="globalGameData">Reference to the global game data.</param>
        /// <returns>The probability to shake.</returns>
        private static float CalculateShakeProbabilityOutOfBattle(DataByFormEntry target,
                                                                  Ball ball,
                                                                  GlobalGameData globalGameData)
        {
            float normalizedProbability;
            float difficulty = globalGameData.GetCatchDifficultyMultiplier();

            if (ball.NeverFails || difficulty < 0)
                normalizedProbability = 1;
            else
            {
                float catchRate = Mathf.Max(CalculateModifiedCatchRateOutOfBattle(target, ball), 1);

                StaticLogger.Info("The modified catch rate is " + catchRate + ".");

                float probability = Mathf.Floor(65536 / Mathf.Pow(255 / catchRate, 0.1875f));

                normalizedProbability = probability / 65535f;
            }

            StaticLogger.Info("The shake probability is " + normalizedProbability + ".");

            return normalizedProbability * difficulty;
        }

        /// <summary>
        /// Calculate the modified catch rate for a monster and a ball out of battle.
        /// This is only useful for dex info.
        /// </summary>
        /// <param name="target">Reference to the battler.</param>
        /// <param name="ball">Ball to use.</param>
        /// <returns>The modified catch rate for that battler.</returns>
        private static float CalculateModifiedCatchRateOutOfBattle(DataByFormEntry target,
                                                                   Ball ball)
        {
            // 1 makes the formula work without making it difficult to read.
            const int maxHP = 1;
            const int currentHP = 1;
            int rate = target.CatchRate;

            float catchMultiplier = ball.GetCatchMultiplierOutOfBattle(target);

            const float statusBonus = 1;

            float levelMultiplier = Mathf.Max((30 - 30) * 0.1f, 1);

            const float tripleMaxHP = maxHP * 3;

            const float numeratorHP = tripleMaxHP - 2 * currentHP;
            float numerator = numeratorHP * rate * catchMultiplier;

            return numerator / tripleMaxHP * statusBonus * levelMultiplier;
        }

        /// <summary>
        /// Try to calculate the action parameters for a move.
        /// Can fail for specific battle types and targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move that will be used.</param>
        /// <param name="type">Type of the user.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <param name="parameters">The generated parameters</param>
        /// <returns>True if the parameters could be calculated.</returns>
        public static bool TryAutoGenerateMoveTargets(BattleManager battleManager,
                                                      Move move,
                                                      BattlerType type,
                                                      int inBattleIndex,
                                                      out List<int> parameters)
        {
            parameters = new List<int>();

            if (!move.CanHaveMultipleTargets)
                switch (move.MovePossibleTargets)
                {
                    case Move.PossibleTargets.Self:
                    case Move.PossibleTargets.Allies:
                    case Move.PossibleTargets.AlliesAndSelf when battleManager.BattleType == BattleType.SingleBattle:
                    case Move.PossibleTargets.AdjacentAllies:
                    case Move.PossibleTargets.Adjacent when battleManager.BattleType == BattleType.SingleBattle:
                    case Move.PossibleTargets.Enemies when battleManager.BattleType == BattleType.SingleBattle:
                    case Move.PossibleTargets.AdjacentEnemies when battleManager.BattleType == BattleType.SingleBattle:
                    case Move.PossibleTargets.AllButSelf when battleManager.BattleType == BattleType.SingleBattle:
                    case Move.PossibleTargets.All when battleManager.BattleType == BattleType.SingleBattle:
                        parameters.AddRange(AutoGenerateParametersForSingleTargetMove(battleManager,
                                                move,
                                                type,
                                                inBattleIndex));

                        return true;

                    default: return false;
                }

            parameters.AddRange(GetTargetParametersForMultipleTargetsMove(battleManager,
                                                                          move,
                                                                          type,
                                                                          inBattleIndex));

            return true;
        }

        /// <summary>
        /// Calculate the action parameters for a single target move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move that will be used.</param>
        /// <param name="type">Type of the user.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <returns>The array of parameters.</returns>
        private static IEnumerable<int> AutoGenerateParametersForSingleTargetMove(BattleManager battleManager,
            Move move,
            BattlerType type,
            int inBattleIndex)
        {
            List<int> parameters = new();

            switch (battleManager.BattleType)
            {
                case BattleType.SingleBattle:
                    AutoGenerateParametersForSingleTargetMoveInSingleBattle(ref parameters,
                                                                            battleManager,
                                                                            move,
                                                                            type,
                                                                            inBattleIndex);

                    break;
                case BattleType.DoubleBattle:
                    AutoGenerateParametersForSingleTargetMoveInDoubleBattle(ref parameters,
                                                                            battleManager,
                                                                            move,
                                                                            type,
                                                                            inBattleIndex);

                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return parameters;
        }

        /// <summary>
        /// Calculate the action parameters for a single target move in single battle.
        /// </summary>
        /// <param name="parameters">Reference to the parameters list</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move that will be used.</param>
        /// <param name="type">Type of the user.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <returns>The array of parameters.</returns>
        private static void AutoGenerateParametersForSingleTargetMoveInSingleBattle(ref List<int> parameters,
            BattleManager battleManager,
            Move move,
            BattlerType type,
            int inBattleIndex)
        {
            switch (move.MovePossibleTargets)
            {
                case Move.PossibleTargets.Allies:
                case Move.PossibleTargets.AdjacentAllies:
                    // No allies in single battle, no targets :(
                    break;
                case Move.PossibleTargets.AlliesAndSelf:
                case Move.PossibleTargets.Self:
                    GenerateParametersForSelf(ref parameters, type, inBattleIndex);
                    break;
                case Move.PossibleTargets.Adjacent:
                case Move.PossibleTargets.AdjacentEnemies:
                case Move.PossibleTargets.Enemies:
                case Move.PossibleTargets.AllButSelf:
                    GenerateParametersForEnemies(ref parameters, battleManager, type);
                    break;
                case Move.PossibleTargets.All:
                    GenerateParametersForSelf(ref parameters, type, inBattleIndex);
                    GenerateParametersForEnemies(ref parameters, battleManager, type);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Calculate the action parameters for a single target move in double battle.
        /// Can only determine targets for self and allies.
        /// </summary>
        /// <param name="parameters">Reference to the parameters list</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move that will be used.</param>
        /// <param name="type">Type of the user.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <returns>The array of parameters.</returns>
        private static void AutoGenerateParametersForSingleTargetMoveInDoubleBattle(ref List<int> parameters,
            BattleManager battleManager,
            Move move,
            BattlerType type,
            int inBattleIndex)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (move.MovePossibleTargets)
            {
                case Move.PossibleTargets.Self: GenerateParametersForSelf(ref parameters, type, inBattleIndex); break;
                case Move.PossibleTargets.Allies:
                    GenerateParametersForAllies(ref parameters, battleManager, type, inBattleIndex);
                    break;
                default:
                    StaticLogger.Error("Can't autogenerate targets for double battle and move "
                                     + move.LocalizableName
                                     + ".");

                    break;
            }
        }

        /// <summary>
        /// Calculate the action parameters for a multiple targets move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move that will be used.</param>
        /// <param name="type">Type of the user.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <returns>The array of parameters.</returns>
        private static IEnumerable<int> GetTargetParametersForMultipleTargetsMove(BattleManager battleManager,
            Move move,
            BattlerType type,
            int inBattleIndex)
        {
            List<int> parameters = new();

            switch (move.MovePossibleTargets)
            {
                case Move.PossibleTargets.Self: GenerateParametersForSelf(ref parameters, type, inBattleIndex); break;
                case Move.PossibleTargets.AllButSelf:
                case Move.PossibleTargets.Adjacent:
                    GenerateParametersForAllies(ref parameters, battleManager, type, inBattleIndex);
                    GenerateParametersForEnemies(ref parameters, battleManager, type);
                    break;
                case Move.PossibleTargets.Allies:
                case Move.PossibleTargets.AdjacentAllies:
                    GenerateParametersForAllies(ref parameters, battleManager, type, inBattleIndex);
                    break;
                case Move.PossibleTargets.Enemies:
                case Move.PossibleTargets.AdjacentEnemies:
                    GenerateParametersForEnemies(ref parameters, battleManager, type);
                    break;
                case Move.PossibleTargets.AlliesAndSelf:
                    GenerateParametersForSelf(ref parameters, type, inBattleIndex);
                    GenerateParametersForAllies(ref parameters, battleManager, type, inBattleIndex);
                    break;
                case Move.PossibleTargets.All:
                    GenerateParametersForSelf(ref parameters, type, inBattleIndex);
                    GenerateParametersForAllies(ref parameters, battleManager, type, inBattleIndex);
                    GenerateParametersForEnemies(ref parameters, battleManager, type);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return parameters;
        }

        /// <summary>
        /// Generate the parameters for a move that targets the user.
        /// </summary>
        /// <param name="parameters">Reference to the parameters list.</param>
        /// <param name="type">User type.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        private static void GenerateParametersForSelf(ref List<int> parameters,
                                                      BattlerType type,
                                                      int inBattleIndex)
        {
            parameters.Add((int) type);
            parameters.Add(inBattleIndex);
        }

        /// <summary>
        /// Generate the parameters for a move that targets the allies.
        /// </summary>
        /// <param name="parameters">Reference to the parameters list.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">User type.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        private static void GenerateParametersForAllies(ref List<int> parameters,
                                                        BattleManager battleManager,
                                                        BattlerType type,
                                                        int inBattleIndex)
        {
            if (battleManager.BattleType == BattleType.SingleBattle) return;

            parameters.Add((int) type);
            parameters.Add(inBattleIndex == 0 ? 1 : 0);
        }

        /// <summary>
        /// Generate the parameters for a move that targets the enemies.
        /// </summary>
        /// <param name="parameters">Reference to the parameters list.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">User type.</param>
        private static void GenerateParametersForEnemies(ref List<int> parameters,
                                                         BattleManager battleManager,
                                                         BattlerType type)
        {
            parameters.Add(type == BattlerType.Ally ? 1 : 0);
            parameters.Add(0);

            if (battleManager.BattleType == BattleType.SingleBattle) return;

            parameters.Add(type == BattlerType.Ally ? 1 : 0);
            parameters.Add(1);
        }

        #region Loggable

        /// <summary>
        /// Get the static logger for this class.
        /// </summary>
        private static ILog StaticLogger => GetStaticLogger();

        /// <summary>
        /// Backfield for GetLogger.
        /// </summary>
        private static ILog staticLogger;

        /// <summary>
        /// Get the logger for this class.
        /// </summary>
        /// <returns></returns>
        private static ILog GetStaticLogger()
        {
            #if UNITY_EDITOR
            LogHandler.Initialize();
            #endif
            return staticLogger ??= LogManager.GetLogger(typeof(BattleManager));
        }

        #endregion
    }
}