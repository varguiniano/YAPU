using System;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters
{
    /// <summary>
    /// Class that pairs a move and an level for easy serialization.
    /// </summary>
    [Serializable]
    public class MoveLevelPair : MonsterDatabaseData
    {
        /// <summary>
        /// Reference to the move.
        /// </summary>
        [HorizontalGroup("Pair")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [LabelWidth(100)]
        public Move Move;

        /// <summary>
        /// Level value.
        /// </summary>
        [HorizontalGroup("Pair")]
        [LabelWidth(100)]
        public byte Level;
    }
}