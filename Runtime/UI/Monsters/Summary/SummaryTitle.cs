using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Controller for the title of the summary screen.
    /// </summary>
    public class SummaryTitle : WhateverBehaviour<SummaryTitle>
    {
        /// <summary>
        /// Reference to the title.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Title;

        /// <summary>
        /// Reference to the icon.
        /// </summary>
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Set the title for the given tab.
        /// </summary>
        /// <param name="tab">Reference to the tab.</param>
        public void SetTabTitle(SummaryTab tab)
        {
            if (tab.UseSpriteInsteadOfText)
            {
                Title.Text.enabled = false;
                Icon.sprite = tab.TitleSprite;
                Icon.enabled = true;
            }
            else
            {
                Icon.enabled = false;
                Title.SetValue(tab.TitleLocalizableKey);
                Title.Text.enabled = true;
            }
        }
    }
}