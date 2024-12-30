using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using Varguiniano.YAPU.Runtime.UI.Monsters.Summary;
using Varguiniano.YAPU.Runtime.UI.Moves;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.MoveReplaceDialog
{
    /// <summary>
    /// Controller for the move replacing dialog.
    /// </summary>
    public class MoveReplaceDialogController : HidableUiElement<MoveReplaceDialogController>
    {
        /// <summary>
        /// Reference to the background.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Image Background;

        /// <summary>
        /// Reference to the open position of the left section.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionOpenPosition;

        /// <summary>
        /// Reference to the open position of the right section.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionClosedPosition;

        /// <summary>
        /// Reference to the left section.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSection;

        /// <summary>
        /// Reference to the move selector.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private MenuSelector MoveSelector;

        /// <summary>
        /// Reference to the move info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoveInfoPanel MoveInfoPanel;

        /// <summary>
        /// Reference to the monster info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement MonsterInfoPanel;

        /// <summary>
        /// Reference to the monster button.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MonsterButton MonsterButton;

        /// <summary>
        /// Reference to the type badges.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<TypeBadge> TypeBadges;

        /// <summary>
        /// Reference to the panel with stats.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SummaryStatsTab SummaryStatsTab;

        /// <summary>
        /// Duration of the open/close animation.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private float OpenCloseDuration = 0.25f;

        /// <summary>
        /// Reference to the audio to play when a move is learnt.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference MoveLearntAudio;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Bool to mark when a move has been chosen.
        /// </summary>
        private bool dialogFinished;

        /// <summary>
        /// Reference to the new move to learn.
        /// </summary>
        private Move newMoveToLearn;

        /// <summary>
        /// Reference to the monster instance.
        /// </summary>
        private MonsterInstance monsterInstance;

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
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Callback raised when the dialog is finished.
        /// </summary>
        private Action<bool> finishedCallback;

        /// <summary>
        /// Play the audio for the move learnt.
        /// </summary>
        public void PlayAudio() => audioManager.PlayAudio(MoveLearntAudio);

        /// <summary>
        /// Request the player to replace a move in a monster's move set.
        /// </summary>
        public IEnumerator RequestReplaceMove(MonsterInstance monster, Move newMove, Action<bool> finished)
        {
            dialogFinished = false;
            newMoveToLearn = newMove;
            monsterInstance = monster;
            finishedCallback = finished;

            inputManager.BlockInput();

            MoveSelector.Show(false);
            MoveInfoPanel.ShowOutOfBattle(monster);
            MonsterInfoPanel.Show(false);

            MoveSelector.OnButtonSelected = null;

            LeftSection.localPosition = LeftSectionClosedPosition.localPosition;

            SetMoves(PrepareMovesToShow(monsterInstance, newMoveToLearn), monster);

            MonsterButton.Panel.SetMonster(monsterInstance);

            (MonsterType firstType, MonsterType secondType) = monsterInstance.GetTypes(settings);

            TypeBadges[0].SetType(firstType);
            TypeBadges[1].SetType(secondType);

            SummaryStatsTab.SetData(monsterInstance, null);

            Show();

            Background.DOFade(1, OpenCloseDuration);

            dialogFinished = false;

            yield return LeftSection.DOLocalMove(LeftSectionOpenPosition.localPosition, OpenCloseDuration)
                                    .WaitForCompletion();

            MoveSelector.OnButtonSelected += OnMoveSelected;

            inputManager.BlockInput(false);

            MoveSelector.Show();
            MoveInfoPanel.ShowOutOfBattle(monster);
            MonsterInfoPanel.Show();

            yield return new WaitUntil(() => dialogFinished);
        }

        /// <summary>
        /// Called when a move is selected.
        /// </summary>
        /// <param name="index">Index of the selected move.</param>
        private void OnMoveSelected(int index) => StartCoroutine(OnMoveSelectedRoutine(index));

        /// <summary>
        /// Called when a move is selected.
        /// </summary>
        /// <param name="index">Index of the selected move.</param>
        private IEnumerator OnMoveSelectedRoutine(int index)
        {
            int option = -1;

            if (index == 4)
            {
                DialogManager.ShowChoiceMenu(new List<string>
                                             {
                                                 "Common/True",
                                                 "Common/False"
                                             },
                                             choice => option = choice,
                                             onBackCallback: () => option = 1,
                                             showDialog: true,
                                             localizationKey: "Dialogs/Moves/AskIfSureDontWantReplace",
                                             modifiers: newMoveToLearn.LocalizableName);

                yield return new WaitUntil(() => option != -1);

                if (option != 0)
                {
                    MoveSelector.OnStateEnter();
                    yield break;
                }

                yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/DidntLearn",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            monsterInstance
                                                                               .GetNameOrNickName(localizer),
                                                                            localizer[newMoveToLearn.LocalizableName]
                                                                        });

                finishedCallback.Invoke(false);

                Close();

                yield break;
            }

            Move oldMove = ((MoveButton) MoveSelector.MenuOptions[index]).Move.Move;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         choice => option = choice,
                                         onBackCallback: () => option = 1,
                                         showDialog: true,
                                         localizationKey: "Dialogs/Moves/AskIfSureWantReplace",
                                         modifiers: new[] {oldMove.LocalizableName, newMoveToLearn.LocalizableName});

            yield return new WaitUntil(() => option != -1);

            if (option != 0)
            {
                MoveSelector.OnStateEnter();
                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Forgot",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monsterInstance.GetNameOrNickName(localizer),
                                                                        localizer[oldMove.LocalizableName]
                                                                    });

            monsterInstance.CurrentMoves[index] = new MoveSlot(newMoveToLearn);

            PlayAudio();

            yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Learnt",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monsterInstance.GetNameOrNickName(localizer),
                                                                        localizer[newMoveToLearn.LocalizableName]
                                                                    });

            finishedCallback.Invoke(true);

            Close();
        }

        /// <summary>
        /// Close the UI.
        /// </summary>
        private void Close()
        {
            MoveSelector.OnButtonSelected -= OnMoveSelected;

            MoveSelector.Show(false);
            MoveInfoPanel.ShowOutOfBattle(show: false);
            MonsterInfoPanel.Show(false);

            LeftSection.DOLocalMove(LeftSectionClosedPosition.localPosition, OpenCloseDuration);

            Background.DOFade(0, OpenCloseDuration)
                      .OnComplete(() =>
                                  {
                                      Show(false);
                                      dialogFinished = true;
                                  });
        }

        /// <summary>
        /// Prepare the moves to be shown on the menu.
        /// </summary>
        /// <param name="monster">The monster to be shown.</param>
        /// <param name="newMove">The move it wants to learn.</param>
        /// <returns>An array of move slots to display.</returns>
        private static List<MoveSlot> PrepareMovesToShow(MonsterInstance monster, Move newMove)
        {
            List<MoveSlot> moves = monster.CloneMoveSlots().ToList();

            moves.Add(new MoveSlot(newMove));

            return moves;
        }

        /// <summary>
        /// Set the moves of the menu.
        /// </summary>
        /// <param name="moves">Moves to set.</param>
        /// <param name="owner">Owner of the moves.</param>
        private void SetMoves(IList<MoveSlot> moves, MonsterInstance owner)
        {
            List<bool> enabledButtons = new();

            for (int i = 0; i < moves.Count; ++i)
            {
                bool enableMove = moves[i].Move != null;

                if (enableMove)
                {
                    MoveSelector.MenuOptions[i].Show();
                    ((MoveButton) MoveSelector.MenuOptions[i]).SetMove(moves[i], owner, false, 0);
                }
                else
                    MoveSelector.MenuOptions[i].Hide();

                enabledButtons.Add(enableMove);
            }

            MoveSelector.UpdateLayout(enabledButtons);
        }
    }
}