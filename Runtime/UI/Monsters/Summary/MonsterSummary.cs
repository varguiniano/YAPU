using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Controller of the monster summary screen.
    /// </summary>
    public class MonsterSummary : HidableUiElement<MonsterSummary>, IInputReceiver, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the monster panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MonsterPanel MonsterPanel;

        /// <summary>
        /// Hidable element of the monster panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement HidableMonsterPanel;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [FoldoutGroup("References")]
        [SerializeField]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Hidable element of the monster sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement HidableMonsterSprite;

        /// <summary>
        /// Reference to the left panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform LeftPanel;

        /// <summary>
        /// Left panel hidden position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform LeftHiddenPosition;

        /// <summary>
        /// Left panel shown position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform LeftShownPosition;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Background;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image BackDetail;

        /// <summary>
        /// Reference of the arrows that mark that you can change monsters.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private GameObject[] ChangeMonsterArrows;

        /// <summary>
        /// Reference to the title controller.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SummaryTitle Title;

        /// <summary>
        /// Reference to the summary tabs.
        /// </summary>
        [FoldoutGroup("Tabs")]
        [SerializeField]
        private SummaryTab[] Tabs;

        /// <summary>
        /// List of tab indexes to display for normal monsters.
        /// </summary>
        [FoldoutGroup("Tabs")]
        [SerializeField]
        private List<int> NormalMonsterTabs;

        /// <summary>
        /// List of tab indexes to display for eggs.
        /// </summary>
        [FoldoutGroup("Tabs")]
        [SerializeField]
        private List<int> EggTabs;

        /// <summary>
        /// Duration of the open close animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float AnimationDuration = .25f;

        /// <summary>
        /// Audio to play on navigation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference OnNavigationAudio;

        /// <summary>
        /// Audio to play on back.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference OnBackAudio;

        /// <summary>
        /// Event raised when it is closed.
        /// Sends the last selected monster.
        /// </summary>
        public Action<int> OnClose;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; private set; }

        /// <summary>
        /// Roster being displayed.
        /// </summary>
        private List<MonsterInstance> monsters;

        /// <summary>
        /// Index of the currently displayed monster.
        /// </summary>
        private int currentlyDisplayedIndex;

        /// <summary>
        /// Index of the currently displayed tab.
        /// </summary>
        private int currentTabIndex;

        /// <summary>
        /// Current monster being displayed.
        /// </summary>
        private MonsterInstance CurrentMonster => monsters[currentlyDisplayedIndex];

        /// <summary>
        /// Reference to the current tab.
        /// </summary>
        private SummaryTab CurrentTab => Tabs[currentTabIndex];

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
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Flag to know if we are in battle.
        /// </summary>
        private bool inBattle;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private BattleManager battleManager;

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
        /// This is a UI receiver.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name used for input debugging.
        /// </summary>
        public string GetDebugName() => name;

        /// <summary>
        /// Show the summary.
        /// </summary>
        /// <param name="monsterInstances">Monsters to show.</param>
        /// <param name="firstIndex">Index to show.</param>
        /// <param name="battleManagerReference">Reference to the battle manager if we are in battle.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        public void Show(List<MonsterInstance> monsterInstances,
                         int firstIndex,
                         BattleManager battleManagerReference,
                         PlayerCharacter playerCharacter)
        {
            inputManager.BlockInput();

            PlayerCharacter = playerCharacter;

            battleManager = battleManagerReference;
            inBattle = battleManager != null;

            monsters = monsterInstances;
            currentlyDisplayedIndex = firstIndex;

            SwitchTab(0);

            UpdateInfo();

            LeftPanel.localPosition = LeftHiddenPosition.localPosition;

            Show();

            Background.DOFade(1, AnimationDuration);
            BackDetail.DOFade(0.11764705882f, AnimationDuration); // Set alpha to 30/255

            LeftPanel.DOLocalMove(LeftShownPosition.localPosition, AnimationDuration)
                     .SetEase(Ease.OutBack)
                     .OnComplete(() =>
                                 {
                                     HidableMonsterPanel.Show();
                                     HidableMonsterSprite.Show();

                                     inputManager.BlockInput(false);
                                     inputManager.RequestInput(this);
                                 });
        }

        /// <summary>
        /// Hide the summary.
        /// </summary>
        private void Close() => StartCoroutine(CloseRoutine());

        /// <summary>
        /// Hide the summary.
        /// </summary>
        public IEnumerator CloseRoutine()
        {
            inputManager.ReleaseInput(this);
            inputManager.BlockInput();

            LeftPanel.localPosition = LeftShownPosition.localPosition;
            HidableMonsterPanel.Show(false);
            HidableMonsterSprite.Show(false);

            Background.DOFade(0, AnimationDuration);
            BackDetail.DOFade(0, AnimationDuration);

            yield return LeftPanel.DOLocalMove(LeftHiddenPosition.localPosition, AnimationDuration)
                                  .SetEase(Ease.InBack)
                                  .WaitForCompletion();

            inputManager.BlockInput(false);
            Show(false);
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
                            if (newTab == Tabs.Length - 1)
                                newTab = 0;
                            else
                                newTab++;

                            break;
                        case < 0:
                            if (newTab == 0)
                                newTab = Tabs.Length - 1;
                            else
                                newTab--;

                            break;
                    }
                while (!DoesMonsterHaveTab(newTab));

                audioManager.PlayAudio(OnNavigationAudio);

                SwitchTab(newTab);
            }

            if (monsters.Count <= 1) return;

            float yInput = context.ReadValue<Vector2>().y;

            if (yInput == 0) return;

            // For this case, up means previous, down means next.
            yInput *= -1;

            bool validSelection = false;
            int newSelection = currentlyDisplayedIndex;

            while (!validSelection)
            {
                switch (yInput)
                {
                    case > 0:
                        if (newSelection == monsters.Count - 1)
                            newSelection = 0;
                        else
                            newSelection++;

                        break;
                    case < 0:
                        if (newSelection == 0)
                            newSelection = monsters.Count - 1;
                        else
                            newSelection--;

                        break;
                }

                if (monsters[newSelection] != null) validSelection = true;
            }

            audioManager.PlayAudio(OnNavigationAudio);

            Display(newSelection);
        }

        /// <summary>
        /// If the current tab has a submenu, enter it.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnSelect(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (CurrentTab.HasSubMenu) CurrentTab.EnterSubmenu();
        }

        /// <summary>
        /// Open the dex for this monster.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(OnBackAudio);

            if (globalGameData.HasDex)
                DialogManager.ShowSingleMonsterDexScreen(CurrentMonster.Species,
                                                         CurrentMonster.Form,
                                                         CurrentMonster.PhysicalData.Gender,
                                                         CurrentMonster.ExtraData.PersonalityValue,
                                                         PlayerCharacter);
            else
                DialogManager.ShowDialog("Dialogs/Dex/DontHave");
        }

        /// <summary>
        /// Close the summary.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(OnBackAudio);

            OnClose?.Invoke(currentlyDisplayedIndex);

            Close();
        }

        /// <summary>
        /// Should the given tab index be displayed for the current monster?
        /// </summary>
        /// <param name="tabIndex">Tab to check.</param>
        private bool DoesMonsterHaveTab(int tabIndex) => GetAvailableTabsForThisMonster().Contains(tabIndex);

        /// <summary>
        /// Get the tab indexes available for the currently displayed monster.
        /// </summary>
        /// <returns></returns>
        private List<int> GetAvailableTabsForThisMonster() =>
            CurrentMonster.EggData.IsEgg ? EggTabs : NormalMonsterTabs;

        /// <summary>
        /// Display the info for the given index.
        /// </summary>
        /// <param name="index">Index to display.</param>
        private void Display(int index)
        {
            if (index < 0 || index >= monsters.Count) return;

            if (monsters[index] == null) return;

            currentlyDisplayedIndex = index;

            UpdateInfo();
        }

        /// <summary>
        /// Switch to the given tab.
        /// </summary>
        /// <param name="index">Index of the new tab.</param>
        private void SwitchTab(int index)
        {
            CurrentTab.Show(false);

            currentTabIndex = index;

            CurrentTab.Show();

            Title.SetTabTitle(CurrentTab);
        }

        /// <summary>
        /// Update the displayed information.
        /// </summary>
        private void UpdateInfo()
        {
            if (!DoesMonsterHaveTab(currentTabIndex)) SwitchTab(GetAvailableTabsForThisMonster()[0]);

            if (!CurrentMonster.EggData.IsEgg) audioManager.PlayAudio(CurrentMonster.FormData.Cry);

            foreach (GameObject arrow in ChangeMonsterArrows) arrow.SetActive(monsters.Count > 1);

            MonsterPanel.SetMonster(CurrentMonster, false);
            UIMonsterSprite.SetMonster(CurrentMonster);

            foreach (SummaryTab tab in Tabs) tab.SetData(CurrentMonster, battleManager);
        }

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

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
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