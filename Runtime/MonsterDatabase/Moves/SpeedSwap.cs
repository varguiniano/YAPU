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
    /// Data class for the move SpeedSwap.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/SpeedSwap", fileName = "SpeedSwap")]
    public class SpeedSwap : SwapUserAndTargetStatMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Particles to display on the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D Particle;

        /// <summary>
        /// Reference to the post processing that can change battle color tones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject TonesPostProcessing;

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
            for (int i = 0; i < targets.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                Volume targetVolume = Instantiate(TonesPostProcessing).GetComponent<Volume>();

                yield return WaitAFrame;

                targetVolume.sharedProfile.TryGet(out ShadowsMidtonesHighlights targetEffect);

                targetEffect.shadows.overrideState = true;
                targetEffect.midtones.overrideState = true;
                targetEffect.highlights.overrideState = true;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                Vector4 original = targetEffect.midtones.GetValue<Vector4>();
                Vector4 tone = original;

                Vector4 purple = new(.91f,
                                     .33f,
                                     1f,
                                     0f);

                bool finished = false;

                DOTween.To(() => tone,
                           x => tone = x,
                           purple,
                           .1f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     //effect.shadows.SetValue(new Vector4Parameter(tone));
                                     targetEffect.midtones.SetValue(new Vector4Parameter(tone));
                                     //effect.highlights.SetValue(new Vector4Parameter(tone));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                finished = false;

                CoroutineRunner.RunRoutine(battleManager.GetMonsterSprite(userType, userIndex)
                                                        .FXAnimator.PlayAbsorb(.9f / battleManager.BattleSpeed,
                                                                               userPosition.position,
                                                                               particleTexture: Particle));

                yield return battleManager.GetMonsterSprite(targetType, targetIndex)
                                          .FXAnimator.PlayAbsorb(.9f / battleManager.BattleSpeed,
                                                                 targetPositions[i].position,
                                                                 particleTexture: Particle);

                yield return new WaitForSeconds(.2f);

                DOTween.To(() => tone,
                           x => tone = x,
                           original,
                           .1f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     //effect.shadows.SetValue(new Vector4Parameter(tone));
                                     targetEffect.midtones.SetValue(new Vector4Parameter(tone));
                                     //effect.highlights.SetValue(new Vector4Parameter(tone));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                targetEffect.shadows.overrideState = false;
                targetEffect.midtones.overrideState = false;
                targetEffect.highlights.overrideState = false;

                DOVirtual.DelayedCall(3, () => Destroy(targetVolume.gameObject));
            }
        }
    }
}