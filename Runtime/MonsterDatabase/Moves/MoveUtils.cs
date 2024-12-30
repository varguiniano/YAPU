using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class with utilities related to moves.
    /// </summary>
    public static class MoveUtils
    {
        /// <summary>
        /// Retrieve the localizable string of a move effectiveness.
        /// </summary>
        /// <param name="effectiveness">The effectiveness of the move.</param>
        /// <returns>A localizable key to feed the localizer.</returns>
        public static string EffectivenessToLocalizableKey(float effectiveness) =>
            effectiveness switch
            {
                <= 0 => "Moves/Effectiveness/None",
                < 1 => "Moves/Effectiveness/NotVery",
                1 => "Moves/Effectiveness/Normal",
                > 1 => "Moves/Effectiveness/Very",
                _ => throw new ArgumentOutOfRangeException(nameof(effectiveness), effectiveness, null)
            };

        /// <summary>
        /// Generate a list of valid targets for a move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battlerType">Type of the battler.</param>
        /// <param name="index">Index of the battler.</param>
        /// <param name="move">Move to check.</param>
        /// <param name="logger">Logger object to use.</param>
        /// <returns>A list of the valid targets.</returns>
        public static List<Battler> GenerateValidTargetsForMove(BattleManager battleManager,
                                                                BattlerType battlerType,
                                                                int index,
                                                                Move move,
                                                                ILog logger)
        {
            logger.Info("Couldn't auto generate target for move "
                      + move.LocalizableName
                      + ". Calculating possible targets.");

            Battler[] targets = battlerType switch
            {
                BattlerType.Ally => new[]
                                    {
                                        battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy,
                                            0),
                                        battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy,
                                            1),
                                        battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally,
                                            0),
                                        battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally,
                                            1)
                                    },
                BattlerType.Enemy => new[]
                                     {
                                         battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally,
                                             0),
                                         battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally,
                                             1),
                                         battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy,
                                             0),
                                         battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy,
                                             1)
                                     },
                _ => throw new BattleManager.UnsupportedBattlerException(battlerType)
            };

            bool[] valid =
                GenerateValidTargetsArray(battleManager.BattleType,
                                          move,
                                          targets,
                                          index);

            int validCount = valid.Count(isValid => isValid);

            switch (validCount)
            {
                case 0:
                    logger.Warn("No possible valid targets for "
                              + move.LocalizableName
                              + ". We will assign null.");

                    return null;
                default:

                    List<Battler> possibleTargets = new();

                    for (int i = 0; i < valid.Length; i++)
                        if (valid[i])
                            possibleTargets.Add(targets[i]);

                    return possibleTargets;
            }
        }

        /// <summary>
        /// Generate an array of valid targets for a move.
        /// </summary>
        /// <param name="battleType">Type of battle.</param>
        /// <param name="move">Move to check.</param>
        /// <param name="battlers">Array of the battlers.</param>
        /// <param name="inBattleIndex">In battle index of the user.</param>
        /// <returns>An array of booleans.</returns>
        public static bool[]
            GenerateValidTargetsArray(BattleType battleType, Move move, Battler[] battlers, int inBattleIndex) =>
            new[]
            {
                battlers[0]?.CanBeTargeted() == true
             && move.MovePossibleTargets is Move.PossibleTargets.Adjacent or Move.PossibleTargets.All or Move
                   .PossibleTargets.Enemies or Move.PossibleTargets.AdjacentEnemies or Move.PossibleTargets.AllButSelf,
                battlers[1]?.CanBeTargeted() == true
             && battleType == BattleType.DoubleBattle
             && move.MovePossibleTargets is Move.PossibleTargets.Adjacent or Move.PossibleTargets.All or Move
                   .PossibleTargets.Enemies or Move.PossibleTargets.AdjacentEnemies or Move.PossibleTargets.AllButSelf,
                battlers[2]?.CanBeTargeted() == true
             && (inBattleIndex == 0
                     ? GenerateOwnValidTarget(move)
                     : GenerateAllyValidTarget(battleType, move)),
                battlers[3]?.CanBeTargeted() == true
             && (inBattleIndex == 1
                     ? GenerateOwnValidTarget(move)
                     : GenerateAllyValidTarget(battleType, move))
            };

        /// <summary>
        /// Try generate random targets for moves that need them on the fly, like Sleep Talk or Metronome.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move to generate from.</param>
        /// <param name="userType">User of the move.</param>
        /// <param name="userIndex">User of the move.</param>
        /// <param name="preferredTarget">Is there a preferred target?</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="callback">Callback with the generated targets.</param>
        public static IEnumerator TryGenerateRandomTargetsForMove(BattleManager battleManager,
                                                                  Move move,
                                                                  BattlerType userType,
                                                                  int userIndex,
                                                                  Battler preferredTarget,
                                                                  ILog logger,
                                                                  Action<List<(BattlerType Type, int Index)>> callback)
        {
            List<(BattlerType Type, int Index)> targets = new();

            if (BattleUtils.TryAutoGenerateMoveTargets(battleManager,
                                                       move,
                                                       userType,
                                                       userIndex,
                                                       out List<int> newTargets))
                for (int i = 0; i < newTargets.Count; i += 2)
                    targets.Add(((BattlerType) newTargets[i], newTargets[i + 1]));
            else
            {
                List<Battler> validTargets =
                    GenerateValidTargetsForMove(battleManager, userType, userIndex, move, logger);

                if (validTargets == null)
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    callback.Invoke(null);

                    yield break;
                }

                targets.Add(battleManager.Battlers.GetTypeAndIndexOfBattler(validTargets.Contains(preferredTarget)
                                                                                ? preferredTarget
                                                                                : battleManager.RandomProvider
                                                                                   .RandomElement(validTargets)));
            }

            callback.Invoke(targets);
        }

        /// <summary>
        /// Generate if the target is valid for its own.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if valid for its own.</returns>
        private static bool GenerateOwnValidTarget(Move move) =>
            move.MovePossibleTargets is Move.PossibleTargets.Self or Move.PossibleTargets.All
                or Move.PossibleTargets.AlliesAndSelf;

        /// <summary>
        /// Generate if the target is valid for its ally.
        /// </summary>
        /// <param name="battleType">Type of battle.</param>
        /// <param name="move">Move to check.</param>
        /// <returns>True if valid for its ally.</returns>
        private static bool GenerateAllyValidTarget(BattleType battleType, Move move) =>
            battleType == BattleType.DoubleBattle
         && move.MovePossibleTargets is Move.PossibleTargets.Adjacent or Move.PossibleTargets.All or
                Move.PossibleTargets.Allies or Move.PossibleTargets.AdjacentAllies or Move.PossibleTargets.AllButSelf
                or Move.PossibleTargets.AlliesAndSelf;

        /// <summary>
        /// Enable and play a visual effect.
        /// Useful for move animations.
        /// </summary>
        /// <param name="effect">Effect to play.</param>
        internal static void EnableAndPlay(this VisualEffect effect)
        {
            effect.enabled = true;
            effect.Play();
        }
    }
}