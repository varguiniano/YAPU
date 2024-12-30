using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move IcicleCrash.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/IcicleCrash", fileName = "IcicleCrash")]
    public class IcicleCrash : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's audio
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the stinger prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Transform IciclePrefab;

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
            for (int i = 0; i < targetPositions.Count; i++)
            {
                Transform targetPosition = targetPositions[i];

                battleManager.Audio.PlayAudio(Audio);

                Transform icicle = Instantiate(IciclePrefab, userPosition);
                IceFX ice = Instantiate(IcePrefab, targetPosition);

                icicle.rotation = Quaternion.Euler(0, 0, targets[i].Item1 == BattlerType.Ally ? -135 : 45);

                yield return new WaitForSeconds(.33f / speed);

                yield return icicle.DOMove(targetPosition.position, .12f / speed).WaitForCompletion();

                Destroy(icicle.gameObject);

                yield return ice.PlayAnimation(1f / speed, speed);

                DOVirtual.DelayedCall(3, () => Destroy(ice.gameObject));
            }
        }
    }
}