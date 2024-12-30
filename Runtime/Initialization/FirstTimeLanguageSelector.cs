using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Initialization
{
    /// <summary>
    /// Controller for the language selector used the first time the game starts.
    /// </summary>
    public class FirstTimeLanguageSelector : HidableUiElement<FirstTimeLanguageSelector>, IInputReceiver
    {
        /// <summary>
        /// Flag to use for each language.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<string, Sprite> Flags;

        /// <summary>
        /// Reference to the flag.
        /// </summary>
        [SerializeField]
        private Image Flag;

        /// <summary>
        /// Reference to the language name.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro LanguageName;

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
        /// Event called when the language is chosen.
        /// </summary>
        public Action OnLanguageChosen;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

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
        /// Total number of languages.
        /// </summary>
        private int totalLanguages;

        /// <summary>
        /// Update the language and request input.
        /// </summary>
        public override void Show(bool show = true)
        {
            if (show)
            {
                totalLanguages = localizer.GetAllLanguageIds().Count;

                localizer.SetLanguage(0);

                UpdateCurrentLanguage();
            }

            base.Show(show);

            if (show) inputManager.RequestInput(this);
        }

        /// <summary>
        /// Update the screen with the currently selected language.
        /// </summary>
        private void UpdateCurrentLanguage()
        {
            Flag.sprite = Flags[localizer.GetCurrentLanguage()];
            LanguageName.SetValue(localizer.GetCurrentLanguage());
        }

        /// <summary>
        /// Navigate the languages.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            float xValue = context.ReadValue<Vector2>().x;

            int newId = localizer.GetCurrentLanguageIndex();

            switch (xValue)
            {
                case < 0:
                    newId--;
                    break;
                case > 0:
                    newId++;
                    break;
            }

            if (newId == localizer.GetCurrentLanguageIndex()) return;

            audioManager.PlayAudio(NavigationAudio);

            if (newId < 0)
                newId = totalLanguages - 1;
            else if (newId >= totalLanguages) newId = 0;

            localizer.SetLanguage(newId);

            UpdateCurrentLanguage();
        }

        /// <summary>
        /// Select a language.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectionAudio);

            inputManager.ReleaseInput(this);
            OnLanguageChosen.Invoke();
        }

        /// <summary>
        /// Input type for this class.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

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