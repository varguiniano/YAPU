using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.TextInput
{
    /// <summary>
    /// Controller for a dialog that requests the player to enter a text.
    /// </summary>
    public class TextInputDialog : HidableUiElement<TextInputDialog>, IInputReceiver
    {
        /// <summary>
        /// Event raised when the player finishes entering.
        /// </summary>
        public Action<bool, string> OnFinished;

        /// <summary>
        /// Reference to the input field.
        /// </summary>
        [SerializeField]
        private TMP_Text InputField;

        /// <summary>
        /// Text indicating the size of the text.
        /// </summary>
        [SerializeField]
        private TMP_Text SizeIndicator;

        /// <summary>
        /// Reference to the prompt text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Prompt;

        /// <summary>
        /// Reference to the prompt image.
        /// </summary>
        [SerializeField]
        private Image Image;

        /// <summary>
        /// Reference to the prompt to cancel.
        /// </summary>
        [SerializeField]
        private HidableUiElement CancelPrompt;

        /// <summary>
        /// Sound to play when submitting or canceling.
        /// </summary>
        [SerializeField]
        private AudioReference InteractionSound;

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
        /// Max size of the text to enter.
        /// </summary>
        private byte maxSize;

        /// <summary>
        /// Flag to know when the player has finished entering text.
        /// </summary>
        private bool isFinished;

        /// <summary>
        /// Can the dialog be canceled and exited?
        /// </summary>
        private bool canCancel;

        /// <summary>
        /// Is the player holding the backspace key?
        /// </summary>
        private bool holdingBackspace;

        /// <summary>
        /// Request a text to the player.
        /// </summary>
        /// <param name="textMaxSize">Max size of the text.</param>
        /// <param name="promptLocalizationKey">Localization key of the prompt to display to the player.</param>
        /// <param name="promptModifiers">Prompt text modifiers.</param>
        /// <param name="promptImage">Image to prompt to the player.</param>
        /// <param name="canPlayerCancel">Can the player can and exit?</param>
        /// <param name="defaultText">Default text already entered</param>
        public IEnumerator RequestText(byte textMaxSize,
                                       string promptLocalizationKey,
                                       string[] promptModifiers,
                                       Sprite promptImage,
                                       bool canPlayerCancel,
                                       string defaultText)
        {
            inputManager.BlockInput();

            yield return TransitionManager.BlackScreenFadeInRoutine();

            isFinished = false;

            canCancel = canPlayerCancel;

            CancelPrompt.Show(canCancel);

            maxSize = textMaxSize;

            Coroutine backspaceRoutine = StartCoroutine(BackspaceHold());

            InputField.SetText(defaultText);

            UpdateSize();

            Prompt.SetValue(promptLocalizationKey, false, promptModifiers);

            Image.sprite = promptImage;

            Show();

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            inputManager.RequestInput(this);

            if (Keyboard.current != null) Keyboard.current.onTextInput += OnCharEntered;

            inputManager.BlockInput(false);

            yield return new WaitUntil(() => isFinished);

            StopCoroutine(backspaceRoutine);

            if (Keyboard.current != null) Keyboard.current.onTextInput -= OnCharEntered;

            inputManager.ReleaseInput(this);

            inputManager.BlockInput();

            yield return TransitionManager.BlackScreenFadeInRoutine();

            Show(false);
        }

        /// <summary>
        /// Remove the last character on started and keep track of pressed.
        /// </summary>
        public void OnTextBackspace(InputAction.CallbackContext context)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    RemoveLastCharacter();
                    break;
                case InputActionPhase.Canceled:
                    holdingBackspace = false;
                    break;
                case InputActionPhase.Performed:
                    holdingBackspace = true;
                    break;
            }
        }

        /// <summary>
        /// Coroutine that removes the last character when the backspace is being held.
        /// </summary>
        private IEnumerator BackspaceHold()
        {
            while (!isFinished)
            {
                if (holdingBackspace) RemoveLastCharacter();

                yield return new WaitForSeconds(.05f);
            }
        }

        /// <summary>
        /// Remove the last character.
        /// </summary>
        private void RemoveLastCharacter()
        {
            if (InputField.text.Length > 0) InputField.SetText(InputField.text.Remove(InputField.text.Length - 1));

            UpdateSize();
        }

        /// <summary>
        /// Submit the text.
        /// </summary>
        public void OnTextSubmit(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (InputField.text.IsNullEmptyOrWhiteSpace()) return;

            audioManager.PlayAudio(InteractionSound);

            OnFinished?.Invoke(true, InputField.text);
            isFinished = true;
        }

        /// <summary>
        /// Cancel the text input.
        /// </summary>
        public void OnTextCancel(InputAction.CallbackContext context)
        {
            if (!canCancel || context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(InteractionSound);

            OnFinished?.Invoke(false, string.Empty);
            isFinished = true;
        }

        /// <summary>
        /// Write the character to the text if it's not backspace or escape and is not more than the max characters.
        /// </summary>
        /// <param name="c">Character entered.</param>
        private void OnCharEntered(char c)
        {
            if (c is '\b' or '\n' or '\t' or '\r' or '\u001B') return;

            string newText = InputField.text + c;

            if (newText.Length <= maxSize) InputField.SetText(InputField.text + c);

            UpdateSize();
        }

        /// <summary>
        /// Update the text that states the size.
        /// </summary>
        private void UpdateSize() => SizeIndicator.SetText(InputField.text.Length + "/" + maxSize);

        /// <summary>
        /// Input debug name.
        /// </summary>
        /// <returns></returns>
        public string GetDebugName() => name;

        /// <summary>
        /// This class uses text input.
        /// </summary>
        public InputType GetInputType() => InputType.TextInput;

        #region Unused input callbacks

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

        public void OnNavigation(InputAction.CallbackContext context)
        {
        }

        public void OnSelect(InputAction.CallbackContext context)
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

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        public void OnStateEnter()
        {
        }

        public void OnStateExit()
        {
        }

        #endregion
    }
}