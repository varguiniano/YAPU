using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle
{
    /// <summary>
    /// Data class representing an item effect that heals the player roster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OutOfBattle/HealPlayerRoster", fileName = "HealPlayerRoster")]
    public class HealPlayerRoster : UseOutOfBattleItemEffect
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
            playerCharacter.PlayerRoster.CompletelyHeal();
            finished.Invoke(true);
            yield break;
        }
    }
}