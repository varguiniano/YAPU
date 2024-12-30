using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the side status of Light Screen.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/DamageReduction",
                     fileName = "DamageReductionSideStatus")]
    public class DamageReductionSideStatus : SideStatus
    {
        /// <summary>
        /// Category affected by this status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [HideIf(nameof(AffectAllCategories))]
        private Move.Category CategoryToReduce;

        /// <summary>
        /// Affect all move categories?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool AffectAllCategories;

        /// <summary>
        /// Multiplier to the attack when in single battle.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float SingleBattleMultiplier;

        /// <summary>
        /// Multiplier to the attack when in double battle.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float DoubleBattleMultiplier;

        /// <summary>
        /// Abilities that are immune to this status.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier"></param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                      float multiplier,
                                                                      Battler user,
                                                                      Battler target,
                                                                      bool effectIgnoresAbilities,
                                                                      BattleManager battleManager,
                                                                      ILocalizer localizer,
                                                                      Action<float> finished)
        {
            if (user.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(user.GetAbility()))
            {
                user.GetAbility().ShowAbilityNotification(user);
                finished.Invoke(multiplier);
                yield break;
            }

            if (move.GetMoveCategory(user, target, effectIgnoresAbilities, battleManager) != CategoryToReduce
             && !AffectAllCategories)
            {
                finished.Invoke(multiplier);
                yield break;
            }

            finished.Invoke(multiplier
                          * (battleManager.BattleType == BattleType.SingleBattle
                                 ? SingleBattleMultiplier
                                 : DoubleBattleMultiplier));
        }
    }
}