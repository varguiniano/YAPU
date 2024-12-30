using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Profile
{
    /// <summary>
    /// Controller for the badges screen.
    /// </summary>
    public class BadgesScreen : HidableUiElement<BadgesScreen>
    {
        /// <summary>
        /// Reference to the title.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Title;

        /// <summary>
        /// Reference to the profile hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement ProfileHider;

        /// <summary>
        /// Reference to the badge description hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement BadgeDescriptionHider;

        /// <summary>
        /// Reference to the badge description.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro BadgeDescription;

        /// <summary>
        /// Badge image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image BadgeImage;

        /// <summary>
        /// Reference to the badge list for this region.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BadgeList BadgeList;

        /// <summary>
        /// Reference to the Version text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VersionText VersionText;

        /// <summary>
        /// Reference to the tips hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement TipsHider;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Cached list of the badges in the region.
        /// </summary>
        private List<Badge> badgesInRegion;

        /// <summary>
        /// Subscribe to UI events.
        /// </summary>
        private void OnEnable()
        {
            BadgeList.OnBackSelected += OnBack;
            BadgeList.OnHovered += OnBadgeHovered;
        }

        /// <summary>
        /// Unsubscribe from UI events.
        /// </summary>
        private void OnDisable()
        {
            BadgeList.OnBackSelected -= OnBack;
            BadgeList.OnHovered -= OnBadgeHovered;
        }

        /// <summary>
        /// Show the badges screen.
        /// </summary>
        /// <param name="playerCharacterReference">Reference to the player character.</param>
        public void ShowScreen(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;

            badgesInRegion = playerCharacter.GlobalGameData.GetBadges(playerCharacter.Region);

            BadgeList.SetButtons(badgesInRegion);

            Show();

            BadgeList.RequestInput();

            BadgeDescriptionHider.Show();
        }

        /// <summary>
        /// Called when a badge is hovered.
        /// </summary>
        /// <param name="index">Index of the hovered badge.</param>
        private void OnBadgeHovered(int index)
        {
            BadgeDescription.SetValue(badgesInRegion[index].LocalizableDescription);
            BadgeImage.sprite = badgesInRegion[index].Image;
        }

        /// <summary>
        /// Called when the player wants to go back.
        /// </summary>
        private void OnBack()
        {
            BadgeList.ReleaseInput();
            Title.SetValue("Menu/Profile");
            BadgeDescriptionHider.Show(false);
            ProfileHider.Show();
            VersionText.Show();
            TipsHider.Show();
            Show(false);
        }
    }
}