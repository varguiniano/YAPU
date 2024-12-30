using System;
using Varguiniano.YAPU.Runtime.Configuration;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    /// <summary>
    /// Helper class for money operations.
    /// </summary>
    public class MoneyHelper : Loggable<MoneyHelper>
    {
        /// <summary>
        /// Build a string with a money symbol.
        /// </summary>
        /// <param name="amount">Amount of money.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The generated string.</returns>
        public static string BuildMoneyString(uint amount, YAPUSettings settings, ILocalizer localizer)
        {
            YAPUSettings.CurrencySymbolPosition position;

            try
            {
                position = settings.CurrencySymbolPositions[localizer.GetCurrentLanguage()];
            }
            catch
            {
                position = settings.DefaultCurrencySymbolPosition;
            }

            return position switch
            {
                YAPUSettings.CurrencySymbolPosition.Left => localizer["Currency"] + amount,
                YAPUSettings.CurrencySymbolPosition.Right => amount + localizer["Currency"],
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}