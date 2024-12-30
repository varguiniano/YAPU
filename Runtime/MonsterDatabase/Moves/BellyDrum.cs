using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move BellyDrum.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/BellyDrum", fileName = "BellyDrum")]
    public class BellyDrum : StageChangeMove
    {
        /// <summary>
        /// Reference to the FX prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect FXPrefab;

        /// <summary>
        /// Move audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Fail the move if the user has less than half HP.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="localizer"></param>
        /// <param name="userType"></param>
        /// <param name="userIndex"></param>
        /// <param name="targetType"></param>
        /// <param name="targetIndex"></param>
        /// <param name="ignoresAbilities"></param>
        /// <returns></returns>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            return user.GetStats(battleManager)[Stat.Hp] * .5f >= user.CurrentHP
                || user.StatStage[Stat.Attack] == 6
                || base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);
        }

        /// <summary>
        /// Change the stages.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="user"></param>
        /// <param name="targets">Move targets.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
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
            yield return battleManager.BattlerHealth.ChangeLife(userType,
                                                                userIndex,
                                                                userType,
                                                                userIndex,
                                                                -(int) (battleManager.Battlers
                                                                           .GetBattlerFromBattleIndex(userType,
                                                                                userIndex)
                                                                           .GetStats(battleManager)[Stat.Hp]
                                                                      * .5f),
                                                                playAudio: false);

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
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
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

            VisualEffect fx = Instantiate(FXPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);

            yield return new WaitForSeconds(1.3f / speed);

            fx.Stop();

            DOVirtual.DelayedCall(3f, () => Destroy(fx.gameObject));
        }
    }
}