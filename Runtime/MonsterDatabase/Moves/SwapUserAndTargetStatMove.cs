using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Move that swaps a stat of the user and target.
    /// </summary>
    public abstract class SwapUserAndTargetStatMove : ChangeUserAndTargetStatMove
    {
        /// <summary>
        /// Swap them around.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="affectedBattler">Battle to calculate the stat for.</param>
        /// <param name="stat">Stat to affect.</param>
        /// <returns>The new value.</returns>
        protected override uint GetNewStatValue(BattleManager battleManager,
                                                Battler user,
                                                Battler target,
                                                Battler affectedBattler,
                                                Stat stat) =>
            (affectedBattler == user ? target : user).GetStats(battleManager)[stat];
    }
}