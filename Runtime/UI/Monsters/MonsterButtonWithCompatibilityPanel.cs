using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// A monster button that also has a panel to show if that monster is compatible with something.
    /// </summary>
    public class MonsterButtonWithCompatibilityPanel : MonsterButton
    {
        /// <summary>
        /// Is this monster button currently showing compatibility?
        /// </summary>
        public bool IsCompatible { get; private set; }

        /// <summary>
        /// Reference to the panel to show this monster is not compatible.
        /// </summary>
        [FoldoutGroup("Compatibility Panel")]
        [SerializeField]
        private HidableUiElement NotCompatiblePanel;

        /// <summary>
        /// Text to show this monster is not compatible.
        /// </summary>
        [FoldoutGroup("Compatibility Panel")]
        [SerializeField]
        private LocalizedTextMeshPro NotCompatibleText;

        /// <summary>
        /// Set if this monster is compatible.
        /// </summary>
        public void SetCompatibility(bool compatible) =>
            CoroutineRunner.RunRoutine(UpdateCompatibilityAfterPanel(compatible));

        /// <summary>
        /// Update the compatibility after the monster panel has finished updating.
        /// </summary>
        private IEnumerator UpdateCompatibilityAfterPanel(bool compatible)
        {
            // Give a couple of frames to the panel to start updating.
            yield return WaitAFrame;
            yield return WaitAFrame;

            yield return new WaitWhile(() => Panel.Updating);

            NotCompatiblePanel.Show(!compatible);
            IsCompatible = compatible;
        }

        /// <summary>
        /// Change the text to show on the compatibility panel.
        /// </summary>
        /// <param name="localizationKey">Localization key of the text to show.</param>
        public void SetCompatibilityText(string localizationKey) => NotCompatibleText.SetValue(localizationKey);
    }
}