using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Hex.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/Hex", fileName = "Hex")]
    public class Hex : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Multiplier to apply when the target has a status.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        private float StatusPowerMultiplier = 2;

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int power = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (target == null || target.GetStatus() == null) return power;

            return Mathf.RoundToInt(power * StatusPowerMultiplier);
        }
    }
}