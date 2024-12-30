using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Moves;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Controller of the summary tab showing moves.
    /// </summary>
    public class SummaryMovesTab : SummaryTab
    {
        /// <summary>
        /// Reference to the moves menu.
        /// </summary>
        [SerializeField]
        private MenuSelector MovesMenu;

        /// <summary>
        /// Reference to the move info panel.
        /// </summary>
        [SerializeField]
        private MoveInfoPanel MoveInfoPanel;

        /// <summary>
        /// Audio to play when entering the moves menu.
        /// </summary>
        [SerializeField]
        private AudioReference EnterMovesMenuAudio;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Flag to know if we are swapping a move.
        /// </summary>
        private bool swapping;

        /// <summary>
        /// Index to be swapped.
        /// </summary>
        private int indexToSwap;

        /// <summary>
        /// Monster being used.
        /// </summary>
        private MonsterInstance monsterInstance;

        /// <summary>
        /// Reference to the battle manager if we are in battle.
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// Set the moves for this monster.
        /// </summary>
        /// <param name="monster">Monster reference.</param>
        /// <param name="battleManagerReference">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManagerReference)
        {
            monsterInstance = monster;
            battleManager = battleManagerReference;

            for (int i = 0; i < MovesMenu.MenuOptions.Count; i++)
            {
                MenuItem button = MovesMenu.MenuOptions[i];

                ((MoveButton)button).SetMove(monsterInstance.CurrentMoves[i],
                                             monster,
                                             false); // TODO: Use battle manager reference?
            }
        }

        /// <summary>
        /// Make the moves menu request input.
        /// </summary>
        public override void EnterSubmenu()
        {
            base.EnterSubmenu();

            audioManager.PlayAudio(EnterMovesMenuAudio);

            MovesMenu.OnButtonSelected += index =>
                                          {
                                              if (swapping)
                                              {
                                                  MovesMenu.MenuOptions[indexToSwap].Button.interactable = true;

                                                  monsterInstance.ExchangeMoveSlots(indexToSwap, index);

                                                  SetData(monsterInstance, battleManager);

                                                  swapping = false;
                                              }
                                              else
                                              {
                                                  indexToSwap = index;
                                                  MovesMenu.MenuOptions[indexToSwap].Button.interactable = false;
                                                  swapping = true;
                                              }
                                          };

            MovesMenu.OnBackSelected += () =>
                                        {
                                            if (swapping)
                                            {
                                                MovesMenu.MenuOptions[indexToSwap].Button.interactable = true;

                                                swapping = false;
                                            }
                                            else
                                            {
                                                MovesMenu.OnBackSelected = null;
                                                MovesMenu.OnButtonSelected = null;

                                                MoveInfoPanel.ShowOutOfBattle(monsterInstance, false);

                                                MovesMenu.ReleaseInput();
                                            }
                                        };

            MoveInfoPanel.ShowOutOfBattle(monsterInstance);

            MovesMenu.RequestInput();
        }
    }
}