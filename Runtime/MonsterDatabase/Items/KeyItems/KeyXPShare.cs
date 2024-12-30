using UnityEngine;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.KeyItems
{
    /// <summary>
    /// Data class for the global XP share item.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Key/XPShare", fileName = "KeyXPShare")]
    public class KeyXPShare : OutsideOfBattleUsableKeyItem
    {
        /// <summary>
        /// Show the state of the global xp share on the description.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <returns>The full description.</returns>
        public override string GetDescription(ILocalizer localizer, PlayerSettings playerSettings) =>
            "["
          + localizer[playerSettings.AllTeamGainsXPOnFaint ? "Common/Enabled" : "Common/Disabled"]
          + "] "
          + base.GetDescription(localizer, playerSettings);
    }
}