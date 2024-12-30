using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnStatCalculation
{
    /// <summary>
    /// Base class for an held item effect that affects a monster's stat calculation.
    /// </summary>
    public abstract class OnStatCalculationItemEffect : MonsterDatabaseScriptable<OnStatCalculationItemEffect>
    {
        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        public abstract float OnCalculateStat(Item item,
                                              MonsterInstance monster,
                                              Stat stat);
    }
}