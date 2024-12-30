using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will raise the critical stage when the HP is bellow a threshold.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/RaiseCriticalIfLowHPAfterAction",
                     fileName = "RaiseCriticalIfLowHPAfterAction")]
    public class RaiseCriticalIfLowHPAfterAction : PostActionCallbackItemEffect
    {
        /// <summary>
        /// Threshold of the HP that triggers this.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float HPThreshold = .25f;

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
            float threshold = HPThreshold;

            if (item is Berry berry && battler.CanUseAbility(battleManager, false))
                threshold = battler.GetAbility().ModifyBerryHPThreshold(berry, threshold, battler, battleManager);
            
            if ((float)battler.CurrentHP / MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager)
              > threshold)
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

            // TODO: Ripen.
            yield return battleManager.BattlerStats.ChangeCriticalStage(type, index, 1, type, index);

            finished?.Invoke(true);
        }
    }
}