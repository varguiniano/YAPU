using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an effect that restores the targets lowered stats when used..
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RestoreTargetsLoweredStats",
                     fileName = "RestoreTargetsLoweredStats")]
    public class RestoreTargetsLoweredStats : UseOnTargetItemEffect
    {
        /// <summary>
        /// Set the status.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public override IEnumerator UseOnBattler(Item item,
                                                 Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer,
                                                 Action<bool> finished,
                                                 bool wasFlung = false)
        {
            if (!battler.ResetLoweredStatStages()) yield break;

            yield return DialogManager.ShowDialogAndWait("Battle/RestoredLoweredStats",
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            finished.Invoke(true);
        }
    }
}