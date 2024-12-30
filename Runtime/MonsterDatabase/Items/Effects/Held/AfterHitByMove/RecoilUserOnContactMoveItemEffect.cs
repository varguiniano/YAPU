using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that recoils the user when hit by a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/RecoilUserOnContactMoveItemEffect",
                     fileName = "RecoilUserOnContactMoveItemEffect")]
    public class RecoilUserOnContactMoveItemEffect : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Percentage to HP to recoil.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float RecoilPercentageOfMaxHP = 1f / 6f;

        /// <summary>
        /// Localization key for the recoil message.
        /// </summary>
        [SerializeField]
        private string RecoilMessage;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Move user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHitByMove(Item item,
                                                   DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            finished.Invoke(false);

            if (!move.DoesMoveMakeContact(user, battler, battleManager, ignoresAbilities)) yield break;

            item.ShowItemNotification(battler, localizer);

            yield return DialogManager.ShowDialogAndWait(RecoilMessage,
                                                         localizableModifiers: false,
                                                         modifiers: new[] {user.GetNameOrNickName(localizer)},
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                battler,
                                                                -(int) (user.GetStats(battleManager)[Stat.Hp]
                                                                      * RecoilPercentageOfMaxHP),
                                                                isSecondaryDamage: true);
        }
    }
}