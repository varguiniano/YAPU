using UnityEngine;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Text displaying the number of caught monsters in the dex.
    /// </summary>
    public class CaughtNumber : EasyUpdateText<CaughtNumber>, IPlayerDataReceiver
    {
        /// <summary>
        /// Include all forms and gender differences?
        /// </summary>
        [SerializeField]
        private bool IncludeAllFormsAndGenderDifferences;

        /// <summary>
        /// Reference to the dex.
        /// </summary>
        [Inject]
        private MonsterDex.Dex dex;

        /// <summary>
        /// Update the number in the dex.
        /// </summary>
        public void UpdateNumber() =>
            UpdateText(IncludeAllFormsAndGenderDifferences
                           ? dex.NumberCaughtIncludingFormsAndGenders.ToString("0000")
                           + " / "
                           + dex.NumberOfFormsIncludingGenderDifferences.ToString("0000")
                           : dex.NumberCaughtInAtLeastOneForm.ToString("0000")
                           + " / "
                           + dex.NumberOfMonsters.ToString("0000"));
    }
}