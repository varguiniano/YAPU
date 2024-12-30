using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Initialization
{
    /// <summary>
    /// Controller for the screen that shows the controls.
    /// </summary>
    public class ControlsView : HidableUiElement<ControlsView>, IInputReceiver
    {
        /// <summary>
        /// Action called when the player continues.
        /// </summary>
        public Action PlayerContinued;

        /// <summary>
        /// Reference to the selection audio.
        /// </summary>
        [SerializeField]
        private AudioReference SelectionAudio;

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
        /// Input type for this class.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Request input on show.
        /// </summary>
        public override void Show(bool show = true)
        {
            if (show) inputManager.RequestInput(this);

            base.Show(show);
        }

        /// <summary>
        /// Select a language.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectionAudio);

            inputManager.ReleaseInput(this);
            PlayerContinued.Invoke();
        }

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

        /// <summary>
        /// Input debugging name.
        /// </summary>
        public string GetDebugName() => name;

        #region Unused input calls.

        public void OnNavigation(InputAction.CallbackContext context)
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

        public void OnBack(InputAction.CallbackContext context)
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