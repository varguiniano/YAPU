using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move BugBuzz.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Bug/BugBuzz", fileName = "BugBuzz")]
    public class BugBuzz : StageChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the pulse prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation PulsePrefab;

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
                BasicSpriteAnimation[] pulses = new BasicSpriteAnimation[6];

                for (int i = 0; i < 6; ++i) pulses[i] = Instantiate(PulsePrefab, userPosition);

                yield return WaitAFrame;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                for (int i = 0; i < 6; ++i)
                {
                    CoroutineRunner.RunRoutine(pulses[i].PlayAnimation(speed, true));
                    pulses[i].GetCachedComponent<Transform>().DOMove(targetPosition.position, .7f / speed);

                    yield return new WaitForSeconds(.2f / speed);
                }

                yield return new WaitForSeconds(.6f / speed);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          foreach (BasicSpriteAnimation pulse in pulses) Destroy(pulse.gameObject);
                                      });
            }
        }
    }
}