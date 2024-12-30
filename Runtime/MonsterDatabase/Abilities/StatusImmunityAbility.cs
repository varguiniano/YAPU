using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that grant immunity to a status.
    /// </summary>
    public abstract class StatusImmunityAbility : Ability
    {
        /// <summary>
        /// Status to cure after action or on etb.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private List<Status> StatusesToCure;

        /// <summary>
        /// Cure the status.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            if (!StatusesToCure.Contains(battler.GetStatus())) yield break;

            ShowAbilityNotification(battler);
            yield return battleManager.Statuses.RemoveStatus(battler);
        }

        /// <summary>
        /// Cure the status.
        /// </summary>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            yield return base.AfterAction(owner, action, user, battleManager);

            if (!StatusesToCure.Contains(owner.GetStatus())) yield break;

            ShowAbilityNotification(owner);
            yield return battleManager.Statuses.RemoveStatus(owner);
        }
    }
}