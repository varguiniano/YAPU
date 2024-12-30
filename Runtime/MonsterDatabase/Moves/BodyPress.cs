using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move BodyPress.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/BodyPress", fileName = "BodyPress")]
    public class BodyPress : DamageMove
    {
        /// <summary>
        /// Body press uses the defense.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <returns>The stat to use.</returns>
        protected override Stat GetAttackStatForDamageCalculation(BattleManager battleManager,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            Stat.Defense;

        // TODO: Animation.
    }
}