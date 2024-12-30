using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will heal a status after an action has been performed.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/HealStatus", fileName = "HealStatusEffect")]
    public class HealStatusAfterAction : PostActionCallbackItemEffect
    {
        /// <summary>
        /// Status to be healed by this effect.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        [SerializeField]
        private Status StatusToHeal;

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterAction(Item item,
                                                Battler battler,
                                                BattleAction action,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            if (battler.GetStatus() != StatusToHeal)
            {
                finished.Invoke(false);
                yield break;
            }
            
            item.ShowItemNotification(battler, battleManager.Localizer);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.GetMonsterSprite(type, index).EatBerry(battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/EatBerry",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        item.GetName(localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battler.RemoveStatus(localizer, false);

            yield return DialogManager.ShowDialogAndWait("Battle/HealStatus",
                                                         localizableModifiers: false,
                                                         modifiers:
                                                         battler.GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            battleManager.Battlers.GetPanel(type, index).UpdatePanel(battleManager.BattleSpeed);

            finished?.Invoke(true);
        }
    }
}