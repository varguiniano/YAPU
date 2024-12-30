using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Unnerve.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Unnerve", fileName = "Unnerve")]
    public class Unnerve : Ability
    {
        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            ShowAbilityNotification(battler);

            yield return DialogManager.ShowDialogAndWait("Abilities/Unnerve/Dialog",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers:
                                                         battler.GetNameOrNickName(battleManager.Localizer));
        }

        /// <summary>
        /// Check if this battler allows other battlers to use a held item.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="other">Battler attempting to use the item.</param>
        /// <param name="itemToUse">Item they want to use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override bool CanOtherMonsterUseHeldItem(Battler owner,
                                                        Battler other,
                                                        Item itemToUse,
                                                        BattleManager battleManager) =>
            // Only allow allies to eat berries.
            itemToUse is not Berry
         || battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type
         == battleManager.Battlers.GetTypeAndIndexOfBattler(other).Type;
    }
}