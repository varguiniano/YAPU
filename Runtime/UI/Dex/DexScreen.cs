using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex screen and the main dex menu.
    /// </summary>
    public class DexScreen :
        VirtualizedMenuSelector<(MonsterDexEntry, FormDexEntry, MonsterGender), DexMonsterButton,
            DexMonsterButton.Factory>,
        IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the hider for the monster list.
        /// </summary>
        [FoldoutGroup("Dex Screen")]
        [SerializeField]
        private HidableUiElement ListHider;

        /// <summary>
        /// Reference to the warning to show when the list is empty.
        /// </summary>
        [FoldoutGroup("Dex Screen")]
        [SerializeField]
        private HidableUiElement EmptyWarning;

        /// <summary>
        /// Reference to the transform of the monster list.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private Transform ListTransform;

        /// <summary>
        /// Open position of the monster list.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private Transform ListOpenPosition;

        /// <summary>
        /// Closed position of the monster list.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private Transform ListClosedPosition;

        /// <summary>
        /// Reference to the sprite that shows the hovered monster.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Sprite to show when the monster is not known.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private Sprite EmptySprite;

        /// <summary>
        /// Reference to the seen number.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private SeenNumber SeenNumber;

        /// <summary>
        /// Reference to the caught number.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private CaughtNumber CaughtNumber;

        /// <summary>
        /// Reference to the caught number.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Dex Screen")]
        private CaughtNumber AllFormsCaughtNumber;

        /// <summary>
        /// Reference to the sorting menu.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private DexSortingMenu SortingMenu;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; private set; }

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Flag to indicate buttons are being rebuilt.
        /// </summary>
        private bool rebuildingButtons;

        /// <summary>
        /// Fake personality to display monster sprites.
        /// </summary>
        private int fakePersonality;

        /// <summary>
        /// Open and show the dex screen.
        /// </summary>
        public void OpenDexScreen(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
            StartCoroutine(OpenRoutine());
        }

        /// <summary>
        /// Routine to show the dex screen.
        /// </summary>
        private IEnumerator OpenRoutine()
        {
            InputManager.BlockInput();

            DialogManager.ShowLoadingIcon();

            Show(false);

            ListHider.Show(false);
            EmptyWarning.Show(false);

            ListTransform.position = ListClosedPosition.position;
            
            fakePersonality = Random.Range(int.MinValue, int.MaxValue);

            RebuildButtons();

            yield return new WaitWhile(() => rebuildingButtons);

            SeenNumber.UpdateNumber();
            CaughtNumber.UpdateNumber();
            AllFormsCaughtNumber.UpdateNumber();

            OnBackSelected += CloseDexScreen;
            OnHovered += UpdateMonsterSprite;
            OnButtonSelected += OnMonsterSelected;

            SortingMenu.OnMenuClosed += RebuildButtons;

            GetCachedComponent<CanvasGroup>().DOFade(1, .25f);

            ListHider.Show();

            yield return ListTransform.DOMove(ListOpenPosition.position, .25f)
                                      .SetEase(Ease.OutBack)
                                      .WaitForCompletion();

            Show();

            DialogManager.ShowLoadingIcon(false);
            InputManager.BlockInput(false);
            RequestInput();
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        private void CloseDexScreen() => StartCoroutine(CloseRoutine());

        /// <summary>
        /// Close the dex screen.
        /// </summary>
        public IEnumerator CloseRoutine()
        {
            if (!Shown) yield break;

            ReleaseInput();
            InputManager.BlockInput();

            OnBackSelected -= CloseDexScreen;
            OnHovered -= UpdateMonsterSprite;
            OnButtonSelected -= OnMonsterSelected;

            SortingMenu.OnMenuClosed -= RebuildButtons;

            UIMonsterSprite.GetCachedComponent<Image>().sprite = EmptySprite;
            UIMonsterSprite.GetCachedComponent<Image>().material = null;

            GetCachedComponent<CanvasGroup>().DOFade(0, .25f);

            yield return ListTransform.DOMove(ListClosedPosition.position, .25f)
                                      .SetEase(Ease.InBack)
                                      .WaitForCompletion();

            ListHider.Show(false);
            Show(false);

            InputManager.BlockInput(false);
        }

        /// <summary>
        /// Open the dex screen when a monster is selected if it is known.
        /// </summary>
        /// <param name="index">Index of the selected monster.</param>
        private void OnMonsterSelected(int index)
        {
            (MonsterDexEntry monsterEntry, FormDexEntry formToUse, MonsterGender genderToUse) = Data[index];

            List<MonsterDexEntry> entriesToDisplay = new();

            foreach ((MonsterDexEntry entry, FormDexEntry _, MonsterGender _) in Data) entriesToDisplay.Add(entry);

            if (monsterEntry.HasMonsterBeenSeen)
                DialogManager.ShowSingleMonsterDexScreen(monsterEntry.Species,
                                                         formToUse.Form,
                                                         genderToUse,
                                                         fakePersonality,
                                                         PlayerCharacter,
                                                         entriesToDisplay,
                                                         onClose: lastIndex => Select(lastIndex, false));
        }

        /// <summary>
        /// Called when the player hits the extra 1 button, open the sorting menu.
        /// </summary>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            SortingMenu.Open();
        }

        /// <summary>
        /// Update the monster sprite displayed to the currently hovered index.
        /// </summary>
        private void UpdateMonsterSprite(int index)
        {
            (MonsterDexEntry monsterEntry, FormDexEntry firstKnownFormEntry, MonsterGender firstKnownGender) =
                Data[index];

            if (monsterEntry.HasMonsterBeenSeen)
                UIMonsterSprite.SetMonster(monsterEntry.Species,
                                           firstKnownFormEntry.Form,
                                           firstKnownGender,
                                           false,
                                           fakePersonality);
            else
            {
                UIMonsterSprite.GetCachedComponent<Image>().sprite = EmptySprite;
                UIMonsterSprite.GetCachedComponent<Image>().material = null;
            }
        }

        /// <summary>
        /// Fill a button with the data from a monster.
        /// </summary>
        /// <param name="child">Button to fill.</param>
        /// <param name="childData">Monster to fill with.</param>
        protected override void PopulateChildData(DexMonsterButton child,
                                                  (MonsterDexEntry, FormDexEntry, MonsterGender) childData) =>
            child.SetMonster(childData.Item1, childData.Item2, childData.Item3);

        /// <summary>
        /// Rebuild the displayed buttons.
        /// </summary>
        private void RebuildButtons() => StartCoroutine(RebuildButtonsRoutine());

        /// <summary>
        /// Rebuild the displayed buttons.
        /// </summary>
        private IEnumerator RebuildButtonsRoutine()
        {
            rebuildingButtons = true;

            InputManager.BlockInput();
            DialogManager.ShowLoadingIcon();

            yield return WaitAFrame;

            List<(MonsterDexEntry, FormDexEntry, MonsterGender)> reorderedEntries = null;

            yield return SortingMenu.FilterDex(database.GetMonsterEntries(),
                                               newEntries => reorderedEntries = newEntries);

            yield return WaitAFrame;

            SetButtons(reorderedEntries);

            yield return WaitAFrame;

            EmptyWarning.Show(Data.Count == 0);

            if (Data.Count == 0)
            {
                UIMonsterSprite.GetCachedComponent<Image>().sprite = EmptySprite;
                UIMonsterSprite.GetCachedComponent<Image>().material = null;
            }

            InputManager.BlockInput(false);
            DialogManager.ShowLoadingIcon(false);

            rebuildingButtons = false;
        }
    }
}