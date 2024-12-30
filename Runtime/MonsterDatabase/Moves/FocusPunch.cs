using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for FocusPunch.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/FocusPunch", fileName = "FocusPunch")]
    public class FocusPunch : DamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the punch prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Punch PunchPrefab;

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="customFailMessage">Custom message to be used when the move fails.</param>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (!user.ReceivedDamageThisTurn)
                return base.WillMoveFail(battleManager,
                                         localizer,
                                         userType,
                                         userIndex,
                                         ref targets,
                                         hitNumber,
                                         expectedHits,
                                         ignoresAbilities,
                                         out customFailMessage);

            customFailMessage = "Moves/FocusPunch/LostFocus";
            return true;
        }

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
            foreach (Punch punch in targetPositions.Select(targetPosition => Instantiate(PunchPrefab, targetPosition)))
            {
                yield return WaitAFrame;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return punch.PlayAnimation(.3f / speed, speed);

                DOVirtual.DelayedCall(3, () => Destroy(punch.gameObject));
            }
        }
    }
}