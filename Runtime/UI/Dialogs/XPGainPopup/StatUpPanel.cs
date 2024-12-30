using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.XPGainPopup
{
    /// <summary>
    /// Control panel for a stat that does up.
    /// </summary>
    public class StatUpPanel : WhateverBehaviour<StatUpPanel>
    {
        /// <summary>
        /// Reference to the stat name text.
        /// </summary>
        [SerializeField]
        private TMP_Text StatName;

        /// <summary>
        /// Reference to the previous value text.
        /// </summary>
        [SerializeField]
        private TMP_Text PreviousValue;

        /// <summary>
        /// Reference to the new value text.
        /// </summary>
        [SerializeField]
        private TMP_Text NewValue;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the values of the panel.
        /// </summary>
        /// <param name="stat">Stat to be displayed.</param>
        /// <param name="oldValue">Old value of the stat.</param>
        /// <param name="newValue">New value of the stat.</param>
        public void SetValues(Stat stat, uint oldValue, uint newValue)
        {
            StatName.SetText(localizer[stat.GetLocalizationString()] + ":");
            PreviousValue.SetText(oldValue.ToString());
            NewValue.SetText(newValue.ToString());
        }
    }
}