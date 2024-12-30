using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for moves similar to Absorb.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/Absorb", fileName = "Absorb")]
    public class AbsorbMove : DamageMove
    {
        /// <summary>
        /// Reference to the hit prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected BasicSpriteAnimation HitPrefab;

        /// <summary>
        /// Size of the particles over time.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected AnimationCurve ParticlesSizeOverTime;

        /// <summary>
        /// Audio for the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected AudioReference Audio;

        /// <summary>
        /// Percentage of HP drain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrain = 1f / 8f;

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
            for (int i = 0; i < targets.Count; ++i)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

                BattleMonsterSprite targetSprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPositions[i].position, Quaternion.identity);

                CoroutineRunner.Instance.StartCoroutine(hit.PlayAnimation(speed,
                                                                          finished: () => Destroy(hit.gameObject)));

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                yield return sprite.FXAnimator.PlayAbsorb(2f / battleManager.BattleSpeed,
                                                          sprite.transform.position,
                                                          targetSprite.transform,
                                                          spawnRadius: .2f,
                                                          sizeOverLifetime: ParticlesSizeOverTime);

                sprite.FXAnimator.PlayBoost(battleManager.BattleSpeed, false);
            }
        }

        /// <summary>
        /// Deal the damage and calculate a chance to flinch.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="targetType">Target of the move.</param>
        /// <param name="targetIndex">Target of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="targets">All targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedMoveHits"></param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        protected override IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           Battler target,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedMoveHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           Action<bool> finishedCallback,
                                                           bool forceSurvive = false)
        {
            yield return base.ExecuteDamageEffect(battleManager,
                                                  localizer,
                                                  userType,
                                                  userIndex,
                                                  user,
                                                  targetType,
                                                  targetIndex,
                                                  target,
                                                  targets,
                                                  hitNumber,
                                                  expectedMoveHits,
                                                  externalPowerMultiplier,
                                                  ignoresAbilities,
                                                  finishedCallback,
                                                  forceSurvive);

            int hpToDrain = Mathf.Max((int) (LastDamageMade * HPDrain), 1);

            hpToDrain = (int) (hpToDrain * user.CalculateDrainHPMultiplier(battleManager, target));

            hpToDrain = (int) (hpToDrain
                             * target.CalculateDrainerDrainHPMultiplier(battleManager, user, ignoresAbilities));

            if (hpToDrain > 0 && !user.CanHeal(battleManager)) yield break;

            yield return battleManager.BattlerHealth.ChangeLife(userType, userIndex, userType, userIndex, hpToDrain);

            battleManager.Animation.UpdatePanels();

            yield return DialogManager.ShowDialogAndWait("Battle/HealthDrained",
                                                         localizableModifiers: false,
                                                         modifiers: target.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }
    }
}