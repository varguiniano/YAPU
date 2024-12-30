using UnityEngine;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.ChoiceMenu
{
    /// <summary>
    /// Class representing an option in the choice menu.
    /// </summary>
    public class ChoiceOption : MenuItem
    {
        /// <summary>
        /// Reference to this option text.
        /// </summary>
        [SerializeField]
        public LocalizedTextMeshPro Text;
    }
}