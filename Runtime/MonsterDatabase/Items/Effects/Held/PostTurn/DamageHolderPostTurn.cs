using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostTurn
{
    /// <summary>
    /// Data class for a held item effect that will damage a percentage of HP after each turn.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostTurn/DamageHolderPostTurn",
                     fileName = "DamageHolderPostTurn")]
    public class DamageHolderPostTurn : PostTurnCallbackItemEffect
    {
        /// <summary>
        /// Percentage of HP to change.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPChange;

        /// <summary>
        /// Abilities immune to this item.
        /// </summary>
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterTurnPreStatus(Item item,
                                                       Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer,
                                                       Action<bool> finished)
        {
            finished.Invoke(false);
            yield break;
        }

        /// <summary>
        /// Called once after each turn.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterTurnPostStatus(Item item,
                                                        Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        Action<bool> finished)
        {
            item.ShowItemNotification(battler, battleManager.Localizer);

            if (battler.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(battler.GetAbility()))
            {
                battler.GetAbility().ShowAbilityNotification(battler);
                finished.Invoke(false);
                yield break;
            }

            int hpToChange =
                Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) * HPChange), 1);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            int amount = 0;

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                battler,
                                                                -hpToChange,
                                                                isSecondaryDamage: true,
                                                                finished: (finalAmount, _) =>
                                                                          {
                                                                              amount = finalAmount;
                                                                          });

            if (amount == 0)
            {
                finished.Invoke(false);
                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Items/BlackSludge/EffectDamage",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        item.GetName(localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            battleManager.Battlers.GetPanel(type, index).UpdatePanel(battleManager.BattleSpeed, tween: true);

            finished.Invoke(false);
        }
    }
}