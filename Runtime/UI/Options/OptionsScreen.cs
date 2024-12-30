using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Rendering;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Controller for the options screen.
    /// </summary>
    public class OptionsScreen : MenuSelector
    {
        /// <summary>
        /// Reference to the option description.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Reference to the scroll.
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        private ScrollRect Scroll;

        /// <summary>
        /// Reference to all scroll items, including titles.
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        private List<MenuItem> AllScrollItems;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the rendering manager.
        /// </summary>
        [Inject]
        private RenderingManager renderingManager;

        /// <summary>
        /// Show or hide the options screen.
        /// </summary>
        public void ShowOptions(bool show = true) => StartCoroutine(show ? ShowRoutine() : HideRoutine());

        /// <summary>
        /// Show the options screen.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            Show(false);

            string descriptionKey = ((OptionsMenuItem) MenuOptions[CurrentSelection]).SwitchOption(0);

            Description.SetValue(descriptionKey);

            renderingManager.ResolutionUpdated += UpdateSelectorArrowPositionAfterAFrame;

            yield return GetCachedComponent<CanvasGroup>().DOFade(1, .25f).WaitForCompletion();

            Show();

            inputManager.RequestInput(this);
        }

        /// <summary>
        /// Hide the options.
        /// </summary>
        protected IEnumerator HideRoutine()
        {
            inputManager.ReleaseInput(this);

            renderingManager.ResolutionUpdated -= UpdateSelectorArrowPositionAfterAFrame;

            Show();

            yield return GetCachedComponent<CanvasGroup>().DOFade(0, .25f).WaitForCompletion();

            Show(false);
        }

        /// <summary>
        /// Navigate left or right to switch options.
        /// </summary>
        public override void OnNavigation(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            float xMove = context.ReadValue<Vector2>().x;

            // BUG: This should be handled by the deadzone but it's not working for some reason and makes scrolling with a joystick without changing other options impossible.
            if (Mathf.Abs(xMove) < .3f) xMove = 0;

            if (Mathf.Abs(context.ReadValue<Vector2>().y) > .3f)
            {
                base.OnNavigation(context);

                Description.SetValue(((OptionsMenuItem) MenuOptions[CurrentSelection]).GetDescription());

                return;
            }

            if (xMove == 0) return;

            AudioManager.PlayAudio(SelectAudio);

            string descriptionKey = ((OptionsMenuItem) MenuOptions[CurrentSelection]).SwitchOption(xMove);

            Description.SetValue(descriptionKey);
        }

        /// <summary>
        /// Reorder stuff to first scroll and then update the arrow position.
        /// </summary>
        /// <param name="index">Index of the item to select.</param>
        /// <param name="playAudio">Play the navigation audio?</param>
        /// <param name="updateArrow">Should update the arrow when selecting?</param>
        /// <param name="force">Force reselection?</param>
        public override void Select(int index, bool playAudio = true, bool updateArrow = true, bool force = false)
        {
            base.Select(index, playAudio, false, force);

            UpdateScroll();

            UpdateSelectorArrowPosition();
        }

        /// <summary>
        /// Called when the player hits back.
        /// </summary>
        public override void OnBack(InputAction.CallbackContext context)
        {
            base.OnBack(context);

            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            ShowOptions(false);
        }

        /// <summary>
        /// Update the scroll position based on the current button.
        /// </summary>
        private void UpdateScroll()
        {
            if (CurrentSelection == 0)
            {
                Scroll.verticalNormalizedPosition = 1;
                return;
            }

            int currentGlobalIndex = AllScrollItems.IndexOf(MenuOptions[CurrentSelection]);

            Scroll.verticalNormalizedPosition = 1 - (float) currentGlobalIndex / (AllScrollItems.Count - 1);
        }

        /// <summary>
        /// Debug name for input.
        /// </summary>
        public override string GetDebugName() => "OptionsScreen";
    }
}