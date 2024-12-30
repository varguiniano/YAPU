using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Types
{
    /// <summary>
    /// Behaviour to control a badge that shows a monster type.
    /// </summary>
    public class TypeBadge : HidableUiElement<TypeBadge>
    {
        /// <summary>
        /// Reference to the background.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Background;

        /// <summary>
        /// Reference to the left section.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Left;

        /// <summary>
        /// Reference to the Icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Reference to the type name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Text;

        /// <summary>
        /// Set the type of this badge.
        /// </summary>
        /// <param name="monsterType">Type to set.</param>
        [Button("Set type")]
        public void SetType(MonsterType monsterType)
        {
            if (monsterType == null)
            {
                Show(false);
                return;
            }

            Background.color = monsterType.BackgroundColor;
            Left.color = monsterType.Color;
            Icon.sprite = monsterType.IconOverColor;

            // Make sure it works when debugging on editor.
            if (Application.isPlaying)
                Text.SetValue(monsterType.LocalizableName);
            else
            {
                Text.LocalizationKey = monsterType.LocalizableName;
                Text.Text.SetText(monsterType.LocalizableName);
            }
            
            Show();
        }
    }
}