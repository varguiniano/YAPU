using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move String Shot.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Bug/StringShot", fileName = "StringShot")]
    public class StringShot : StageChangeMove
    {
        /// <summary>
        /// Move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

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
        /// Reference to the web prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer WebPrefab;

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
            for (int i = 0; i < targetPositions.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                SpriteRenderer web = Instantiate(WebPrefab, targetPositions[i]);

                yield return WaitAFrame;

                AudioManager.Instance.PlayAudio(Audio, pitch: speed);

                yield return sprite.FXAnimator.PlayAbsorb(1f / speed,
                                                          sprite.transform.position,
                                                          userPosition,
                                                          spawnRadius: 0,
                                                          sizeOverLifetime: BeamSize,
                                                          spawnRate: 500,
                                                          particleTexture: BeamParticle);

                yield return new WaitForSeconds(0.8f / speed);

                yield return web.DOFade(1, .1f / speed).WaitForCompletion();

                yield return new WaitForSeconds(0.4f / speed);

                yield return web.DOFade(0, .1f / speed).WaitForCompletion();

                DOVirtual.DelayedCall(3, () => Destroy(web.gameObject));
            }
        }
    }
}