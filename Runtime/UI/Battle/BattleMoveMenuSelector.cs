using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Moves;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Class representing the Move menu selector in battle.
    /// </summary>
    public class BattleMoveMenuSelector : MenuSelector
    {
        /// <summary>
        /// Reference to the main menu.
        /// </summary>
        [SerializeField]
        private MenuSelector MainMenu;

        /// <summary>
        /// Reference to the move info panel.
        /// </summary>
        [SerializeField]
        private MoveInfoPanel MoveInfoPanel;

        /// <summary>
        /// Reference to the button to enable megaevolution.
        /// </summary>
        [SerializeField]
        private HidableUiElement MegaTip;

        /// <summary>
        /// Open position for the mega tip.
        /// </summary>
        [SerializeField]
        private Transform MegaTipOpenPosition;

        /// <summary>
        /// Closed position for the mega tip.
        /// </summary>
        [SerializeField]
        private Transform MegaTipClosedPosition;

        /// <summary>
        /// Reference to the mega tip transform.
        /// </summary>
        private Transform MegaTipTransform
        {
            get
            {
                if (megaTipTransform == null) megaTipTransform = MegaTip.transform;
                return megaTipTransform;
            }
        }

        /// <summary>
        /// Backfield for MegaTipTransform.
        /// </summary>
        private Transform megaTipTransform;

        /// <summary>
        /// Flag to tell the menu if it should listen to the main menu.
        /// Useful for hiding the moves menu when a monster is force to repeat the same move or to struggle.
        /// </summary>
        [ReadOnly]
        public bool ListenToMainMenu;

        /// <summary>
        /// Flag to tell the menu if the monster can megaevolve.
        /// </summary>
        [ReadOnly]
        public bool CanMegaevolve;

        /// <summary>
        /// Should the monster megaevolve?
        /// </summary>
        [ReadOnly]
        public bool ShouldMegaevolve
        {
            get => shouldMegaevolve;
            private set
            {
                shouldMegaevolve = value;

                MegaTipTransform.DOKill();

                if (shouldMegaevolve)
                    MegaTipTransform.DOMove(MegaTipOpenPosition.position, .25f).SetEase(Ease.OutBack);
                else
                    MegaTipTransform.DOMove(MegaTipClosedPosition.position, .25f).SetEase(Ease.InBack);
            }
        }

        /// <summary>
        /// Backfield for ShouldMegaevolve.
        /// </summary>
        private bool shouldMegaevolve;

        /// <summary>
        /// Owner of the moves.
        /// </summary>
        private Battler movesOwner;

        /// <summary>
        /// Potential target of the moves.
        /// </summary>
        private Battler movesTarget;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// Subscribe to the main menu selecting moves.
        /// </summary>
        private void OnEnable() => MainMenu.OnButtonSelected += OnMainMenuSelected;

        /// <summary>
        /// Unsubscribe.
        /// </summary>
        private void OnDisable() => MainMenu.OnButtonSelected -= OnMainMenuSelected;

        /// <summary>
        /// Called when the main menu selects something.
        /// </summary>
        /// <param name="index">Index of the selected item.</param>
        private void OnMainMenuSelected(int index)
        {
            if (!ListenToMainMenu) return;
            if (index == 0) Show();
        }

        /// <summary>
        /// Ignore disabled moves.
        /// </summary>
        /// <param name="context"></param>
        public override void OnSelect(InputAction.CallbackContext context)
        {
            if (((MoveButton)MenuOptions[CurrentSelection]).IsDisabled)
            {
                AudioManager.PlayAudio(SelectAudio);
                return;
            }

            MoveInfoPanel.ShowInBattle(false, movesOwner, movesTarget, battleManager);

            base.OnSelect(context);
        }

        /// <summary>
        /// Go back to the main menu.
        /// </summary>
        /// <param name="context"></param>
        public override void OnBack(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            base.OnBack(context);

            MoveInfoPanel.ShowInBattle(false, movesOwner, movesTarget, battleManager);

            Show(false);
            MainMenu.Show();
        }

        /// <summary>
        /// Set the moves of the menu.
        /// </summary>
        /// <param name="moves">Moves to set.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="useEffectiveness">Show moves effectiveness?</param>
        /// <param name="effectiveness">Effectiveness of the move.</param>
        /// <param name="target">Potential target of the move.</param>
        /// <param name="battleManagerReference">Reference to the battle manager.</param>
        public void SetMoves(MoveSlot[] moves,
                             Battler owner,
                             List<bool> useEffectiveness,
                             List<float> effectiveness,
                             Battler target,
                             BattleManager battleManagerReference)
        {
            movesOwner = owner;
            movesTarget = target;
            battleManager = battleManagerReference;

            List<MoveSlot> usableMoves = movesOwner.GetUsableMoves(battleManager);

            List<bool> enabledButtons = new();

            for (int i = 0; i < moves.Length; ++i)
            {
                bool enableMove = moves[i].Move != null;

                if (enableMove)
                {
                    MenuOptions[i].Show();

                    ((MoveButton)MenuOptions[i]).SetMove(moves[i],
                                                         owner,
                                                         useEffectiveness[i],
                                                         effectiveness.Count > i ? effectiveness[i] : 0,
                                                         true,
                                                         usableMoves.Contains(moves[i]),
                                                         true,
                                                         owner,
                                                         movesTarget,
                                                         battleManager);
                }
                else
                {
                    MenuOptions[i].Hide();
                    MenuOptions[i].Button.interactable = false;
                }

                enabledButtons.Add(enableMove);
            }

            UpdateLayout(enabledButtons);
        }

        /// <summary>
        /// Show or hide the mega evolution button.
        /// </summary>
        /// <param name="enabledButtons">Enabled buttons for the moves.</param>
        public override void UpdateLayout(List<bool> enabledButtons)
        {
            base.UpdateLayout(enabledButtons);

            MegaTip.Show(CanMegaevolve);
            ShouldMegaevolve = false;
        }

        /// <summary>
        /// Display or close the move info panel.
        /// </summary>
        /// <param name="context"></param>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            MoveInfoPanel.ShowInBattle(!MoveInfoPanel.Shown, movesOwner, movesTarget, battleManager);
        }

        /// <summary>
        /// Enable or disable megaevolution.
        /// </summary>
        /// <param name="context"></param>
        public override void OnExtra2(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (!CanMegaevolve) return;

            AudioManager.PlayAudio(SelectAudio);

            ShouldMegaevolve = !ShouldMegaevolve;
        }
    }
}