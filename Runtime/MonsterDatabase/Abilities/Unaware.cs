using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Unaware.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Unaware", fileName = "Unaware")]
    public class Unaware : Ability
    {
        /// <summary>
        /// Ignore the targets evasion when calculating move accuracy?
        /// </summary>
        public override bool IgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing() => true;

        /// <summary>
        /// Ignore the users accuracy when calculating move accuracy?
        /// </summary>
        public override bool IgnoreAccuracyWhenCalculatingMoveAccuracyWhenTargeted() => true;

        /// <summary>
        /// Ignore the users attack when targeted by a move?
        /// </summary>
        public override bool IgnoreAttackStageWhenWhenTargeted() => true;

        /// <summary>
        /// Ignore the targets defense when using a move?
        /// </summary>
        public override bool IgnoreDefenseStageWhenWhenUsingMove() => true;
    }
}