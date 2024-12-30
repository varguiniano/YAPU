using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Text displaying the number of caught monsters in the dex.
    /// </summary>
    public class SeenNumber : EasyUpdateText<CaughtNumber>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the dex.
        /// </summary>
        [Inject]
        private MonsterDex.Dex dex;

        /// <summary>
        /// Update the number in the dex.
        /// </summary>
        public void UpdateNumber() =>
            UpdateText(dex.NumberSeenInAtLeastOneForm.ToString("0000") + " / " + dex.NumberOfMonsters.ToString("0000"));
    }
}