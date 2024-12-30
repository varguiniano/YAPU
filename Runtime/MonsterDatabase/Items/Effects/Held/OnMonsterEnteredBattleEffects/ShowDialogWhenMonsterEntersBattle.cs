using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnMonsterEnteredBattleEffects
{
    /// <summary>
    /// Data class for a held item effect that shows a message when the monster enters the battlefield.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/EntersBattle/ShowDialogWhenMonsterEntersBattle",
                     fileName = "ShowDialogWhenMonsterEntersBattle")]
    public class ShowDialogWhenMonsterEntersBattle : OnMonsterEnteredBattleEffect
    {
        /// <summary>
        /// Localization key of the message to show.
        /// </summary>
        [SerializeField]
        private string MessageKey;

        /// <summary>
        /// Called after the holder enters battle.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator OnMonsterEnteredBattle(Item item,
                                                           Battler battler,
                                                           BattleManager battleManager,
                                                           Action<bool> finished)
        {
            if (!ShowCondition(item, battler, battleManager)) yield break;
            
            item.ShowItemNotification(battler, battleManager.Localizer);

            yield return DialogManager.ShowDialogAndWait(MessageKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        item.GetName(battleManager.Localizer),
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f
                                                           / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Condition to check if the message should be shown.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it should be shown.</returns>
        protected virtual bool ShowCondition(Item item, Battler battler, BattleManager battleManager) => true;
    }
}