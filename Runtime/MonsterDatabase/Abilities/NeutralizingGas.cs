using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability NeutralizingGas.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/NeutralizingGas", fileName = "NeutralizingGas")]
    public class NeutralizingGas : Ability
    {
        /// <summary>
        /// Items immune to this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Item> ImmuneItems;

        /// <summary>
        /// Abilities immune to this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Can other monsters use their ability?
        /// </summary>
        public override bool CanOtherMonsterUseAbility(Battler owner, Battler other, BattleManager battleManager) =>
            ImmuneItems.Contains(other.HeldItem) || ImmuneAbilities.Contains(other.GetAbility());

        /// <summary>
        /// Show a notification with the ability.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            ShowAbilityNotification(battler);
        }

        /// <summary>
        /// Trigger the ETB of abilities already on the field.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterLeavingBattle(battleManager, battler);

            foreach (Battler other in battleManager.Battlers.GetBattlersFighting().Where(other => other != battler))
                yield return other.GetAbility().OnMonsterEnteredBattle(battleManager, other);
        }
    }
}