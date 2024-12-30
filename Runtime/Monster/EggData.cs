using System;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Data containing the egg information of a monster.
    /// </summary>
    [Serializable]
    public struct EggData
    {
        /// <summary>
        /// Is the monster an egg?
        /// </summary>
        public bool IsEgg;

        /// <summary>
        /// Eggs cycles left for this egg to hatch.
        /// </summary>
        public byte EggCyclesLeft;
    }
}