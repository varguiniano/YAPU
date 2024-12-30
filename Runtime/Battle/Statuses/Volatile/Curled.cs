using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Curled status.
    /// Added after using Defense Curl.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Curled", fileName = "Curled")]
    public class Curled : VolatileStatus
    {
        /// <summary>
        /// Multiplier to apply to certain moves when used.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<Move, float> MovePowerMultipliers;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="owner">Owner of the status and move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float GetMovePowerMultiplierWhenUsingMove(Battler owner,
                                                                  Move move,
                                                                  Battler target,
                                                                  BattleManager battleManager)
        {
            float multiplier = MovePowerMultipliers.GetValueOrDefault(move, 1);

            return multiplier * base.GetMovePowerMultiplierWhenUsingMove(owner, move, target, battleManager);
        }
    }
}