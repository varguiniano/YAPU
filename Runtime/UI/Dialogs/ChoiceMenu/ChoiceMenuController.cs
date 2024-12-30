using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.ChoiceMenu
{
    /// <summary>
    /// Controller of a generic choice menu that can be used to display options for dialogs or popups.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ChoiceMenuController : MenuSelector
    {
        /// <summary>
        /// Size the menu should have depending on the options.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private SerializableDictionary<int, int> SizePerOptions;

        /// <summary>
        /// Reference to the default position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform DefaultPosition;

        /// <summary>
        /// Max number of options.
        /// </summary>
        private const int MaxOptions = 11;

        /// <summary>
        /// Reference to this object's transform.
        /// </summary>
        private Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Set the number of options for the menu.
        /// </summary>
        /// <param name="numberOfOptions">Number of options to set.</param>
        [Button]
        [FoldoutGroup("Debug")]
        public void SetNumberOfOptions([PropertyRange(1, MaxOptions)] int numberOfOptions = 2)
        {
            if (numberOfOptions is < 1 or > MaxOptions)
            {
                Logger.Error("Unsupported number of options.");
                return;
            }

            List<bool> optionsEnabled = new();

            for (int i = 0; i < MaxOptions; ++i) optionsEnabled.Add(i < numberOfOptions);

            UpdateLayout(optionsEnabled);

            GetCachedComponent<RectTransform>()
               .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizePerOptions[numberOfOptions]);
        }

        /// <summary>
        /// Move the menu to the given position.
        /// </summary>
        /// <param name="target">Target position. Null is default.</param>
        [Button]
        [FoldoutGroup("Debug")]
        public void SetPosition(Transform target = null) =>
            Transform.position = target == null ? DefaultPosition.position : target.position;
    }
}