using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Psystrike.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/Psystrike", fileName = "Psystrike")]
    public class Psystrike : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Psyshock uses the Defense instead of the Special Defense.
        /// </summary>
        protected override Stat GetDefenseStatForDamageCalculation(BattleManager battleManager,
                                                                   Battler user,
                                                                   Battler target,
                                                                   bool ignoresAbilities) =>
            Stat.Defense;
    }
}