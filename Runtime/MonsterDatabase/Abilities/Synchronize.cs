using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Synchronize ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Synchronize", fileName = "Synchronize")]
    public class Synchronize : Ability
    {
        /// <summary>
        /// Status compatible with this ability.
        /// </summary>
        [SerializeField]
        private List<Status> CompatibleStatuses;

        /// <summary>
        /// Called when the a status is added to the owner.
        /// </summary>
        /// <param name="owner">Reference to the monster instance.</param>
        /// <param name="userType">Monster that caused the status.</param>
        /// <param name="userIndex">Monster that caused the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnStatusAddedInBattle(Battler owner,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          BattleManager battleManager)

        {
            if (!CompatibleStatuses.Contains(owner.GetStatus())) yield break;

            (BattlerType ownerType, int ownerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);
            Status status = owner.GetStatus();

            if (user == null || user == owner || user.GetStatus() != null) yield break;

            ShowAbilityNotification(owner);

            yield return battleManager.Statuses.AddStatus(status, user, ownerType, ownerIndex);
        }

        /// <summary>
        /// Have encounters always be the same nature as the owner.
        /// </summary>
        public override Nature ModifyEncounterNature(MonsterInstance owner, EncounterType encounterType) =>
            owner.StatData.Nature;
    }
}