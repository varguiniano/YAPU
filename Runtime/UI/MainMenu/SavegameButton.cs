using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.UI.MainMenu
{
    /// <summary>
    /// Behaviour for a button that represents a savegame.
    /// </summary>
    public class SavegameButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the button's text.
        /// </summary>
        [SerializeField]
        private TMP_Text Text;

        /// <summary>
        /// Set the name of the savegame.
        /// </summary>
        /// <param name="nameToSet">Name of the save.</param>
        public void SetSaveName(string nameToSet) => Text.SetText(nameToSet);

        /// <summary>
        /// Factory class for dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<SavegameButton>
        {
        }
    }
}