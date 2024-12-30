using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Interface that defines objects that can be passed as command parameters.
    /// </summary>
    public interface ICommandParameter
    {
        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer);
    }
}