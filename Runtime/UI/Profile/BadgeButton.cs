using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Badges;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Profile
{
    /// <summary>
    /// Controller for the badge buttons.
    /// </summary>
    public class BadgeButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the badge icon.
        /// </summary>
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Reference to the badge name.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Name;

        /// <summary>
        /// Set the badge for this button.
        /// </summary>
        /// <param name="badge">Badge to set.</param>
        public void SetBadge(Badge badge)
        {
            Icon.sprite = badge.Image;
            Name.SetValue(badge.LocalizableName);
        }

        /// <summary>
        /// Factory class for dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<BadgeButton>
        {
        }
    }
}