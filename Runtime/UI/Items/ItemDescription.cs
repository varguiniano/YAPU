using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Items
{
    /// <summary>
    /// Controller for the description of an item in UI.
    /// </summary>
    public class ItemDescription : WhateverBehaviour<ItemDescription>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        private TMP_Text Name;

        /// <summary>
        /// Reference to the description text.
        /// </summary>
        [SerializeField]
        private TMP_Text Description;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [Inject]
        private PlayerSettings playerSettings;

        /// <summary>
        /// Set the name and description.
        /// </summary>
        /// <param name="item">Item to set.</param>
        public void SetItem(Item item)
        {
            Name.SetText(item == null ? "" : item.GetName(localizer));
            Description.SetText(item == null ? "" : item.GetDescription(localizer, playerSettings));
        }
    }
}