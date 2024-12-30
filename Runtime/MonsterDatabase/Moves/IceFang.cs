using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for IceFang.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/IceFang", fileName = "IceFang")]
    public class IceFang : StatusChanceDamageMove
    {
        /// <summary>
        /// Move sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Bite animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation BiteAnimationPrefab;

        /// <summary>
        /// Prefab for the ice FX.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private IceFX IcePrefab;

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
                BasicSpriteAnimation bite = Instantiate(BiteAnimationPrefab, targetPosition);
                IceFX ice = Instantiate(IcePrefab, targetPosition);

                yield return WaitAFrame;

                battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

                CoroutineRunner.RunRoutine(bite.PlayAnimation(speed,
                                                              finished: () => bite.GetCachedComponent<SpriteRenderer>()
                                                                           .DOFade(0,
                                                                                .2f / speed)));

                yield return new WaitForSeconds(.2f / speed);

                yield return ice.PlayAnimation(.3f, speed);

                DOVirtual.DelayedCall(3f,
                                      () =>
                                      {
                                          Destroy(ice.gameObject);
                                          Destroy(bite.gameObject);
                                      });
            }
        }
    }
}