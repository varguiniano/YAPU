using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime.TextPostProcessors;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Localization.PostProcessors
{
    /// <summary>
    /// Text post processor that replaces substrings from different values from the dex.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Localization/TextPostProcessors/InjectDexValues", fileName = "InjectDexValues")]
    public class InjectDexValues : LocalizedTextPostProcessor, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the Dex.
        /// </summary>
        [Inject]
        private Dex dex;
        
        /// <summary>
        /// Replace the substring.
        /// </summary>
        /// <param name="text">Text to post process.</param>
        /// <param name="extraParams">Extra parameters provided. May vary per application.</param>
        /// <returns>True if the text was successfully postprocessed.</returns>
        public override bool PostProcessText(ref string text, params object[] extraParams)
        {
            text = text.Replace("{Dex.CaughtSpecies}", dex.NumberCaughtInAtLeastOneForm.ToString());
            return true;
        }
    }
}