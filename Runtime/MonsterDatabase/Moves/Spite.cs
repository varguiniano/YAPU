using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Spite.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/Spite", fileName = "Spite")]
    public class Spite : Move
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the animation prefab for being angry.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AngryAnimation AngryAnimationPrefab;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            AngryAnimation angry = Instantiate(AngryAnimationPrefab, userPosition);

            yield return WaitAFrame;

            battleManager.Audio.PlayAudio(Audio);

            yield return angry.PlayAnimation(speed);

            DOVirtual.DelayedCall(3, () => Destroy(angry));
        }

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="ignoresAbilities"></param>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).LastPerformedAction.LastMove
         == null
         || base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                if (target.LastPerformedAction.LastMove != null)
                    for (int i = 0; i < target.CurrentMoves.Length; i++)
                    {
                        if (target.CurrentMoves[i].Move != target.LastPerformedAction.LastMove) continue;

                        target.CurrentMoves[i].CurrentPP = (byte)Mathf.Max(0, target.CurrentMoves[i].CurrentPP - 4);

                        yield return DialogManager.ShowDialogAndWait("Moves/Spite/Dialog",
                                                                     switchToNextAfterSeconds: 1.5f
                                                                       / battleManager.BattleSpeed,
                                                                     localizableModifiers: false,
                                                                     modifiers: new[]
                                                                         {
                                                                             target.GetNameOrNickName(battleManager
                                                                                .Localizer),
                                                                             battleManager.Localizer
                                                                                 [target.CurrentMoves[i].Move
                                                                                    .LocalizableName],
                                                                             target.CurrentMoves[i]
                                                                                .CurrentPP.ToString()
                                                                         });

                        finishedCallback.Invoke(true);
                        yield break;
                    }

                finishedCallback.Invoke(false);
                yield break;
            }
        }
    }
}