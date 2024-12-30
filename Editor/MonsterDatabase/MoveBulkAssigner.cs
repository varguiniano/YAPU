using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Editor.Utils;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Editor window that allows to select a move and to a bunch of monsters at once.
    /// </summary>
    public class MoveBulkAssigner : OdinEditorWindow
    {
        /// <summary>
        /// Open the window.
        /// </summary>
        [MenuItem("YAPU/Move Bulk Assigner")]
        private static void OpenWindow() => GetWindow<MoveBulkAssigner>().Show();

        /// <summary>
        /// Move to assign.
        /// </summary>
        [SerializeField]
        [OnValueChanged(nameof(RebuildList))]
        [ValueDropdown(nameof(GetAllMoves))]
        private Move Move;

        /// <summary>
        /// Lines with all mons and a toggle to assign the move.
        /// </summary>
        [Space]
        [SerializeField]
        [HideIf("@" + nameof(Move) + " == null")]
        [TableList(AlwaysExpanded = true, IsReadOnly = true)]
        private List<MoveBulkAssignLine> Monsters;

        /// <summary>
        /// Rebuild the list of monsters.
        /// </summary>
        private void RebuildList()
        {
            Monsters = new List<MoveBulkAssignLine>();

            // ReSharper disable twice ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (MonsterEntry monster in AssetManagementUtils.FindAssetsByType<MonsterEntry>())
            {
                foreach (Form form in monster.AvailableForms)
                {
                    MoveBulkAssignLine line = new(monster, form, Move);

                    Monsters.Add(line);
                }
            }
        }

        /// <summary>
        /// Retrieve all moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        protected List<Move> GetAllMoves() => allMoves ??= AssetManagementUtils.FindAssetsByType<Move>();

        /// <summary>
        /// Cached list of all moves.
        /// </summary>
        private List<Move> allMoves;

        /// <summary>
        /// Class representing a line in the bulk assigner.
        /// </summary>
        [Serializable]
        public class MoveBulkAssignLine
        {
            /// <summary>
            /// Reference to the monster.
            /// </summary>
            [ReadOnly]
            [HideLabel]
            public MonsterEntry Monster;

            /// <summary>
            /// The form being edited.
            /// </summary>
            [ReadOnly]
            [HideLabel]
            public Form Form;

            /// <summary>
            /// Does this mon know the move?
            /// </summary>
            [HideLabel]
            [HideIf(nameof(KnowsMoveInOtherCategoryThanOther))]
            [OnValueChanged(nameof(UpdateKnownMove))]
            public bool KnowsMove;

            /// <summary>
            /// Does the monster know the move in a category different than the other learnt moves?
            /// </summary>
            [ReadOnly]
            [HideLabel]
            [ShowIf(nameof(KnowsMoveInOtherCategoryThanOther))]
            [InfoBox("This monster knows the move as a level up move, egg move or on evolution move, so it can't be edited here.")]
            public bool KnowsMoveInOtherCategoryThanOther;

            /// <summary>
            /// The move being edited.
            /// </summary>
            private Move move;

            /// <summary>
            /// Reference to the form data.
            /// </summary>
            private DataByFormEntry FormData => Monster[Form];

            /// <summary>
            /// Constructor.
            /// </summary>
            public MoveBulkAssignLine(MonsterEntry monster, Form form, Move moveEdited)
            {
                Monster = monster;
                Form = form;
                move = moveEdited;

                KnowsMove = monster[form].GetOtherLearnMovesInEditor().Contains(move);

                KnowsMoveInOtherCategoryThanOther =
                    monster[form].MovesByLevel.Any(slot => slot.Move == move)
                 || monster[form].EggMoves.Contains(move)
                 || monster[form].OnEvolutionMoves.Contains(move);
            }

            /// <summary>
            /// Update the data when changing the toggle.
            /// </summary>
            private void UpdateKnownMove()
            {
                switch (KnowsMove)
                {
                    case true when !FormData.GetOtherLearnMovesInEditor().Contains(move):
                        FormData.AddOtherLearnMove(move);
                        break;
                    case false when FormData.GetOtherLearnMovesInEditor().Contains(move):
                        FormData.RemoveOtherLearnMoves(new List<Move> { move });
                        break;
                }

                EditorUtility.SetDirty(Monster);
                AssetDatabase.SaveAssets();
            }
        }
    }
}