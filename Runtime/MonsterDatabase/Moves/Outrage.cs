using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Outrage.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dragon/Outrage", fileName = "Outrage")]
    public class Outrage : FixationMove
    {
        /// <summary>
        /// Reference to the outrage sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the explosion FX.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect ExplosionFX;

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
            VisualEffect explosion = Instantiate(ExplosionFX, userPosition);

            battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

            yield return userPosition.DOShakePosition(2 / speed, .1f, 100).WaitForCompletion();

            explosion.enabled = true;
            explosion.Play();

            yield return new WaitForSeconds(1.3f / speed);

            explosion.Stop();

            DOVirtual.DelayedCall(3,
                                  () =>
                                  {
                                      Destroy(explosion.gameObject);
                                  });
        }
    }
}