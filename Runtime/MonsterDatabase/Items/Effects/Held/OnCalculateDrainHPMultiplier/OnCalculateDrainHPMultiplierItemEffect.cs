using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateDrainHPMultiplier
{
    /// <summary>
    /// Base clas for item effects that modify the HP drained.
    /// </summary>
    public abstract class
        OnCalculateDrainHPMultiplierItemEffect : MonsterDatabaseScriptable<OnCalculateDrainHPMultiplierItemEffect>
    {
        /// <summary>
        /// Calculate the multiplier to use when draining HP.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="item">Item owner of this effect.</param>
        /// <param name="user">Battler draining.</param>
        /// <param name="target">Target to drain from, can be null.</param>
        /// <returns>The multiplier to apply.</returns>
        public abstract float CalculateDrainHPMultiplier(BattleManager battleManager,
                                                         Item item,
                                                         Battler user,
                                                         Battler target);
    }
}