using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move DazzlingGleam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fairy/DazzlingGleam", fileName = "DazzlingGleam")]
    public class DazzlingGleam : DamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the post processing that can change battle color tones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject TonesPostProcessing;

        /// <summary>
        /// Pink color to set.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector4 Pink;

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
            Volume volume = Instantiate(TonesPostProcessing).GetComponent<Volume>();

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            battleManager.Audio.PlayAudio(Sound, pitch: speed);

            Vector4 original = effect.midtones.GetValue<Vector4>();
            Vector4 tone = original;

            bool finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       Pink,
                       .3f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(tone));
                                 effect.midtones.SetValue(new Vector4Parameter(tone));
                                 effect.highlights.SetValue(new Vector4Parameter(tone));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       original,
                       1.4f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(tone));
                                 effect.midtones.SetValue(new Vector4Parameter(tone));
                                 effect.highlights.SetValue(new Vector4Parameter(tone));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            effect.shadows.overrideState = false;
            effect.midtones.overrideState = false;
            effect.highlights.overrideState = false;

            DOVirtual.DelayedCall(3, () => Destroy(volume.gameObject));
        }
    }
}