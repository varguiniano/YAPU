using System;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters
{
    /// <summary>
    /// Class that stores a pair of a stat and an int value.
    /// </summary>
    [Serializable]
    public class StatByteValuePair : MonsterDatabaseData
    {
        /// <summary>
        /// Reference to the Stat.
        /// </summary>
        [HorizontalGroup("Stat")]
        [LabelWidth(100)]
        public Stat Stat;

        /// <summary>
        /// Value.
        /// </summary>
        [HorizontalGroup("Stat")]
        [LabelWidth(100)]
        public byte Value;
    }
}