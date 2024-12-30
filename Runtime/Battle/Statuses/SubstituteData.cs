using System;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses
{
    /// <summary>
    /// Struct representing the data of a battler's substitute.
    /// </summary>
    [Serializable]
    public struct SubstituteData
    {
        /// <summary>
        /// Is the substitute enabled?
        /// </summary>
        public bool SubstituteEnabled;

        /// <summary>
        /// Max Hp os the substitute.
        /// </summary>
        public uint MaxHP;

        /// <summary>
        /// Current HP of the substitute.
        /// </summary>
        public uint CurrentHP;
    }
}