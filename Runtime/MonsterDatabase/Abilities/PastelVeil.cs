using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability PastelVeil.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/PastelVeil", fileName = "PastelVeil")]
    public class PastelVeil : Ability
    {
        /// <summary>
        /// Statuses to prevent for allies. Direct immunities are set in the status itself.
        /// </summary>
        [SerializeField]
        private List<Status> PreventedStatuses;

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public override IEnumerator CanAnyMonsterAddStatus(Status status,
                                                           Battler owner,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           BattleManager battleManager,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Action<bool> callback)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (!PreventedStatuses.Contains(status)
             || battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type != targetType
             || (user.CanUseAbility(battleManager, IgnoresOtherAbilities(battleManager, owner, null))
              && !AffectsUserOfEffect(user, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager)))
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(owner);
            callback.Invoke(false);
        }
    }
}