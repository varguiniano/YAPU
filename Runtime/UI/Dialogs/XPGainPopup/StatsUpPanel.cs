using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.XPGainPopup
{
    /// <summary>
    /// Panel to be displayed when stats go up.
    /// </summary>
    public class StatsUpPanel : HidableUiElement<StatsUpPanel>
    {
        /// <summary>
        /// Reference to the individual panels.
        /// </summary>
        [SerializeField]
        private StatUpPanel[] Panels;

        /// <summary>
        /// Set the values of the panels.
        /// </summary>
        /// <param name="previousValues">Previous values of those stats.</param>
        /// <param name="newValues">New values of those stats.</param>
        public void SetValues(uint[] previousValues, uint[] newValues)
        {
            foreach (Stat stat in Utils.GetAllItems<Stat>())
                Panels[(int)stat].SetValues(stat, previousValues[(int)stat], newValues[(int)stat]);
        }
    }
}