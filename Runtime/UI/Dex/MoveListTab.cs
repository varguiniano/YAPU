﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Moves;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Base class for tabs that display a list of moves.
    /// </summary>
    public abstract class MoveListTab : MonsterDexTab
    {
        /// <summary>
        /// Audio to play when entering the menu.
        /// </summary>
        [SerializeField]
        private AudioReference EnterMenuAudio;

        /// <summary>
        /// Menu controller that allows viewing the moves info.
        /// </summary>
        [SerializeField]
        protected MovesMenu MovesSelector;

        /// <summary>
        /// Reference to the move info panel.
        /// </summary>
        [SerializeField]
        private MoveInfoPanel MoveInfoPanel;

        /// <summary>
        /// Reference to the tips shower.
        /// </summary>
        [SerializeField]
        private SingleDexTipsShower TipsShower;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Subscribe to menu events.
        /// </summary>
        private void OnEnable()
        {
            MovesSelector.OnHovered += OnMoveHovered;
            MovesSelector.OnBackSelected += OnBackSelected;
        }

        /// <summary>
        /// Unsubscribe from menu events.
        /// </summary>
        private void OnDisable()
        {
            MovesSelector.OnHovered -= OnMoveHovered;
            MovesSelector.OnBackSelected -= OnBackSelected;
        }

        /// <summary>
        /// Enter the moves menu when the select button is pressed.
        /// </summary>
        public override void OnSelectPressedOnParentScreen()
        {
            if (MovesSelector.Data.Count == 0) return;

            audioManager.PlayAudio(EnterMenuAudio);
            MovesSelector.RequestInput();
            MoveInfoPanel.ShowOutOfBattle();
            TipsShower.SwitchToSubmenu();
        }

        /// <summary>
        /// Called when an ability is hovered.
        /// </summary>
        /// <param name="index">Index hovered.</param>
        private void OnMoveHovered(int index) => MoveInfoPanel.SetMove(MovesSelector.Data[index]);

        /// <summary>
        /// Called when back is selected in the menu.
        /// </summary>
        private void OnBackSelected()
        {
            MoveInfoPanel.ShowOutOfBattle(show: false);
            MovesSelector.ReleaseInput();
            TipsShower.SwitchToGeneral();
        }

        /// <summary>
        /// Set the data from this monster into the tab.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            MovesSelector.SetButtons(GetMoves(data).OrderBy(move => localizer[move.LocalizableName]).ToList());
        }

        /// <summary>
        /// Retrieve the moves to display.
        /// </summary>
        /// <param name="data">Monster data.</param>
        /// <returns>The list of moves to display.</returns>
        protected abstract IEnumerable<Move> GetMoves(DataByFormEntry data);
    }
}