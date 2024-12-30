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
    /// Data class for the move Seed Bomb.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/SeedBomb", fileName = "SeedBomb")]
    public class SeedBomb : DamageMove
    {
        /// <summary>
        /// Animation for this move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SeedBombMoveAnimation AnimationPrefab;

        /// <summary>
        /// Audio for this move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

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
            for (int i = 0; i < targets.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                if (WillMoveFail(battleManager,
                                 battleManager.Localizer,
                                 userType,
                                 userIndex,
                                 targetType,
                                 targetIndex,
                                 ignoresAbilities))
                    yield break;

                SeedBombMoveAnimation animationInstance =
                    Instantiate(AnimationPrefab, battleManager.GetMonsterSprite(targetType, targetIndex).Pivot);

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                yield return animationInstance.Play(battleManager.GetMonsterSprite(userType, userIndex).Pivot,
                                                    targetType,
                                                    battleManager.BattleSpeed);

                DOVirtual.DelayedCall(2f, () => Destroy(animationInstance));
            }
        }
    }
}