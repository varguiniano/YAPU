using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for move's which damage is based on the target's weight.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/DamageBasedOnTargetWeight",
                     fileName = "DamageBasedOnTargetWeightMove")]
    public class DamageBasedOnTargetWeightMove : DamageMove
    {
        /// <summary>
        /// Dictionary of the power of the move by the weight.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<float, int> PowerByWeight;

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
            if (target == null) return 0;

            foreach (KeyValuePair<float, int> pair in
                     PowerByWeight.Where(pair => target.GetWeight(battleManager, ignoresAbilities) < pair.Key))
                return pair.Value;

            return 0;
        }
    }
}