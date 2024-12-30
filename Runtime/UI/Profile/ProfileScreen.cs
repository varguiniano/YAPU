using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Profile
{
    /// <summary>
    /// Main controller for the profile screen.
    /// </summary>
    public class ProfileScreen : HidableUiElement<ProfileScreen>, IInputReceiver, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the title.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Title;

        /// <summary>
        /// Reference to the character image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image CharacterImage;

        /// <summary>
        /// Reference to the player name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text PlayerName;

        /// <summary>
        /// Reference to the play time.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Playtime;

        /// <summary>
        /// Reference to the badge number.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text BadgeNumber;

        /// <summary>
        /// Reference to the dex number.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text DexNumber;

        /// <summary>
        /// Reference to the money number.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoneyText MoneyNumber;

        /// <summary>
        /// Reference to the text for the last save date.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text LastSaveDateText;

        /// <summary>
        /// Reference to the profile hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement ProfileHider;

        /// <summary>
        /// Reference to the tips hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement TipsHider;

        /// <summary>
        /// Reference to the Version text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VersionText VersionText;

        /// <summary>
        /// Reference to the image used to obscure the profile while saving.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Obscurer;

        /// <summary>
        /// Reference to the badges screen.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BadgesScreen BadgesScreen;

        /// <summary>
        /// Audio to use when saving or switching to badges.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Audio to use when going back.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference BackAudio;

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
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the player character data.
        /// </summary>
        [Inject]
        private CharacterData playerCharacterData;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Show the profile screen.
        /// </summary>
        public void ShowScreen(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;

            StartCoroutine(ShowRoutine());
        }

        /// <summary>
        /// Show the profile screen.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            if (Shown) yield break;

            Show(false);

            Title.SetValue("Menu/Profile");
            UpdateProfile();

            inputManager.BlockInput();

            yield return GetCachedComponent<CanvasGroup>().DOFade(1, .25f).WaitForCompletion();

            Show();

            ProfileHider.Show();
            VersionText.Show();

            inputManager.BlockInput(false);
            inputManager.RequestInput(this);
        }

        /// <summary>
        /// Hide the profile.
        /// </summary>
        private IEnumerator HideRoutine()
        {
            inputManager.BlockInput();

            Show();

            yield return GetCachedComponent<CanvasGroup>().DOFade(0, .25f).WaitForCompletion();

            Show(false);

            inputManager.BlockInput(false);

            inputManager.ReleaseInput(this);
        }

        /// <summary>
        /// This receiver is of type UI.
        /// </summary>
        /// <returns></returns>
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
        /// Save.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            StartCoroutine(TrySave());
        }

        /// <summary>
        /// Close.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(BackAudio);

            StartCoroutine(HideRoutine());
        }

        /// <summary>
        /// Show badges.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            if (playerCharacter.GlobalGameData.GetBadges(playerCharacter.Region).Count == 0)
            {
                DialogManager.ShowDialog("Badges/NoBadges");

                return;
            }

            Title.SetValue("Menu/Profile/Badges");
            ProfileHider.Show(false);
            VersionText.Show(false);
            TipsHider.Show(false);

            BadgesScreen.ShowScreen(playerCharacter);
        }

        /// <summary>
        /// Ask the player if they want to save and save if they do.
        /// </summary>
        private IEnumerator TrySave()
        {
            Obscurer.DOFade(.66f, .1f);

            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => choice = playerChoice,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "Menu/Save/Confirmation");

            yield return new WaitUntil(() => choice != -1);

            if (choice == 0)
            {
                yield return savegameManager.SaveGameWithCurrentDate(playerCharacter);
                UpdateProfile();
                yield return DialogManager.ShowDialogAndWait("Menu/Save/Finished");
            }

            Obscurer.DOFade(0, .1f);
        }

        /// <summary>
        /// Update the profile data.
        /// </summary>
        private void UpdateProfile()
        {
            CharacterImage.sprite = playerCharacterData.CharacterType.FrontSprite;
            PlayerName.SetText(playerCharacterData.LocalizableName);

            Playtime.SetText(TimeSpan.FromSeconds(timeManager.ElapsedGameSeconds).ToString(@"hh\h\ mm\m"));

            BadgeNumber.SetText(globalGameData.GetTotalNumberOfBadges().ToString());
            DexNumber.SetText("0"); // TODO: Dex number.
            MoneyNumber.SetMoney(playerBag);

            LastSaveDateText.SetText(globalGameData.LastSaveDate);
        }

        /// <summary>
        /// Debug name for input.
        /// </summary>
        public string GetDebugName() => "ProfileScreen";

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