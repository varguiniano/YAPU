using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Solar Beam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/SolarBeam", fileName = "SolarBeam")]
    public class SolarBeam : TwoTurnDamageMove
    {
        /// <summary>
        /// Reference to the charging audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference ChargingAudio;

        /// <summary>
        /// Animation curve for the charge size.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AnimationCurve ChargeSize;

        /// <summary>
        /// Reference to the charging audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference BeamAudio;

        /// <summary>
        /// Animation curve for the beam size.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AnimationCurve BeamSize;

        /// <summary>
        /// Beam particle.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D BeamParticle;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject BackgroundLight;

        /// <summary>
        /// Normal value for midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float NormalTone;

        /// <summary>
        /// Highest value for the midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float HightestTone;

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

            AudioManager.Instance.PlayAudio(ChargingAudio, pitch: speed);

            yield return sprite.FXAnimator.PlayAbsorb(1.3f / speed,
                                                      sprite.transform.position,
                                                      spawnRadius: .5f,
                                                      sizeOverLifetime: ChargeSize);
        }

        /// <summary>
        /// Play the animation for the second turn of this move.
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
        public override IEnumerator PlaySecondTurnAnimation(BattleManager battleManager,
                                                            float speed,
                                                            BattlerType userType,
                                                            int userIndex,
                                                            Battler user,
                                                            Transform userPosition,
                                                            List<(BattlerType Type, int Index)> targets,
                                                            List<Transform> targetPositions,
                                                            bool ignoresAbilities)
        {
            for (int i = 0; i < targetPositions.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                Volume volume = Instantiate(BackgroundLight).GetComponent<Volume>();

                volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

                effect.shadows.overrideState = true;
                effect.midtones.overrideState = true;
                effect.highlights.overrideState = true;

                float tone = NormalTone;

                AudioManager.Instance.PlayAudio(BeamAudio, pitch: speed);

                CoroutineRunner.RunRoutine(battleManager.BattleCamera.ShakeCamera(3.5f / speed, .5f));

                DOTween.To(() => tone,
                           x => tone = x,
                           HightestTone,
                           .5f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 });

                yield return sprite.FXAnimator.PlayAbsorb(2.8f / speed,
                                                          sprite.transform.position,
                                                          userPosition,
                                                          spawnRadius: 0,
                                                          sizeOverLifetime: BeamSize,
                                                          spawnRate: 500,
                                                          particleTexture: BeamParticle);

                bool finished = false;

                DOTween.To(() => tone,
                           x => tone = x,
                           NormalTone,
                           .5f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                effect.shadows.overrideState = false;
                effect.midtones.overrideState = false;
                effect.highlights.overrideState = false;

                DOVirtual.DelayedCall(2f, () => Destroy(volume.gameObject));
            }
        }
    }
}