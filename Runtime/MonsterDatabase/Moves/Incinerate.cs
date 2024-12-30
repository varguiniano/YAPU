using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Incinerate.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/Incinerate", fileName = "Incinerate")]
    public class Incinerate : DamageMove
    {
        /// <summary>
        /// Items destroyed by this move.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Effect")]
        [InfoBox("Berries are always destroyed.")]
        private List<Item> DestroyableItems;

        /// <summary>
        /// Abilities that will prevent stealing.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        private List<Ability> StealImmuneAbilities;

        /// <summary>
        /// This does have a secondary effect.
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Burn the berry or other items the target might have.
        /// </summary>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            foreach ((BattlerType targetType, int index) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, index);
                Item item = target.HeldItem;

                if (item == null || (item is not Berry && !DestroyableItems.Contains(item))) continue;

                if (target.CanUseAbility(battleManager, ignoresAbilities)
                 && StealImmuneAbilities.Contains(target.GetAbility()))
                {
                    target.GetAbility().ShowAbilityNotification(target);

                    yield break;
                }

                target.ConsumedItemData.ConsumedItem = item;
                target.ConsumedItemData.CanBeRecycled = false;
                target.ConsumedItemData.CanBeRecoveredAfterBattle = true;

                yield return DialogManager.ShowDialogAndWait("Moves/Incinerate/Effect",
                                                             switchToNextAfterSeconds: 1.5f,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            localizer[item.LocalizableName],
                                                                            target.GetNameOrNickName(localizer)
                                                                        });
            }
        }
    }
}