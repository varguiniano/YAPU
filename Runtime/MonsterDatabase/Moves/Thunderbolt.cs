using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Thunderbolt.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/Thunderbolt", fileName = "Thunderbolt")]
    public class Thunderbolt : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the bolt prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation BoltPrefab;

        /// <summary>
        /// Reference to the electric ball prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation BallPrefab;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject PostProPrefab;

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
            foreach (Transform targetPosition in targetPositions)
            {
                BasicSpriteAnimation bolt = Instantiate(BoltPrefab, targetPosition);
                BasicSpriteAnimation ball = Instantiate(BallPrefab, targetPosition);
                Volume volume = Instantiate(PostProPrefab).GetComponent<Volume>();

                yield return WaitAFrame;

                volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

                effect.shadows.overrideState = true;
                effect.midtones.overrideState = true;
                effect.highlights.overrideState = true;
                float tone = NormalTone;

                battleManager.Audio.PlayAudio(Audio, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                DOTween.To(() => tone,
                           x => tone = x,
                           HightestTone,
                           .15f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 })
                       .OnComplete(() =>
                                   {
                                       DOTween.To(() => tone,
                                                  x => tone = x,
                                                  NormalTone,
                                                  .15f / battleManager.BattleSpeed)
                                              .OnUpdate(() =>
                                                        {
                                                            effect.shadows
                                                                  .SetValue(new
                                                                                Vector4Parameter(new
                                                                                    Vector4(1,
                                                                                        1,
                                                                                        1,
                                                                                        tone)));

                                                            effect.midtones
                                                                  .SetValue(new
                                                                                Vector4Parameter(new
                                                                                    Vector4(1,
                                                                                        1,
                                                                                        1,
                                                                                        tone)));

                                                            effect.highlights
                                                                  .SetValue(new
                                                                                Vector4Parameter(new
                                                                                    Vector4(1,
                                                                                        1,
                                                                                        1,
                                                                                        tone)));
                                                        });
                                   });

                CoroutineRunner.RunRoutine(bolt.PlayAnimation(speed, true));

                yield return ball.PlayAnimation(speed, true);

                effect.shadows.overrideState = false;
                effect.midtones.overrideState = false;
                effect.highlights.overrideState = false;

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(bolt.gameObject);
                                          Destroy(ball.gameObject);
                                          Destroy(volume.gameObject);
                                      });
            }
        }
    }
}