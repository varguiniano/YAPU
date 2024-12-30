using TMPro;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs
{
    /// <summary>
    /// Struck representing a dialog text to display.
    /// </summary>
    public readonly struct DialogText
    {
        /// <summary>
        /// Localization key for that text.
        /// </summary>
        public readonly string LocalizationKey;

        /// <summary>
        /// Modifiers for the localization key.
        /// </summary>
        public readonly string[] Modifiers;

        /// <summary>
        /// Are the modifiers localizable?
        /// </summary>
        public readonly bool LocalizableModifiers;

        /// <summary>
        /// Character saying the dialog.
        /// </summary>
        public readonly CharacterData Character;

        /// <summary>
        /// Monster saying the dialog.
        /// </summary>
        public readonly MonsterInstance Monster;

        /// <summary>
        /// Typewriter speed.
        /// </summary>
        public readonly float TypewriterSpeed;

        /// <summary>
        /// Seconds to wait before switching to the next dialog.
        /// -1 means wait for input.
        /// </summary>
        public readonly float SwitchToNextAfterSeconds;

        /// <summary>
        /// Background for the dialog.
        /// </summary>
        public readonly DialogManager.BasicDialogBackground Background;

        /// <summary>
        /// Horizontal alignment for the dialog.
        /// </summary>
        public readonly HorizontalAlignmentOptions HorizontalAlignment;

        /// <summary>
        /// Base constructor for the dialog text.
        /// </summary>
        /// <param name="localizationKey">Localization key for the text.</param>
        /// <param name="character">Character saying the dialog.</param>
        /// <param name="monster">Monster saying the dialog.</param>
        /// <param name="typeWriterSpeed">Speed to play the typewriter at.</param>
        /// <param name="switchToNextAfterSeconds">Seconds to wait before switching to the next dialog. -1 means wait for input.</param>
        /// <param name="localizableModifiers">Are the modifiers localizable?</param>
        /// <param name="background">Background to use.</param>
        /// <param name="horizontalAlignment">Horizontal alignment for the text.</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        public DialogText(string localizationKey,
                          CharacterData character,
                          MonsterInstance monster,
                          float typeWriterSpeed,
                          float switchToNextAfterSeconds,
                          bool localizableModifiers,
                          DialogManager.BasicDialogBackground background,
                          HorizontalAlignmentOptions horizontalAlignment,
                          params string[] modifiers)
        {
            LocalizationKey = localizationKey;
            Modifiers = modifiers;
            LocalizableModifiers = localizableModifiers;
            Character = character;
            Monster = monster;
            TypewriterSpeed = typeWriterSpeed;
            SwitchToNextAfterSeconds = switchToNextAfterSeconds;
            Background = background;
            HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// Translate the message to a simple string for logging purposes.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>A localized string.</returns>
        public string ToString(ILocalizer localizer)
        {
            string text = localizer[LocalizationKey];

            if (Modifiers == null) return text;

            for (int i = 0; i < Modifiers.Length; i++)
            {
                string modifier = Modifiers[i];

                text = text.Replace("{" + i + "}", LocalizableModifiers ? localizer[modifier] : modifier);
            }

            return text;
        }
    }
}