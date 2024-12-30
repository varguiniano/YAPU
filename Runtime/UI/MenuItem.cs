using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Class representing a part of a menu.
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class MenuItem : HidableAndSubscribableButton
    {
        /// <summary>
        /// Position in which to place the arrow selector when selected.
        /// </summary>
        [FoldoutGroup("References")]
        public Transform ArrowSelectorPosition;

        /// <summary>
        /// Ignore the layout when hiding?
        /// </summary>
        [SerializeField]
        private bool ToggleIgnoreLayout;

        /// <summary>
        /// Called on selected.
        /// </summary>
        public virtual void OnSelect()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Button.Select();
        }

        /// <summary>
        /// Called on deselected.
        /// </summary>
        public virtual void OnDeselect() => Button.OnDeselect(null);

        /// <summary>
        /// Show or hide the button.
        /// </summary>
        /// <param name="show"></param>
        public override void Show(bool show)
        {
            if (ToggleIgnoreLayout) GetCachedComponent<LayoutElement>().ignoreLayout = !show;

            base.Show(show);
        }
    }
}