using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.MainMenu
{
    /// <summary>
    /// Controller for the screen that shows the credits.
    /// </summary>
    public class CreditsScreen : HidableUiElement<LoadGameScreen>, IInputReceiver
    {
        /// <summary>
        /// Reference to the navigation audio.
        /// </summary>
        [SerializeField]
        private AudioReference NavigationAudio;

        /// <summary>
        /// Reference to the selection audio.
        /// </summary>
        [SerializeField]
        private AudioReference SelectionAudio;

        /// <summary>
        /// Reference to the displayed text.
        /// </summary>
        [SerializeField]
        private TMP_Text DisplayedText;

        /// <summary>
        /// Reference to the credits text.
        /// </summary>
        [SerializeField]
        private TextAsset CreditsText;

        /// <summary>
        /// Reference to the credits scroll.
        /// </summary>
        [SerializeField]
        private ScrollRect ScrollRect;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Value of the last input.
        /// </summary>
        private float lastInput;

        /// <summary>
        /// Is the player holding the input?
        /// </summary>
        private bool holding;

        /// <summary>
        /// Start the navigation hold coroutine.
        /// </summary>
        private void OnEnable() => StartCoroutine(NavigationHold());

        /// <summary>
        /// Request input on show.
        /// </summary>
        public override void Show(bool show = true)
        {
            if (show) inputManager.RequestInput(this);

            DisplayedText.SetText(CreditsText.text);

            base.Show(show);
        }

        /// <summary>
        /// Scroll the credits.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case {phase: InputActionPhase.Started}:
                    lastInput = context.ReadValue<Vector2>().y;

                    Navigate(lastInput);
                    break;
                case {phase: InputActionPhase.Performed}:
                    holding = true;
                    break;
                case {phase: InputActionPhase.Canceled}:
                    holding = false;
                    break;
            }
        }

        /// <summary>
        /// Navigate if holding.
        /// </summary>
        private IEnumerator NavigationHold()
        {
            WaitForSeconds interval = new(0.05f);

            while (true)
            {
                if (holding) Navigate(lastInput);
                yield return interval;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Navigate through the scroll rect.
        /// </summary>
        /// <param name="input"></param>
        private void Navigate(float input)
        {
            if (Mathf.Abs(input) <= 0.001f) return;

            audioManager.PlayAudio(NavigationAudio);

            ScrollRect.verticalScrollbar.value += input * 0.01f;
        }

        /// <summary>
        /// Close this menu.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectionAudio);

            inputManager.ReleaseInput(this);

            Show(false);
        }

        /// <summary>
        /// Input type for this class.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Input debugging name.
        /// </summary>
        public string GetDebugName() => name;

        /// <summary>
        /// Called when it starts receiving input.
        /// </summary>
        public void OnStateEnter()
        {
        }

        /// <summary>
        /// Called when it stops receiving input.
        /// </summary>
        public void OnStateExit()
        {
        }

        #region Unused input calls.

        public void OnSelect(InputAction.CallbackContext context)
        {
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
        {
        }

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public void OnExtra1(InputAction.CallbackContext context)
        {
        }

        public void OnExtra2(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTextBackspace(InputAction.CallbackContext context)
        {
        }

        public void OnTextSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnTextCancel(InputAction.CallbackContext context)
        {
        }

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}