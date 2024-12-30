using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex screen for a single monster.
    /// </summary>
    public class SingleMonsterDexScreen : HidableUiElement<SingleMonsterDexScreen>, IPlayerDataReceiver, IInputReceiver
    {
        /// <summary>
        /// Reference to the panel with the main info.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private DexMainMonsterInfoPanel MainMonsterInfoPanel;

        /// <summary>
        /// Reference to the main info panel open position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform MainMonsterInfoPanelOpenPosition;

        /// <summary>
        /// Reference to the main info panel closed position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform MainMonsterInfoPanelClosedPosition;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [FoldoutGroup("References")]
        [SerializeField]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Sprite to show when the monster is not known.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Sprite EmptySprite;

        /// <summary>
        /// Reference to the right panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanel;

        /// <summary>
        /// Reference to the right panel open position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanelOpenPosition;

        /// <summary>
        /// Reference to the right panel closed position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanelClosedPosition;

        /// <summary>
        /// Reference to the tip to change forms.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement ChangeFormTip;

        /// <summary>
        /// Reference to the tip to show info.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement ShowInfoTip;

        /// <summary>
        /// Reference to all the dex tabs.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<MonsterDexTab> Tabs;

        /// <summary>
        /// Reference to the tab icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image TabIcon;

        /// <summary>
        /// Audio to play when navigating.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference NavigationAudio;

        /// <summary>
        /// Audio to play when selecting.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Event raised when closed stating the last selected index.
        /// </summary>
        public Action<int> OnClose;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; private set; }

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the player dex.
        /// </summary>
        [Inject]
        private MonsterDex.Dex dex;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Index of the currently displayed tab.
        /// </summary>
        private int currentTabIndex;

        /// <summary>
        /// Reference to the current tab.
        /// </summary>
        private MonsterDexTab CurrentTab => Tabs[currentTabIndex];

        /// <summary>
        /// Cached access to the main info panel transform.
        /// </summary>
        private Transform DexMainMonsterInfoPanelTransform
        {
            get
            {
                if (dexMainMonsterInfoPanelTransform == null)
                    dexMainMonsterInfoPanelTransform = MainMonsterInfoPanel.transform;

                return dexMainMonsterInfoPanelTransform;
            }
        }

        /// <summary>
        /// Backfield for DexMainMonsterInfoPanelTransform.
        /// </summary>
        private Transform dexMainMonsterInfoPanelTransform;

        /// <summary>
        /// Current monster entry.
        /// </summary>
        private MonsterDexEntry currentEntry;

        /// <summary>
        /// Current form entry.
        /// </summary>
        private FormDexEntry currentFormEntry;

        /// <summary>
        /// Current gender entry.
        /// </summary>
        private MonsterGender currentGender;

        /// <summary>
        /// Is the current gender caught?
        /// </summary>
        private bool IsCurrentGenderCaught =>
            (currentFormEntry.GendersCaught[currentGender]
          && currentEntry.Species[currentFormEntry.Form].HasGenderVariations)
         || (currentFormEntry.HasFormBeenCaught && !currentEntry.Species[currentFormEntry.Form].HasGenderVariations);

        /// <summary>
        /// Show the front or the back sprite?
        /// </summary>
        private bool showFrontSprite;

        /// <summary>
        /// Entries to use.
        /// </summary>
        private List<MonsterDexEntry> entries;

        /// <summary>
        /// Unfiltered original entries.
        /// </summary>
        private List<MonsterDexEntry> originalEntries;

        /// <summary>
        /// Personality to display monster sprites.
        /// </summary>
        private int personality;

        /// <summary>
        /// Show the screen for the given monster.
        /// </summary>
        /// <param name="monster">Monster to display.</param>
        /// <param name="form">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        /// <param name="entriesToUse">Entries to use when displaying.</param>
        /// <param name="monsterPersonality">Personality to display monster sprites.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        public void ShowScreen(MonsterEntry monster,
                               Form form,
                               MonsterGender gender,
                               List<MonsterDexEntry> entriesToUse,
                               int monsterPersonality,
                               PlayerCharacter playerCharacter)
        {
            if (!Shown) StartCoroutine(ShowScreenRoutine(monster, form, gender, entriesToUse, monsterPersonality, playerCharacter));
        }

        /// <summary>
        /// Show the screen for the given monster.
        /// </summary>
        /// <param name="monster">Monster to display.</param>
        /// <param name="form">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        /// <param name="entriesToUse">Entries to use when displaying.</param>
        /// <param name="monsterPersonality">Fake personality to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        private IEnumerator ShowScreenRoutine(MonsterEntry monster,
                                              Form form,
                                              MonsterGender gender,
                                              List<MonsterDexEntry> entriesToUse,
                                              int monsterPersonality,
                                              PlayerCharacter playerCharacter)
        {
            inputManager.BlockInput();
            DialogManager.ShowLoadingIcon();

            Show(false);

            PlayerCharacter = playerCharacter;

            personality = monsterPersonality;

            yield return dex.GetEntry(monster,
                                      entry =>
                                      {
                                          currentEntry = entry;
                                      });
            
            currentFormEntry = currentEntry.GetEntryForForm(form);
            
            originalEntries = entriesToUse;
            originalEntries ??= dex.GetAllEntries();

            // Ignore all that haven't been at least seen.
            entries = originalEntries.Where(entry => entry.HasMonsterBeenSeen).ToList();

            currentGender = gender;
            showFrontSprite = true;

            int firstTab = 0;

            while (!ShouldShowTab(firstTab)) firstTab++;

            SwitchTab(firstTab);

            DexMainMonsterInfoPanelTransform.position = MainMonsterInfoPanelClosedPosition.position;
            RightPanel.position = RightPanelClosedPosition.position;

            DexMainMonsterInfoPanelTransform.DOMove(MainMonsterInfoPanelOpenPosition.position, .25f)
                                            .SetEase(Ease.OutBack);

            RightPanel.DOMove(RightPanelOpenPosition.position, .25f).SetEase(Ease.OutBack);

            yield return GetCachedComponent<CanvasGroup>().DOFade(1, .25f).WaitForCompletion();

            Show();

            DialogManager.ShowLoadingIcon(false);
            inputManager.BlockInput(false);
            inputManager.RequestInput(this);
        }

        /// <summary>
        /// Hide the screen.
        /// </summary>
        private void HideScreen()
        {
            if (Shown) StartCoroutine(HideScreenRoutine());
        }

        /// <summary>
        /// Hide the screen.
        /// </summary>
        public IEnumerator HideScreenRoutine()
        {
            inputManager.ReleaseInput(this);
            inputManager.BlockInput();

            OnClose?.Invoke(originalEntries.IndexOf(currentEntry));

            UIMonsterSprite.GetCachedComponent<Image>().sprite = EmptySprite;
            UIMonsterSprite.GetCachedComponent<Image>().material = null;

            DexMainMonsterInfoPanelTransform.DOMove(MainMonsterInfoPanelClosedPosition.position, .25f)
                                            .SetEase(Ease.InBack);

            RightPanel.DOMove(RightPanelClosedPosition.position, .25f).SetEase(Ease.InBack);

            yield return GetCachedComponent<CanvasGroup>().DOFade(0, .25f).WaitForCompletion();

            Show(false);

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Update the current displayed data.
        /// </summary>
        private void UpdateDisplayedData()
        {
            if (!ShouldShowTab(currentTabIndex))
            {
                int newIndex = currentTabIndex;

                do
                {
                    newIndex++;

                    if (newIndex > Tabs.Count - 1) newIndex = 0;
                }
                while (!ShouldShowTab(newIndex));

                SwitchTab(newIndex);
            }

            MainMonsterInfoPanel.UpdateInfo(currentEntry, currentFormEntry, currentGender);

            UIMonsterSprite.SetMonster(currentEntry.Species,
                                       currentFormEntry.Form,
                                       currentGender,
                                       false,
                                       personality,
                                       showFrontSprite);

            ChangeFormTip.Show(currentEntry.NumberOfFormsSeen > 1);

            CurrentTab.SetData(currentEntry, currentFormEntry, currentGender, PlayerCharacter);
        }

        /// <summary>
        /// Switch monsters on up and down.
        /// Switch tabs on left an right.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            float xInput = context.ReadValue<Vector2>().x;

            if (xInput != 0)
            {
                int newTab = currentTabIndex;

                do
                    switch (xInput)
                    {
                        case > 0:
                            if (newTab == Tabs.Count - 1)
                                newTab = 0;
                            else
                                newTab++;

                            break;
                        case < 0:
                            if (newTab == 0)
                                newTab = Tabs.Count - 1;
                            else
                                newTab--;

                            break;
                    }
                while (!ShouldShowTab(newTab));

                audioManager.PlayAudio(NavigationAudio);

                SwitchTab(newTab);
            }

            float yInput = context.ReadValue<Vector2>().y;

            if (yInput == 0) return;

            // For this case, up means previous, down means next.
            yInput *= -1;

            int newSelection = entries.IndexOf(currentEntry);

            do
                switch (yInput)
                {
                    case > 0:
                        if (newSelection == entries.Count - 1)
                            newSelection = 0;
                        else
                            newSelection++;

                        break;
                    case < 0:
                        if (newSelection == 0)
                            newSelection = entries.Count - 1;
                        else
                            newSelection--;

                        break;
                }
            while (!entries[newSelection].HasMonsterBeenSeen);

            audioManager.PlayAudio(NavigationAudio);

            currentEntry = entries[newSelection];
            currentFormEntry = currentEntry.FormEntries.First(entry => entry.HasFormBeenSeen);
            currentGender = currentFormEntry.GendersSeen.First(pair => pair.Value).Key;

            UpdateDisplayedData();
        }

        /// <summary>
        /// Called when the user presses the select button.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            CurrentTab.OnSelectPressedOnParentScreen();
        }

        /// <summary>
        /// Cycle over the forms.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            DataByFormEntry formData = currentEntry.Species[currentFormEntry.Form];
            List<MonsterGender> gendersList = currentFormEntry.GendersSeen.Keys.ToList();
            int currentGenderIndex = gendersList.IndexOf(currentGender);

            // Next gender if there is.
            if (formData.HasMaleMaterialOverride
             && formData.HasBinaryGender)
            {
                int newGenderIndex = currentGenderIndex;

                do
                    newGenderIndex++;
                while (newGenderIndex < gendersList.Count
                    && !currentFormEntry.GendersSeen[gendersList[newGenderIndex]]
                    && newGenderIndex != currentGenderIndex);

                if (newGenderIndex != currentGenderIndex && newGenderIndex < gendersList.Count)
                {
                    currentGender = gendersList[newGenderIndex];
                    UpdateDisplayedData();
                    audioManager.PlayAudio(SelectAudio);
                    return;
                }
            }

            // Next form.
            int currentFormIndex = currentEntry.FormEntries.IndexOf(currentFormEntry);

            int newFormIndex = currentFormIndex;

            do
            {
                newFormIndex++;

                if (newFormIndex >= currentEntry.FormEntries.Count) newFormIndex = 0;
            }
            while (!currentEntry.FormEntries[newFormIndex].HasFormBeenSeen && newFormIndex != currentFormIndex);

            currentFormEntry = currentEntry.FormEntries[newFormIndex];
            currentGender = currentFormEntry.GendersSeen.First(pair => pair.Value).Key;
            UpdateDisplayedData();
            audioManager.PlayAudio(SelectAudio);
        }

        /// <summary>
        /// Switch between front and back sprite.
        /// </summary>
        /// <param name="context"></param>
        public void OnExtra2(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            showFrontSprite = !showFrontSprite;
            UpdateDisplayedData();
            audioManager.PlayAudio(SelectAudio);
        }

        /// <summary>
        /// Close the screen when going back.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            HideScreen();
        }

        /// <summary>
        /// Switch to the given tab.
        /// </summary>
        /// <param name="index">Index of the new tab.</param>
        private void SwitchTab(int index)
        {
            CurrentTab.Show(false);

            currentTabIndex = index;

            UpdateDisplayedData();

            CurrentTab.Show();

            TabIcon.sprite = CurrentTab.TitleSprite;

            ShowInfoTip.Show(CurrentTab.ShowInfoTip);
        }

        /// <summary>
        /// Should this new tab be seen?
        /// </summary>
        /// <param name="newIndex">New tab index.</param>
        /// <returns>True if it should.</returns>
        private bool ShouldShowTab(int newIndex) => Tabs[newIndex].ShowWhenNotCaught || IsCurrentGenderCaught;

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
        /// This is a UI element, so its input is UI.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Debug name to use by the input manager.
        /// </summary>
        /// <returns></returns>
        public string GetDebugName() => name;

        #region Unused input callbacks.

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