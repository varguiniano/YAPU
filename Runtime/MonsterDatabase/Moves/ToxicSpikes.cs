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
    /// Data class for the move ToxicSpikes.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/ToxicSpikes", fileName = "ToxicSpikes")]
    public class ToxicSpikes : SetLayeredSideStatusMove
    {
        /// <summary>
        /// Move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Move FX prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect FXPrefab;

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
            VisualEffect fx = Instantiate(FXPrefab, userPosition);

            yield return WaitAFrame;

            battleManager.Audio.PlayAudio(Audio, pitch: speed);

            yield return new WaitForSeconds(.1f / speed);

            fx.EnableAndPlay();

            yield return new WaitForSeconds(1.2f / speed);

            fx.Stop();

            DOVirtual.DelayedCall(3, () => Destroy(fx.gameObject));
        }
    }
}