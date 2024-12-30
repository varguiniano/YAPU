using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Characters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.MainMenu
{
    /// <summary>
    /// Controller for the screen that allows to load a game.
    /// </summary>
    public class LoadGameScreen : HidableUiElement<LoadGameScreen>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the save date text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private TMP_Text SaveDate;

        /// <summary>
        /// Reference to the save location text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private LocalizedTextMeshPro SaveLocation;

        /// <summary>
        /// Reference to the character icon.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private CharacterWorldIcon CharacterIcon;

        /// <summary>
        /// Reference to the character name text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private TMP_Text CharacterName;

        /// <summary>
        /// Reference to the play time text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private TMP_Text Playtime;

        /// <summary>
        /// Reference to the badge number text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private TMP_Text BadgeNumber;

        /// <summary>
        /// Reference to the dex number text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private TMP_Text DexNumber;

        /// <summary>
        /// Reference to the money text.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private MoneyText MoneyText;

        /// <summary>
        /// Reference to the party members.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private List<MonsterIcon> Monsters;

        /// <summary>
        /// Reference to an empty sprite.
        /// </summary>
        [FoldoutGroup("Save info")]
        [SerializeField]
        private Sprite EmptySprite;

        /// <summary>
        /// Reference to the savegames menu.
        /// </summary>
        [SerializeField]
        private SavegamesMenu SavegamesMenu;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the player character data.
        /// </summary>
        [Inject]
        private CharacterData playerCharacter;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Flag to know if we are refreshing the preview.
        /// </summary>
        private bool refreshing;

        /// <summary>
        /// Next save to refresh.
        /// </summary>
        private string nextSaveToRefresh;

        /// <summary>
        /// Currently displayed save.
        /// </summary>
        private string currentlyDisplayedSave;

        /// <summary>
        /// Routine to show this screen.
        /// </summary>
        public IEnumerator ShowScreen()
        {
            inputManager.BlockInput();

            DialogManager.ShowLoadingIcon();

            Show(false);

            if (!SavegameManager.GetAllSavegames(out List<string> savegames))
            {
                Logger.Error("No savegames available.");
                yield break;
            }

            SavegamesMenu.SetButtons(savegames);

            SavegamesMenu.OnBackSelected += HideScreen;
            SavegamesMenu.OnButtonSelected += OnSaveSelected;
            SavegamesMenu.OnHovered += UpdateSaveData;

            yield return GetCachedComponent<CanvasGroup>().DOFade(1, .25f).WaitForCompletion();

            DialogManager.ShowLoadingIcon(false);

            Show();

            inputManager.BlockInput(false);

            SavegamesMenu.RequestInput();
        }

        /// <summary>
        /// Method to hide the screen.
        /// </summary>
        private void HideScreen()
        {
            ReleaseAllEvents();
            SavegamesMenu.ReleaseInput();

            GetCachedComponent<CanvasGroup>().DOFade(0, .25f).OnComplete(() => Show(false));
        }

        /// <summary>
        /// Called when a save is selected.
        /// </summary>
        private void OnSaveSelected(int saveIndex)
        {
            ReleaseAllEvents();
            SavegamesMenu.ReleaseInput();

            CoroutineRunner.RunRoutine(savegameManager.LoadGame(SavegamesMenu.Data[saveIndex]));
        }

        /// <summary>
        /// Update the data of the selected save.
        /// </summary>
        private void UpdateSaveData(int saveIndex)
        {
            nextSaveToRefresh = SavegamesMenu.Data[saveIndex];

            StartCoroutine(UpdateSaveDataRoutine(SavegamesMenu.Data[saveIndex]));
        }

        /// <summary>
        /// Update the data of the selected save.
        /// </summary>
        private IEnumerator UpdateSaveDataRoutine(string save)
        {
            yield return new WaitWhile(() => refreshing);

            if (save != nextSaveToRefresh || save == currentlyDisplayedSave) yield break;

            refreshing = true;

            yield return savegameManager.LoadBasicSaveInfo(save);

            SaveDate.SetText(globalGameData.LastSaveDate);
            SaveLocation.SetValue(globalGameData.LastPlayerLocation.Scene.LocalizableNameKey);
            CharacterIcon.SetIcon(playerCharacter);
            CharacterName.SetText(playerCharacter.LocalizableName);
            Playtime.SetText(TimeSpan.FromSeconds(timeManager.ElapsedGameSeconds).ToString(@"hh\h\ mm\m"));
            BadgeNumber.SetText(globalGameData.GetTotalNumberOfBadges().ToString());
            DexNumber.SetText(globalGameData.DexNumber.ToString());
            MoneyText.SetMoney(globalGameData.Money);

            for (int i = 0; i < globalGameData.RosterPreview.Count; i++)
            {
                GlobalGameData.MonsterPreview preview = globalGameData.RosterPreview[i];

                if (preview == null || preview.IsNull)
                    Monsters[i].GetCachedComponent<Image>().sprite = EmptySprite;
                else
                    Monsters[i].SetIcon(preview.Species, preview.Form, preview.Gender, preview.IsEgg);
            }

            currentlyDisplayedSave = save;

            refreshing = false;
        }

        /// <summary>
        /// Release all the menu events.
        /// </summary>
        private void ReleaseAllEvents()
        {
            SavegamesMenu.OnBackSelected -= HideScreen;
            SavegamesMenu.OnButtonSelected -= OnSaveSelected;
            SavegamesMenu.OnHovered -= UpdateSaveData;
        }
    }
}