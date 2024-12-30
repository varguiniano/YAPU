using UnityEngine;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.Notifications
{
    /// <summary>
    /// Class that holds information to display an icon and text notification.
    /// </summary>
    public class IconTextNotificationData
    {
        /// <summary>
        /// Icon to use.
        /// </summary>
        public Sprite Icon;

        /// <summary>
        /// Text to use.
        /// </summary>
        public string Text;

        /// <summary>
        /// Is the text localizable?
        /// </summary>
        public bool LocalizableText = true;

        /// <summary>
        /// Are the modifiers localizable keys?
        /// Not used if the text is not localizable.
        /// </summary>
        public bool LocalizableModifiers = true;

        /// <summary>
        /// Modifiers to apply to the text.
        /// Not used if the text is not localizable.
        /// </summary>
        public string[] Modifiers = {""};

        /// <summary>
        /// Does this notification equal another?
        /// </summary>
        public bool Equals(IconTextNotificationData other) =>
            Icon == other.Icon
         && Text == other.Text
         && LocalizableText == other.LocalizableText
         && LocalizableModifiers == other.LocalizableModifiers
         && Modifiers == other.Modifiers;
    }
}