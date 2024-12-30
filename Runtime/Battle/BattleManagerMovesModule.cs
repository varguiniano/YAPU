using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Battle manager module for moves.
    /// </summary>
    public class BattleManagerMovesModule : BattleManagerModule<BattleManagerMovesModule>
    {
        /// <summary>
        /// Last move that was performed successfully.
        /// </summary>
        [ReadOnly]
        public Move LastSuccessfullyPerformedMove;

        /// <summary>
        /// Tell the given battler to perform a move.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Roster index.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="moveIndex">Index of the move.</param>
        [Button("Move")]
        [FoldoutGroup("Debug")]
        [HideInEditorMode]
        private void
            TestPerformMove(BattlerType type,
                            int index,
                            List<(BattlerType Type, int Index)> targets,
                            int moveIndex) =>
            StartCoroutine(PerformMove(type, index, targets, moveIndex));

        /// <summary>
        /// Tell the given battler to perform a move.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">In battle index.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="moveIndex">Index of the move.</param>
        public IEnumerator PerformMove(BattlerType type,
                                       int index,
                                       List<(BattlerType Type, int Index)> targets,
                                       int moveIndex)
        {
            if (moveIndex is < 0 or > 4)
            {
                Logger.Error("Move index must be between 0 and 4! 0-3 are its moves and 4 is Struggle.");
                yield break;
            }

            Battler battler = Battlers.GetBattlerFromBattleIndex(type, index);

            if (moveIndex < 4)
            {
                MoveSlot slot = battler.CurrentMoves[moveIndex];

                if (slot.Move == null)
                {
                    Logger.Error("This battler doesn't know a move on that slot.");
                    yield break;
                }

                if (slot.CurrentPP <= 0)
                {
                    Logger.Error(slot.Move.LocalizableName
                               + " doesn't have PP left. If you want to force it call the method passing the move directly.");

                    yield break;
                }

                // Sanitize the targets to make sure we don't receive more targets than available.
                List<Battler> potentialTargets =
                    MoveUtils.GenerateValidTargetsForMove(BattleManager, type, index, slot.Move, Logger);

                List<(BattlerType Type, int Index)> sanitizedTargets =
                    potentialTargets == null || potentialTargets.IsEmpty()
                        ? new List<(BattlerType Type, int Index)>()
                        : targets
                         .Where(candidateData =>
                                    potentialTargets
                                       .Contains(Battlers
                                                    .GetBattlerFromBattleIndex(candidateData)))
                         .ToList();

                slot.CurrentPP--;

                battler.CurrentMoves[moveIndex] = slot;

                yield return PerformMove(type,
                                         index,
                                         sanitizedTargets,
                                         slot.Move);
            }
            else
                yield return PerformMove(type,
                                         index,
                                         new List<(BattlerType Type, int Index)> {targets[0]},
                                         BattleManager.YAPUSettings.NoPPMove);
        }

        /// <summary>
        /// Tell the given battler to perform a move, ignoring PP.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">In battle index.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="move">Move to perform.</param>
        /// <param name="battler">Battler using the move. Can be overriden for battlers out of the field. (Weird stuff like Future Sight.)</param>
        /// <param name="ignoreStatus">Ignore the status callbacks?</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="hasBeenReflected">Has this move been reflected?</param>
        public IEnumerator ForcePerformMove(BattlerType type,
                                            int index,
                                            List<(BattlerType Type, int Index)> targets,
                                            Move move,
                                            Battler battler = null,
                                            float externalPowerMultiplier = 1,
                                            bool ignoreStatus = false,
                                            bool hasBeenReflected = false) =>
            PerformMove(type,
                        index,
                        targets,
                        move,
                        battler,
                        externalPowerMultiplier,
                        ignoreStatus);

        /// <summary>
        /// Tell the given battler to perform a move.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">In battle index.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="move">Move to perform.</param>
        /// <param name="battler">Battler using the move. Can be overriden for battlers out of the field. (Weird stuff like Future Sight.)</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoreStatus">Ignore the status callbacks?</param>
        /// <param name="hasBeenReflected">Has this move been reflected?</param>
        private IEnumerator PerformMove(BattlerType type,
                                        int index,
                                        List<(BattlerType Type, int Index)> targets,
                                        Move move,
                                        Battler battler = null,
                                        float externalPowerMultiplier = 1,
                                        bool ignoreStatus = false,
                                        bool hasBeenReflected = false)
        {
            battler ??= Battlers.GetBattlerFromBattleIndex(type, index);

            bool ignoresAbilities = battler.IgnoresOtherAbilities(BattleManager, move);

            Logger.Info(battler.GetNameOrNickName(BattleManager.Localizer)
                      + " will attempt to perform "
                      + BattleManager.Localizer[move.LocalizableName]
                      + ".");

            if (targets.Count == 0)
            {
                Logger.Warn("Move "
                          + move.name
                          + " must have at least a target (can even be itself). Move will fail.");

                DialogManager.ShowDialog("Battle/Move/Used",
                                         acceptInput: false,
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        battler.GetNameOrNickName(BattleManager.Localizer),
                                                        BattleManager.Localizer[move.LocalizableName]
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield break;
            }

            bool useMove = true;

            yield return battler.OnAboutToPerformMove(move,
                                                      BattleManager,
                                                      targets,
                                                      ignoreStatus,
                                                      ignoresAbilities,
                                                      shouldUse => useMove &= shouldUse);

            foreach (Battler opponent in Battlers.GetBattlersFighting())
                yield return opponent.OnOtherBattlerAboutToUseAMove(battler,
                                                                    move,
                                                                    BattleManager,
                                                                    targets,
                                                                    hasBeenReflected,
                                                                    ignoresAbilities,
                                                                    (shouldUse, newTargets) =>
                                                                    {
                                                                        useMove &= shouldUse;
                                                                        targets = newTargets;
                                                                    });

            if (!useMove)
            {
                Logger.Info(battler.GetNameOrNickName(BattleManager.Localizer) + " will not perform the move.");
                yield return move.OnMoveFailed(BattleManager, type, index, targets, externalPowerMultiplier);
                battler.SetLastPerformedMove(move, false, BattleManager);
                yield break;
            }

            yield return PerformMoveAfterCallbacks(type,
                                                   index,
                                                   battler,
                                                   targets,
                                                   move,
                                                   ignoresAbilities,
                                                   externalPowerMultiplier: externalPowerMultiplier);

            yield return battler.AfterUsingMove(move, targets, BattleManager, BattleManager.Localizer);
        }

        /// <summary>
        /// Perform a move after callbacks have been made.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">In battle index.</param>
        /// <param name="user">Reference to the user.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="move">Move to perform.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="isSecondTurn">Is it the second turn of a two turn move?</param>
        /// <param name="forceShowUsedMessage">Force showing the used message?</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        public IEnumerator PerformMoveAfterCallbacks(BattlerType type,
                                                     int index,
                                                     Battler user,
                                                     List<(BattlerType Type, int Index)> targets,
                                                     Move move,
                                                     bool ignoresAbilities,
                                                     bool isSecondTurn = false,
                                                     bool forceShowUsedMessage = false,
                                                     float externalPowerMultiplier = 1)
        {
            List<Transform> targetPositions = new();

            bool showMessageNormally = forceShowUsedMessage || move.ShowUsedDialog;

            if (showMessageNormally)
                DialogManager.ShowDialog("Battle/Move/Used",
                                         acceptInput: false,
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        user.GetNameOrNickName(BattleManager.Localizer),
                                                        BattleManager.Localizer[move.LocalizableName]
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            Dictionary<(BattlerType Type, int Index), float> accuracies = new();

            targets = UpdateTargetsWithLeftBattlers(type, index, targets, move);

            if (targets.Count == 0)
            {
                // If the message was never shown, we should show it here.
                if (!showMessageNormally)
                    DialogManager.ShowDialog("Battle/Move/Used",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            user.GetNameOrNickName(BattleManager.Localizer),
                                                            BattleManager.Localizer[move.LocalizableName]
                                                        },
                                             switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield return move.OnMoveFailed(BattleManager, type, index, targets, externalPowerMultiplier);

                user.SetLastPerformedMove(move, false, BattleManager);

                yield break;
            }

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                accuracies[(targetType, targetIndex)] =
                    CalculateAccuracyForTarget(user, target, move, ignoresAbilities);
            }

            yield return DialogManager.WaitForDialog;

            List<(BattlerType Type, int Index)> notFailedTargets = new();

            if (accuracies.Count == 1)
            {
                float accuracyChance = RandomProvider.Value01();
                (BattlerType Type, int Index) targetIndexes = accuracies.First().Key;

                Battler target = Battlers.GetBattlerFromBattleIndex(targetIndexes);

                if (accuracyChance > accuracies.First().Value)
                {
                    // If the message was never shown, we should show it here.
                    if (!showMessageNormally)
                        DialogManager.ShowDialog("Battle/Move/Used",
                                                 acceptInput: false,
                                                 localizableModifiers: false,
                                                 modifiers: new[]
                                                            {
                                                                user.GetNameOrNickName(BattleManager.Localizer),
                                                                BattleManager.Localizer[move.LocalizableName]
                                                            },
                                                 switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / BattleManager.BattleSpeed);
                }
                else
                {
                    // TODO: This code is duplicated below, refactor.
                    bool notFailed = true;

                    yield return target.AboutToBeHitByMove(move,
                                                           user,
                                                           BattleManager,
                                                           showMessageNormally,
                                                           wontFail => notFailed = wontFail);

                    if (notFailed) notFailedTargets.Add(targetIndexes);
                }
            }
            else
                foreach (((BattlerType Type, int Index) targetData, float accuracy) in accuracies)
                {
                    Battler target = Battlers.GetBattlerFromBattleIndex(targetData.Item1, targetData.Item2);

                    if (RandomProvider.Value01() > accuracy)
                    {
                        // If the message was never shown, we should show it here.
                        if (!showMessageNormally)
                            DialogManager.ShowDialog("Battle/Move/Used",
                                                     acceptInput: false,
                                                     localizableModifiers: false,
                                                     modifiers: new[]
                                                                {
                                                                    user.GetNameOrNickName(BattleManager.Localizer),
                                                                    BattleManager.Localizer[move.LocalizableName]
                                                                },
                                                     switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                        yield return DialogManager.ShowDialogAndWait("Battle/Move/Evaded",
                                                                     localizableModifiers: false,
                                                                     modifiers: target.GetNameOrNickName(BattleManager
                                                                        .Localizer),
                                                                     switchToNextAfterSeconds: 1.5f
                                                                       / BattleManager.BattleSpeed);
                    }
                    else
                    {
                        bool notFailed = true;

                        yield return target.AboutToBeHitByMove(move,
                                                               user,
                                                               BattleManager,
                                                               showMessageNormally,
                                                               wontFail => notFailed = wontFail);

                        if (notFailed) notFailedTargets.Add(targetData);
                    }
                }

            if (notFailedTargets.Count == 0)
            {
                yield return move.OnMoveFailed(BattleManager, type, index, notFailedTargets, externalPowerMultiplier);

                user.SetLastPerformedMove(move, false, BattleManager);

                yield break;
            }

            if (move.WillMoveFail(BattleManager,
                                  BattleManager.Localizer,
                                  type,
                                  index,
                                  ref notFailedTargets,
                                  0,
                                  0,
                                  ignoresAbilities,
                                  out string customFailMessage))
            {
                // If the message was never shown, we should show it here.
                if (!showMessageNormally)
                    DialogManager.ShowDialog("Battle/Move/Used",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            user.GetNameOrNickName(BattleManager.Localizer),
                                                            BattleManager.Localizer[move.LocalizableName]
                                                        },
                                             switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait(customFailMessage.IsNullEmptyOrWhiteSpace()
                                                                 ? "Battle/Move/NoEffect"
                                                                 : customFailMessage,
                                                             localizableModifiers: false,
                                                             modifiers: user.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield return move.OnMoveFailed(BattleManager, type, index, notFailedTargets, externalPowerMultiplier);

                user.SetLastPerformedMove(move, false, BattleManager);
                yield break;
            }

            foreach ((BattlerType targetType, int targetIndex) in notFailedTargets)
                targetPositions.Add(BattleManager.GetMonsterSprite(targetType, targetIndex).transform);

            yield return ExecuteAnimationAndEffect(type,
                                                   index,
                                                   user,
                                                   notFailedTargets,
                                                   move,
                                                   isSecondTurn,
                                                   ignoresAbilities,
                                                   externalPowerMultiplier,
                                                   targetPositions);
        }

        /// <summary>
        /// Calculate the accuracy for a move on a specific target.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <returns>The calculated accuracy.</returns>
        internal float CalculateAccuracyForTarget(Battler user, Battler target, Move move, bool ignoresAbilities)
        {
            float accuracy = 100;

            if (!move.HasInfiniteAccuracy(user, target, BattleManager)
             && !user.DoesBypassAllAccuracyChecksWhenUsing(move, target, BattleManager)
             && !target.DoesBypassAllAccuracyChecksWhenTargeted(move, user, ignoresAbilities, BattleManager))
                accuracy = move.CalculateAccuracy(user, target, ignoresAbilities, BattleManager);

            // No accuracy if target fainted.
            if (target.CurrentHP == 0) accuracy = 0;

            accuracy /= 100;

            Logger.Info("Accuracy to target "
                      + target.GetNameOrNickName(BattleManager.Localizer)
                      + ": "
                      + accuracy
                      + ".");

            return accuracy;
        }

        /// <summary>
        /// Execute the move's animation and effect.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">In battle index.</param>
        /// <param name="battler">Reference to the user.</param>
        /// <param name="targets">Move targets.</param>
        /// <param name="move">Move to perform.</param>
        /// <param name="isSecondTurn">Is it the second turn of a two turn move?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="targetPositions">Positions of the targets.</param>
        private IEnumerator ExecuteAnimationAndEffect(BattlerType type,
                                                      int index,
                                                      Battler battler,
                                                      List<(BattlerType Type, int Index)> targets,
                                                      Move move,
                                                      bool isSecondTurn,
                                                      bool ignoresAbilities,
                                                      float externalPowerMultiplier,
                                                      List<Transform> targetPositions)
        {
            bool userHasBeenOverriden = BattleManager.Battlers.GetBattlerFromBattleIndex(type, index) != battler;

            BattleMonsterSprite sprite = BattleManager.GetMonsterSprite(type, index);

            if (battler.Substitute.SubstituteEnabled) yield return sprite.HideSubstitute(BattleManager);

            int numberOfHits = move.GetNumberOfHits(BattleManager, type, index, targets, ignoresAbilities);

            Logger.Info("The move will hit " + numberOfHits + " times.");

            int i;

            bool wasMoveSuccessful = false;

            for (i = 0; i < numberOfHits; ++i)
            {
                // This can happen with recoils like rocky helmet or rough skin.
                // However, ignore it if the user has been overriden, like with Future Sight.
                if (battler.CurrentHP == 0 && !userHasBeenOverriden) break;

                TwoTurnMove twoTurnMove = null;

                if (isSecondTurn)
                {
                    twoTurnMove = (TwoTurnMove) move;

                    yield return twoTurnMove.PlaySecondTurnAnimation(BattleManager,
                                                                     BattleManager.BattleSpeed,
                                                                     type,
                                                                     index,
                                                                     battler,
                                                                     BattleManager.GetMonsterSprite(type, index)
                                                                        .transform,
                                                                     targets,
                                                                     targetPositions,
                                                                     ignoresAbilities);
                }
                else
                    yield return move.PlayAnimation(BattleManager,
                                                    BattleManager.BattleSpeed,
                                                    type,
                                                    index,
                                                    battler,
                                                    BattleManager.GetMonsterSprite(type, index).transform,
                                                    targets,
                                                    targetPositions,
                                                    ignoresAbilities);

                // Some battlers will replace the effect. Ex: Lightning rod.
                // For those, multihit only triggers once so removing them from the targets list
                // prevents both multiple hits and executing the effect on them.
                List<(BattlerType Type, int Index)> nonReplacingTargets = new();

                foreach ((BattlerType targetType, int targetIndex) in targets)
                {
                    Battler target = Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                    yield return target.ShouldReplaceMoveEffectWhenHit(move,
                                                                       Battlers.GetBattlerFromBattleIndex(type, index),
                                                                       ignoresAbilities,
                                                                       BattleManager,
                                                                       stillExecute =>
                                                                       {
                                                                           if (stillExecute)
                                                                               nonReplacingTargets.Add((targetType,
                                                                                               targetIndex));
                                                                       });
                }

                targets = nonReplacingTargets;

                Logger.Info("Executing move " + move.name + " effect.");

                if (isSecondTurn)
                    yield return twoTurnMove.ExecuteSecondEffect(BattleManager,
                                                                 BattleManager.Localizer,
                                                                 type,
                                                                 index,
                                                                 battler,
                                                                 targets,
                                                                 i,
                                                                 numberOfHits,
                                                                 externalPowerMultiplier,
                                                                 ignoresAbilities,
                                                                 success => wasMoveSuccessful = success);
                else
                    yield return move.ExecuteEffect(BattleManager,
                                                    BattleManager.Localizer,
                                                    type,
                                                    index,
                                                    battler,
                                                    targets,
                                                    i,
                                                    numberOfHits,
                                                    externalPowerMultiplier,
                                                    ignoresAbilities,
                                                    success => wasMoveSuccessful = success);

                if (wasMoveSuccessful
                 && move.HasSecondaryEffect()
                 && battler.CanPerformSecondaryEffectOfMove(targets, move, BattleManager))
                {
                    Logger.Info("Executing move " + move.name + " secondary effect.");

                    yield return move.ExecuteSecondaryEffect(BattleManager,
                                                             BattleManager.Localizer,
                                                             type,
                                                             index,
                                                             battler,
                                                             targets,
                                                             i,
                                                             numberOfHits,
                                                             externalPowerMultiplier,
                                                             ignoresAbilities);
                }

                if (!wasMoveSuccessful
                 || move.WillMoveFail(BattleManager,
                                      BattleManager.Localizer,
                                      type,
                                      index,
                                      ref targets,
                                      i,
                                      numberOfHits,
                                      ignoresAbilities,
                                      out string _))
                {
                    // TODO: Should we display the custom fail message here?

                    i++; // Breaking prevents running the last increment.
                    break;
                }

                yield return battler.AfterHittingWithMove(move, targets, BattleManager, BattleManager.Localizer);
            }

            if (move.IsMultiHit)
            {
                if (i == 0)
                    yield return DialogManager.ShowDialogAndWait("Battle/MoveHitTimes/Single",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / BattleManager.BattleSpeed);
                else
                    yield return DialogManager.ShowDialogAndWait("Battle/MoveHitTimes/Multiple",
                                                                 localizableModifiers: false,
                                                                 modifiers: i.ToString(),
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / BattleManager.BattleSpeed);
            }

            if (battler.Substitute.SubstituteEnabled) yield return sprite.ShowSubstitute(BattleManager.BattleSpeed);

            battler.SetLastPerformedMove(move, wasMoveSuccessful, BattleManager);
        }

        /// <summary>
        /// Update the targets of a move to use only left battlers.
        /// </summary>
        /// <returns>A new target dictionary.</returns>
        private List<(BattlerType Type, int Index)> UpdateTargetsWithLeftBattlers(BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> originalTargets,
            Move moveToUse)
        {
            List<(BattlerType Type, int Index)> reselectedTargets =
                moveToUse.SelectFinalTargets(BattleManager, userType, userIndex, originalTargets);

            List<(BattlerType Type, int Index)> targets = new();

            foreach ((BattlerType battlerType, int battlerIndex) in reselectedTargets)
                if (Battlers.IsBattlerFighting(battlerType, battlerIndex))
                    targets.Add((battlerType, battlerIndex));

            // If no target was found, try to have a target of the same type as substitute.
            // Only in case we were presumably attacking an enemy.
            if (targets.Count != 0
             || moveToUse.MovePossibleTargets is not (Move.PossibleTargets.Adjacent or Move.PossibleTargets.Enemies or
                    Move.PossibleTargets.AllButSelf))
                return targets;

            Logger.Info("No original targets available, trying to default.");

            (BattlerType enemyType, int _) = reselectedTargets.First();

            foreach (Battler battler in Battlers.GetBattlersFighting(enemyType))
            {
                targets.Add(Battlers.GetTypeAndIndexOfBattler(battler));
                break; // One is enough.
            }

            return targets;
        }
    }
}