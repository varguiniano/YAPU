using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Sketch.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Sketch", fileName = "Sketch")]
    public class Sketch : Move
    {
        /// <summary>
        /// Learn the last move used by the target.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            // We only use one target.
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targets[0]);

            Move move = target.LastPerformedAction.LastMove;

            if (move == null)
            {
                finishedCallback.Invoke(false);

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                yield break;
            }

            int sketchIndex = -1;

            for (int i = 0; i < user.CurrentMoves.Length; i++)
                if (user.CurrentMoves[i].Move == this)
                {
                    sketchIndex = i;
                    break;
                }

            if (sketchIndex == -1)
            {
                finishedCallback.Invoke(false);

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Forgot",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetNameOrNickName(localizer),
                                                                        localizer[LocalizableName]
                                                                    });

            user.CurrentMoves[sketchIndex] = new MoveSlot(move);

            yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Learnt",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetNameOrNickName(localizer),
                                                                        localizer[move.LocalizableName]
                                                                    });

            finishedCallback.Invoke(true);
        }
    }
}