using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle
{
    /// <summary>
    /// Item effect to escape a dungeon.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OutOfBattle/EscapeDungeon", fileName = "EscapeDungeon")]
    public class EscapeDungeon : UseOutOfBattleItemEffect
    {
        /// <summary>
        /// Use out of battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        PlayerSettings playerSettings,
                                        PlayerCharacter playerCharacter,
                                        ILocalizer localizer,
                                        Action<bool> finished)
        {
            if (!playerCharacter.Scene.CanEscapeRopeFromHere)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            yield return DialogManager.CloseMenus();

            yield return playerCharacter.EscapeDungeon();
        }
    }
}