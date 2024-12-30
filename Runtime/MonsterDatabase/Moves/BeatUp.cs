using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for BeatUp.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/BeatUp", fileName = "BeatUp")]
    public class BeatUp : DamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the punch prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Punch PunchPrefab;

        /// <summary>
        /// Deal damage to a target.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="targetType">Target of the move.</param>
        /// <param name="targetIndex">Target of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="targets">All targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits"></param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        protected override IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           Battler target,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedMoveHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           Action<bool> finishedCallback,
                                                           bool forceSurvive = false)
        {
            yield return base.ExecuteDamageEffect(battleManager,
                                                  localizer,
                                                  userType,
                                                  userIndex,
                                                  GetAttackers(battleManager, userType, userIndex)[hitNumber],
                                                  targetType,
                                                  targetIndex,
                                                  target,
                                                  targets,
                                                  hitNumber,
                                                  expectedMoveHits,
                                                  externalPowerMultiplier,
                                                  ignoresAbilities,
                                                  finishedCallback,
                                                  forceSurvive);
        }

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
            foreach (Punch punch in targetPositions.Select(targetPosition => Instantiate(PunchPrefab, targetPosition)))
            {
                yield return WaitAFrame;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return punch.PlayAnimation(.3f / speed, speed);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(punch.gameObject);
                                      });
            }
        }

        /// <summary>
        /// Power formula: https://bulbapedia.bulbagarden.net/wiki/Beat_Up_(move)
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (int)(user.FormData.BaseStats[Stat.Attack] / 10f + 5);

        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="ignoresAbilities"></param>
        public override int GetNumberOfHits(BattleManager battleManager,
                                            BattlerType userType,
                                            int userIndex,
                                            List<(BattlerType Type, int Index)> targets,
                                            bool ignoresAbilities) =>
            GetAttackers(battleManager, userType, userIndex).Count;

        /// <summary>
        /// Get the attackers of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        private static List<Battler> GetAttackers(BattleManager battleManager,
                                                  BattlerType userType,
                                                  int userIndex) =>
            battleManager.Rosters.GetRoster(userType, userIndex)
                         .Where(attacker => attacker.CanBattle
                                         && attacker.GetStatus() == null)
                         .ToList();
    }
}