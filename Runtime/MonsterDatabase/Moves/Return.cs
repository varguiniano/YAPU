using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Return.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Return", fileName = "Return")]
    public class Return : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="owner">Owner of the move</param>
        /// <returns>The move's power.</returns>
        public override int GetMovePower(MonsterInstance owner = null) =>
            owner == null
                // ReSharper disable once RedundantArgumentDefaultValue
                ? base.GetMovePower(null)
                : Mathf.Clamp(Mathf.RoundToInt(owner.Friendship / 2.5f), 1, int.MaxValue);
    }
}