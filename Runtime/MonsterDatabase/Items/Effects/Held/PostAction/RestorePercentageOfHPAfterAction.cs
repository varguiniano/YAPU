using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will heal a percentage of HP after an action has been performed and a threshold is met.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/RestorePercentageOfHPAfterAction",
                     fileName = "RestorePercentageOfHPAfterAction")]
    public class RestorePercentageOfHPAfterAction : PostActionCallbackItemEffect
    {
        /// <summary>
        /// Percentage of HP to change.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPChange;

        /// <summary>
        /// HP will be restored if it is under this threshold.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float Threshold = .5f;

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
            if ((float) battler.CurrentHP / MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) > Threshold
             || !battler.CanBattle
             || !battler.CanHeal(battleManager))
            {
                finished.Invoke(false);
                yield break;
            }

            item.ShowItemNotification(battler, battleManager.Localizer);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            int hpToChange = (int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) * HPChange);

            int amount = 0;

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                battler,
                                                                hpToChange,
                                                                finished: (finalAmount, _) =>
                                                                          {
                                                                              amount = finalAmount;
                                                                          });

            Logger.Info("Restored HP with effect "
                      + name
                      + " on "
                      + battler.Species.name
                      + ", HP changed by "
                      + amount
                      + ".");

            // TODO: Move this to a separate effect?
            yield return battleManager.GetMonsterSprite(type, index).EatBerry(battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/EatBerry",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        item.GetName(localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/RecoverHP",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        amount.ToString()
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            battleManager.Battlers.GetPanel(type, index).UpdatePanel(battleManager.BattleSpeed);

            yield return ExtraEffect(item, battler, action, battleManager, localizer);

            finished?.Invoke(true);
        }

        /// <summary>
        /// Method that can be used to add extra effects by the inheritors.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        protected virtual IEnumerator ExtraEffect(Item item,
                                                  Battler battler,
                                                  BattleAction action,
                                                  BattleManager battleManager,
                                                  ILocalizer localizer)
        {
            yield break;
        }
    }
}