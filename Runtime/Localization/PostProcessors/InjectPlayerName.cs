using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime.TextPostProcessors;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Localization.PostProcessors
{
    /// <summary>
    /// Text post processor that replaces a substring with the player's name.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Localization/TextPostProcessors/InjectPlayerName", fileName = "InjectPlayerName")]
    public class InjectPlayerName : LocalizedTextPostProcessor, IPlayerDataReceiver
    {
        /// <summary>
        /// Original substring to replace.
        /// </summary>
        [SerializeField]
        private string OriginalSubstring = "{PlayerName}";

        /// <summary>
        /// Reference to the player character data.
        /// </summary>
        [Inject]
        private CharacterData playerCharacter;

        /// <summary>
        /// Replace the substring.
        /// </summary>
        /// <param name="text">Text to post process.</param>
        /// <param name="extraParams">Extra parameters provided. May vary per application.</param>
        /// <returns>True if the text was successfully postprocessed.</returns>
        public override bool PostProcessText(ref string text, params object[] extraParams)
        {
            text = text.Replace(OriginalSubstring, playerCharacter.LocalizableName);
            return true;
        }
    }
}