using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Abilities
{
    /// <summary>
    /// Panel to show an monster's ability.
    /// </summary>
    public class AbilityPanel : HidableUiElement<AbilityPanel>
    {
        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Name;

        /// <summary>
        /// Reference to the description text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Set the ability to display.
        /// </summary>
        /// <param name="ability">Ability to set.</param>
        [Button]
        public void SetAbility(Ability ability)
        {
            Name.SetValue(ability.LocalizableName);
            Description.SetValue(ability.LocalizableDescription);
        }
    }
}