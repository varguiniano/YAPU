using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Base class for simple value command parameters.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class CommandParameter<TValue> : ICommandParameter
    {
        /// <summary>
        /// Value of the parameter.
        /// </summary>
        protected readonly TValue Value;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="value">Value of the parameter.</param>
        protected CommandParameter(TValue value) => Value = value;

        /// <summary>
        /// Implicit conversion to bool.
        /// </summary>
        /// <param name="parameter">Parameter to convert.</param>
        /// <returns>Bool value.</returns>
        public static implicit operator TValue(CommandParameter<TValue> parameter) => parameter.Value;

        /// <summary>
        /// Implicit conversion to this class.
        /// </summary>
        /// <param name="value">Parameter to convert.</param>
        /// <returns>Bool value.</returns>
        public static implicit operator CommandParameter<TValue>(TValue value) => new(value);

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => Value.ToString();
    }
}