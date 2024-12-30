using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Blizzard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/Blizzard", fileName = "Blizzard")]
    public class Blizzard : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to this moves audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected AudioReference Audio;

        /// <summary>
        /// Animation prefab for the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected LateralParticlesMoveAnimation AnimationPrefab;

        /// <summary>
        /// Ice particle.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D Particle;

        /// <summary>
        /// Ice FX prefab.
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
            LateralParticlesMoveAnimation animation = Instantiate(AnimationPrefab);

            List<IceFX> ices = targetPositions.Select(targetPosition => Instantiate(IcePrefab, targetPosition))
                                              .ToList();

            yield return WaitAFrame;

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);
            animation.PlayFX(targets.First().Item1, Particle);

            foreach (Transform targetPosition in targetPositions)
                targetPosition.DOShakePosition(1.5f / speed, .05f, 100);

            yield return new WaitForSeconds(1.7f / speed);

            animation.StopVFX();

            yield return new WaitForSeconds(.2f / speed);

            foreach (IceFX ice in ices) CoroutineRunner.RunRoutine(ice.PlayAnimation(1.3f / speed, speed));

            yield return new WaitForSeconds(1.3f / speed);

            DOVirtual.DelayedCall(3,
                                  () =>
                                  {
                                      Destroy(animation);

                                      foreach (IceFX ice in ices) Destroy(ice.gameObject);
                                  });
        }
    }
}