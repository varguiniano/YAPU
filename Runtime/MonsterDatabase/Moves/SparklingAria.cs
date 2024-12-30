using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move SparklingAria.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/SparklingAria", fileName = "SparklingAria")]
    public class SparklingAria : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Status that this move heals.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Status StatusToHeal;

        /// <summary>
        /// Does have secondary effect.
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Heal the status.
        /// </summary>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            yield return base.ExecuteSecondaryEffect(battleManager,
                                                     localizer,
                                                     userType,
                                                     userIndex,
                                                     user,
                                                     targets,
                                                     hitNumber,
                                                     expectedHits,
                                                     externalPowerMultiplier,
                                                     ignoresAbilities);

            foreach (Battler target in targets.Select(targetData =>
                                                          battleManager.Battlers.GetBattlerFromBattleIndex(targetData)))
                if (target.GetStatus() == StatusToHeal)
                    yield return battleManager.Statuses.RemoveStatus(target);
        }
    }
}