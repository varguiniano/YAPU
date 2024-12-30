using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move FlameWheel.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/FlameWheel", fileName = "FlameWheel")]
    public class FlameWheel : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Prefab for the flame wheel object.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Transform FlameWheelPrefab;

        /// <summary>
        /// Reference to the hit prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation HitPrefab;

        /// <summary>
        /// Play the move's animation.
        /// </summary>
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
                battleManager.Audio.PlayAudio(Audio, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                Transform flameWheel = Instantiate(FlameWheelPrefab, userPosition);

                flameWheel.DORotate(new Vector3(0, 0, 360), .1f, RotateMode.FastBeyond360)
                          .SetRelative(true)
                          .SetLoops(-1);

                yield return new WaitForSeconds(1.6f / speed);

                Vector3 originalUserPosition = userPosition.localPosition;
                Vector3 movement = new(.25f, 0, 0);

                yield return userPosition.DOLocalMove(userType == BattlerType.Ally
                                                          ? originalUserPosition + movement
                                                          : originalUserPosition - movement,
                                                      .25f / speed)
                                         .WaitForCompletion();

                BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPosition);

                userPosition.DOLocalMove(originalUserPosition, .25f / speed);

                yield return hit.PlayAnimation(speed);

                Destroy(hit.gameObject);

                yield return new WaitForSeconds(.5f / speed);

                foreach (SpriteRenderer flame in flameWheel.GetComponentsInChildren<SpriteRenderer>())
                    flame.DOFade(0, .25f / speed);

                yield return new WaitForSeconds(.25f / speed);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          flameWheel.DOKill();
                                          Destroy(flameWheel.gameObject);
                                      });
            }
        }
    }
}