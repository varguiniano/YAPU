using Febucci.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs
{
    /// <summary>
    /// Localized TMP that uses a typewriter to set the text.
    /// </summary>
    [RequireComponent(typeof(TypewriterByCharacter))]
    public class LocalizedTypeWriterTMP : LocalizedTextMeshPro
    {
        /// <summary>
        /// Is the typewriter currently animating?
        /// </summary>
        [FoldoutGroup(nameof(Typewriter))]
        [ReadOnly]
        public bool IsTypewriting;

        /// <summary>
        /// Ratio from the normal speed to the point speed.
        /// </summary>
        [FoldoutGroup(nameof(Typewriter))]
        public float NormalToPointSpeedMultiplier = 20;

        /// <summary>
        /// Ratio from the normal speed to the middle speed.
        /// </summary>
        [FoldoutGroup(nameof(Typewriter))]
        public float NormalToMiddleSpeedMultiplier = 5;

        /// <summary>
        /// Reference to the attached Typewriter.
        /// </summary>
        public TypewriterByCharacter Typewriter => GetCachedComponent<TypewriterByCharacter>();

        /// <summary>
        /// Subscribe to the typewriter events.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            Typewriter.onTypewriterStart.AddListener(() => IsTypewriting = true);
            Typewriter.onTextShowed.AddListener(() => IsTypewriting = false);
        }

        /// <summary>
        /// Unsubscribe to the typewriter events.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Typewriter.onTypewriterStart.RemoveAllListeners();
            Typewriter.onTextShowed.RemoveAllListeners();
        }

        /// <summary>
        /// Set the speed of the typewriter.
        /// </summary>
        /// <param name="newSpeed">Speed to set.</param>
        public void SetTypewriterSpeed(float newSpeed)
        {
            Typewriter.waitForNormalChars = newSpeed;
            Typewriter.waitLong = newSpeed * NormalToPointSpeedMultiplier;
            Typewriter.waitMiddle = newSpeed * NormalToMiddleSpeedMultiplier;
        }

        /// <summary>
        /// Set the text into the type writer instead of the TMP object.
        /// </summary>
        /// <param name="text">Text to set.</param>
        public override void UpdateText(string text) => Typewriter.ShowText(text);
    }
}