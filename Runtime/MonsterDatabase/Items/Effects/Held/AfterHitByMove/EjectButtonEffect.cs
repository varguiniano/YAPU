using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that implements the eject button effect.
    /// This is another one of those very specific and full of exceptions mechanic introduced in Gen 5.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/EjectButtonEffect", fileName = "EjectButtonEffect")]
    public class EjectButtonEffect : AfterHitByMoveItemEffect
    {
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
            // ReSharper disable once InlineTemporaryVariable
            Battler buttonHolder = battler;

            // TODO: Sheer Force.
            // TODO: Knock off, thief, covet, magician.
            // TODO: Pickpocket.
            // TODO: Emergency Exit, Wimp Out.

            // Do not trigger if
            // The holder has fainted.
            // The holder had a substitute.
            // The target is no longer fighting.
            if (!buttonHolder.CanBattle
             || substituteTookHit
             || !battleManager.Battlers.GetBattlersFighting().Contains(buttonHolder))
                yield break;

            (BattlerType buttonHolderType, int buttonHolderIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(buttonHolder);

            // No extra effect if used against wilds.
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (buttonHolderType == BattlerType.Enemy && battleManager.EnemyType == EnemyType.Wild) yield break;

            // Used against trainers, force switch.
            // ReSharper disable once InvertIf
            if (battleManager.Rosters.GetRoster(buttonHolderType, buttonHolderIndex)
                             .Any(candidate => candidate != buttonHolder && candidate.CanBattle)
             && buttonHolder.CanSwitch(battleManager,
                                       buttonHolderType,
                                       buttonHolderIndex,
                                       null,
                                       false,
                                       item,
                                       true,
                                       true))
            {
                item.ShowItemNotification(battler, localizer);

                yield return battleManager.BattleManagerBattlerSwitch.ForceSwitchBattler(buttonHolderType,
                    buttonHolderIndex,
                    buttonHolderType,
                    buttonHolderIndex,
                    null,
                    item,
                    true,
                    false,
                    true);

                finished.Invoke(true);
            }
        }
    }
}