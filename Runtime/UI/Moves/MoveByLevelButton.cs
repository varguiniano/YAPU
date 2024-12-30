using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Controller for a menu item that represents a move that is learnt by level.
    /// </summary>
    public class MoveByLevelButton : MoveButton
    {
        /// <summary>
        /// Reference to the text to display the level.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text LevelText;

        /// <summary>
        /// Set the move and level for this button.
        /// </summary>
        /// <param name="move">Move to display.</param>
        /// <param name="level">Level to display.</param>
        public void SetMoveAndLevel(Move move, byte level)
        {
            LevelText.SetText("Lv. " + level);
            SetMove(move);
        }

        /// <summary>
        /// Factory to be used in DI.
        /// </summary>
        public class MoveByLevelFactory : GameObjectFactory<MoveByLevelButton>
        {
        }
    }
}