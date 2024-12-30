using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Trace ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Trace", fileName = "Trace")]
    public class Trace : Ability
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        protected VolatileStatus Status;

        /// <summary>
        /// Abilities that can't be traced.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Ability> UntraceableAbilities;

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return TriggerEffect(battleManager, battler);
        }

        /// <summary>
        /// Also trigger after every action if it hasn't been replaced yet.
        /// </summary>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            yield return TriggerEffect(battleManager, owner);
        }

        /// <summary>
        /// Trigger the ability.
        /// </summary>
        private IEnumerator TriggerEffect(BattleManager battleManager, Battler battler)
        {
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            List<Battler> candidates = battleManager
                                      .Battlers.GetBattlersFighting(userType == BattlerType.Ally
                                                                        ? BattlerType.Enemy
                                                                        : BattlerType.Ally)
                                      .Where(candidate =>
                                                 candidate != battler
                                              && !UntraceableAbilities.Contains(candidate.GetAbility()))
                                      .ToList();

            if (candidates.IsEmpty()) yield break;

            ShowAbilityNotification(battler);

            Battler target = candidates.Random();

            yield return battleManager.Statuses.AddStatus(Status,
                                                          -1,
                                                          battler,
                                                          userType,
                                                          userIndex,
                                                          IgnoresOtherAbilities(battleManager, battler, null),
                                                          target.GetAbility());

            target.GetAbility().ShowAbilityNotification(battler);
        }
    }
}