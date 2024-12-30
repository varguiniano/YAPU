using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Behaviour to display the moves of a monster.
    /// </summary>
    public class MonsterMovesDisplay : HidableUiElement<MonsterMovesDisplay>
    {
        /// <summary>
        /// Reference to the move buttons.
        /// </summary>
        [SerializeField]
        private List<MoveButton> MoveButtons;

        /// <summary>
        /// Set the moves to display.
        /// </summary>
        /// <param name="monster">Monster to get the moves from.</param>
        /// <param name="effectiveness">Effectiveness of this moves.</param>
        /// <param name="useEffectiveness">Use the effectiveness?</param>
        public void SetMoves(MonsterInstance monster,
                             List<float> effectiveness = null,
                             List<bool> useEffectiveness = null)
        {
            useEffectiveness ??= new List<bool>
                                 {
                                     false,
                                     false,
                                     false,
                                     false
                                 };

            for (int i = 0; i < monster.CurrentMoves.Length; i++)
            {
                MoveSlot slot = monster.CurrentMoves[i];

                if (slot.Move == null)
                    MoveButtons[i].Hide();
                else
                {
                    MoveButtons[i]
                       .SetMove(slot, monster, useEffectiveness[i], effectiveness != null ? effectiveness[i] : 1f);

                    MoveButtons[i].Show();
                }
            }
        }
    }
}