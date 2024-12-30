using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move BurningJealousy.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/BurningJealousy", fileName = "BurningJealousy")]
    public class BurningJealousy : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Status this move inflicts when the target has increased its stats this turn.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private Status StatusToInflict;

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Execute the secondary effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
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
            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                if (!target.IncreasedStatsThisTurn) yield break;

                if (!target.IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                bool canAdd = true;

                yield return target.CanAddStatus(StatusToInflict,
                                                 battleManager,
                                                 userType,
                                                 userIndex,
                                                 ignoresAbilities,
                                                 true,
                                                 add => canAdd = add);

                if (canAdd)
                    yield return
                        battleManager.Statuses.AddStatus(StatusToInflict,
                                                         target,
                                                         userType,
                                                         userIndex,
                                                         ignoreAbilities: ignoresAbilities);
            }
        }
    }
}