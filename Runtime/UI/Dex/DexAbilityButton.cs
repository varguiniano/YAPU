using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Button that displays an ability in the dex.
    /// </summary>
    public class DexAbilityButton : MenuItem
    {
        /// <summary>
        /// Text displaying the name of the ability.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro AbilityName;

        /// <summary>
        /// Ability represented by this button.
        /// </summary>
        private Ability ability;

        /// <summary>
        /// Set the ability for this button.
        /// </summary>
        public void SetAbility(Ability newAbility)
        {
            ability = newAbility;
            AbilityName.SetValue(ability.LocalizableName);
        }

        /// <summary>
        /// Get the ability this button represents.
        /// </summary>
        public Ability GetAbility() => ability;
    }
}