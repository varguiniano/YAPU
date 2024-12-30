using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for BrickBreak.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/BrickBreak", fileName = "BrickBreak")]
    public class BrickBreak : DamageAndRemoveStatusMove
    {
        /// <summary>
        /// Reference to the move sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private ChopHandAnimation AnimationPrefab;

        /// <summary>
        /// Play the animation.
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
            foreach (ChopHandAnimation hand in targetPositions.Select(targetPosition =>
                                                                          Instantiate(AnimationPrefab, targetPosition)))
            {
                yield return WaitAFrame;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return hand.PlayAnimation(speed);

                DOVirtual.DelayedCall(3, () => Destroy(hand.gameObject));
            }
        }
    }
}