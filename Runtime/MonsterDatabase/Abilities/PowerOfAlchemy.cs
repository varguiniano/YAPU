using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// PowerOfAlchemy ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/PowerOfAlchemy", fileName = "PowerOfAlchemy")]
    public class PowerOfAlchemy : Ability
    {
        /// <summary>
        /// Abilities immune to this one.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [SerializeField]
        [FoldoutGroup("Immunities")]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Called when another battler has fainted.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="otherType">Fainted battler.</param>
        /// <param name="otherIndex">Fainted battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnOtherBattlerFainted(Battler owner,
                                                          BattlerType otherType,
                                                          int otherIndex,
                                                          BattleManager battleManager)
        {
            (BattlerType ownType, int ownIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            Ability otherAbility = battleManager.Battlers.GetBattlerFromBattleIndex(otherType, otherIndex).GetAbility();

            if (ownType != otherType || ownIndex == otherIndex || ImmuneAbilities.Contains(otherAbility)) yield break;

            ShowAbilityNotification(owner);

            owner.SetAbility(otherAbility);

            otherAbility.ShowAbilityNotification(owner);
        }
    }
}