using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that cure a status at the end of the turn before it ticks.
    /// </summary>
    public abstract class CureStatusBeforeItTicks : Ability
    {
        /// <summary>
        /// Chance to cure.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float CureChance = 1;

        /// <summary>
        /// Message to display when cured.
        /// </summary>
        [SerializeField]
        private string CureMessageLocalizationKey;

        /// <summary>
        /// Run a random chance and cure.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            if (battler.GetStatus() == null) yield break;
            if (battleManager.RandomProvider.Value01() > CureChance) yield break;

            ShowAbilityNotification(battler);
            
            yield return DialogManager.ShowDialogAndWait(CureMessageLocalizationKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.Statuses.RemoveStatus(battler);
        }
    }
}