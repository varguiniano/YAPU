using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using Varguiniano.YAPU.Runtime.UI.Moves;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.MoveTutor
{
    /// <summary>
    /// Controller for the dialog that allows to teach a list of moves to a monster.
    /// </summary>
    public class MoveTutorDialog : HidableUiElement<MoveTutorDialog>
    {
        /// <summary>
        /// Reference to the move selection menu.
        /// </summary>
        [SerializeField]
        private MoveSelectionMenu MoveSelectionMenu;

        /// <summary>
        /// Reference to the move selection menu transform.
        /// </summary>
        [SerializeField]
        private Transform MoveSelectionMenuTransform;

        /// <summary>
        /// Reference to the move selection menu open position.
        /// </summary>
        [SerializeField]
        private Transform MoveSelectionMenuOpenPosition;

        /// <summary>
        /// Reference to the move selection menu closed position.
        /// </summary>
        [SerializeField]
        private Transform MoveSelectionMenuClosedPosition;

        /// <summary>
        /// Move info panel.
        /// </summary>
        [SerializeField]
        private MoveInfoPanel MoveInfoPanel;

        /// <summary>
        /// Reference to the mini summary.
        /// </summary>
        [SerializeField]
        private MiniMonsterSummary Summary;

        /// <summary>
        /// Reference to the summary transform.
        /// </summary>
        [SerializeField]
        private Transform SummaryTransform;

        /// <summary>
        /// Reference to the Summary open position.
        /// </summary>
        [SerializeField]
        private Transform SummaryOpenPosition;

        /// <summary>
        /// Reference to theSummary closed position.
        /// </summary>
        [SerializeField]
        private Transform SummaryClosedPosition;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Flag to know when the player has made a choice.
        /// </summary>
        private bool choiceMade;

        /// <summary>
        /// Show the move tutor dialog.
        /// </summary>
        /// <param name="monster">Monster that might learn.</param>
        /// <param name="candidates">Move candidates.</param>
        public IEnumerator ShowDialog(MonsterInstance monster, List<Move> candidates)
        {
            MoveSelectionMenu.Show(false);
            Show(false);

            Vector3 menuClosedPosition = MoveSelectionMenuClosedPosition.localPosition;
            MoveSelectionMenuTransform.localPosition = menuClosedPosition;

            Vector3 summaryClosedPosition = SummaryClosedPosition.localPosition;
            SummaryTransform.localPosition = summaryClosedPosition;

            Summary.SetMonster(monster);
            MoveSelectionMenu.SetButtons(candidates);

            choiceMade = false;

            MoveSelectionMenu.OnBackSelected += () =>
                                                {
                                                    MoveSelectionMenu.OnBackSelected = null;
                                                    choiceMade = true;
                                                };

            MoveSelectionMenu.OnButtonSelected += index =>
                                                  {
                                                      MoveSelectionMenu.OnButtonSelected = null;

                                                      StartCoroutine(LearnMove(monster, MoveSelectionMenu.Data[index]));
                                                  };

            Show();
            MoveSelectionMenu.Show();
            MoveInfoPanel.ShowOutOfBattle(monster);

            SummaryTransform.DOLocalMove(SummaryOpenPosition.localPosition, .25f).SetEase(Ease.OutBack);

            yield return MoveSelectionMenuTransform.DOLocalMove(MoveSelectionMenuOpenPosition.localPosition, .25f)
                                                   .SetEase(Ease.OutBack)
                                                   .WaitForCompletion();

            yield return new WaitUntil(() => choiceMade);

            SummaryTransform.DOLocalMove(summaryClosedPosition, .25f).SetEase(Ease.InBack);
            MoveInfoPanel.ShowOutOfBattle(monster);

            yield return MoveSelectionMenuTransform.DOLocalMove(menuClosedPosition, .25f)
                                                   .SetEase(Ease.InBack)
                                                   .WaitForCompletion();

            Show(false);
            MoveSelectionMenu.Show(false);
        }

        /// <summary>
        /// Open the learn move panel.
        /// </summary>
        /// <param name="monster">Monster that will learn.</param>
        /// <param name="move">Move to be learnt.</param>
        private IEnumerator LearnMove(MonsterInstance monster, Move move)
        {
            yield return DialogManager.ShowMoveLearnPanel(monster,
                                                          move,
                                                          localizer,
                                                          _ =>
                                                          {
                                                          });

            choiceMade = true;
        }
    }
}