using System.Collections;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Quests
{
    /// <summary>
    /// Controller for the screen that displays the player's quests.
    /// </summary>
    public class QuestsScreen :
        VirtualizedMenuSelector<(Quest Quest, QuestStatus Status), QuestButton, QuestButton.Factory>,
        IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Background;

        /// <summary>
        /// Reference to the list panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ListPanel;

        /// <summary>
        /// Reference to the list panel hidden position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ListPanelHiddenPosition;

        /// <summary>
        /// Reference to the list panel shown position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ListPanelShownPosition;

        /// <summary>
        /// Reference to the list hider.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement ListHider;

        /// <summary>
        /// Reference to the game's completion percentage.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text CompletionPercentage;

        /// <summary>
        /// Reference to the quest description panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private QuestDescription QuestDescription;

        /// <summary>
        /// Duration of the open close animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float AnimationDuration = .25f;

        /// <summary>
        /// Reference to the quest manager.
        /// </summary>
        [Inject]
        private QuestManager questManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Show the quest screen.
        /// </summary>
        /// <param name="playerCharacterReference">Reference to the player character.</param>
        public void ShowQuests(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;
            StartCoroutine(ShowRoutine());
        }

        /// <summary>
        /// Routine to display the quests screen.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            InputManager.BlockInput();
            DialogManager.ShowLoadingIcon();

            Show(false);

            OnBackSelected += Hide;
            OnHovered += OnQuestHovered;

            ListHider.Show(false);
            QuestDescription.Show(false);

            ListPanel.localPosition = ListPanelHiddenPosition.localPosition;

            CompletionPercentage.SetText(questManager.VisibleGamePercentage
                                             ? (questManager.GameCompletionPercentage * 100).ToString("#00.##") + "%"
                                             : "");

            Show();

            Background.DOFade(1, AnimationDuration);

            SetButtons(questManager.GetAllQuests()
                                   .Select(pair => (pair.Key, pair.Value))
                                   .OrderBy(data => data.Value.IsCompleted)
                                   .ThenByDescending(data => data.Key.IsMainQuest)
                                   .ToList());

            yield return ListPanel.DOLocalMove(ListPanelShownPosition.localPosition, AnimationDuration)
                                  .SetEase(Ease.OutBack)
                                  .WaitForCompletion();

            ListHider.Show();

            DialogManager.ShowLoadingIcon(false);
            InputManager.BlockInput(false);
            InputManager.RequestInput(this);
        }

        /// <summary>
        /// Hide the quests screen.
        /// </summary>
        private void Hide() => StartCoroutine(HideRoutine());

        /// <summary>
        /// Hide the bag screen.
        /// </summary>
        private IEnumerator HideRoutine()
        {
            if (!Shown) yield break;

            InputManager.ReleaseInput(this);
            InputManager.BlockInput();

            OnBackSelected -= Hide;
            OnHovered -= OnQuestHovered;

            ListHider.Show(false);
            QuestDescription.Show(false);

            ListPanel.localPosition = ListPanelShownPosition.localPosition;

            Background.DOFade(0, AnimationDuration);

            yield return ListPanel.DOLocalMove(ListPanelHiddenPosition.localPosition, AnimationDuration)
                                  .SetEase(Ease.InBack)
                                  .WaitForCompletion();

            Show(false);
            InputManager.BlockInput(false);
        }

        /// <summary>
        /// Open the map if that quest's current objective has a map location.
        /// </summary>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            (Quest quest, QuestStatus status) = Data[CurrentSelection];

            if (status.IsCompleted) return;

            SceneInfoAsset location = quest.Objectives[status.CurrentObjective].MapLocation;

            if (location == null) return;

            AudioManager.PlayAudio(SelectAudio);

            mapSceneLauncher.ShowMap(playerCharacter, currentObjective: location);
        }

        /// <summary>
        /// Called when a quest is hovered.
        /// </summary>
        private void OnQuestHovered(int index)
        {
            QuestDescription.SetData(Data[index]);
            QuestDescription.Show();
        }

        /// <summary>
        /// Populate a button with its data.
        /// </summary>
        protected override void PopulateChildData(QuestButton child, (Quest Quest, QuestStatus Status) childData) =>
            child.SetQuest(childData.Quest, childData.Status);
    }
}