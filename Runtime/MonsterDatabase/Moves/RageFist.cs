using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for the move RageFist.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/RageFist", fileName = "RageFist")]
    public class RageFist : DamageAndSetVolatileMove
    {
        // TODO: Animation.

        /// <summary>
        /// Max multiplier to apply to the power.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private uint MaxMultiplier;

        /// <summary>
        /// Species that evolve after using this move several times and therefore need to count its uses.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<MonsterEntry> SpeciesToCountUses;

        /// <summary>
        /// Count towards evolution.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            if (SpeciesToCountUses.Contains(user.Species)) user.ExtraData.EvolutionCounter++;

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
        }

        /// <summary>
        /// Multiply the damage by the amount of times the user has been hit.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            RageFistStatus rageFistStatus = (RageFistStatus) Status;

            // Always one more than the actual hits.
            if (rageFistStatus.GetHitTimes(user, out uint hitTimes))
                hitTimes++;
            else
                hitTimes = 1;

            return (int) (base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
                        * Mathf.Min(hitTimes, MaxMultiplier));
        }
    }
}