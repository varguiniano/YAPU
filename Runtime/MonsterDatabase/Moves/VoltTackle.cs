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
    /// Data class for VoltTackle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/VoltTackle", fileName = "VoltTackle")]
    public class VoltTackle : StatusChanceDamageWithRecoilBasedOnDamageDone
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation ParalysisPrefab;

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
            for (int i = 0; i < targetPositions.Count; i++)
            {
                Transform targetPosition = targetPositions[i];
                battleManager.Audio.PlayAudio(Audio, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                Transform userPivot = battleManager.GetMonsterSprite(userType, userIndex).Pivot;
                Transform targetPivot = battleManager.GetMonsterSprite(targets[i].Item1, targets[i].Item2).Pivot;

                BasicSpriteAnimation userElectricity = Instantiate(ParalysisPrefab, userPivot);
                BasicSpriteAnimation targetElectricity = Instantiate(ParalysisPrefab, targetPivot);

                yield return WaitAFrame;

                yield return userElectricity.PlayAnimation(speed, hideOnFinish: true);
                yield return userElectricity.PlayAnimation(speed * 2.5f, hideOnFinish: true);

                yield return new WaitForSeconds(.1f / speed);

                yield return userElectricity.PlayAnimation(speed, hideOnFinish: true);

                Vector3 originalUserPosition = userPosition.localPosition;
                Vector3 movement = new(.25f, 0, 0);

                yield return userPosition.DOLocalMove(userType == BattlerType.Ally
                                                          ? originalUserPosition + movement
                                                          : originalUserPosition - movement,
                                                      .25f / speed)
                                         .WaitForCompletion();

                BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPosition);

                userPosition.DOLocalMove(originalUserPosition, .25f / speed);

                yield return hit.PlayAnimation(speed, hideOnFinish: true);

                yield return new WaitForSeconds(.6f / speed);

                yield return targetElectricity.PlayAnimation(speed, hideOnFinish: true);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(hit.gameObject);
                                          Destroy(userElectricity.gameObject);
                                          Destroy(targetElectricity.gameObject);
                                      });
            }
        }
    }
}