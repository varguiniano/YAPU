using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Minimized status.
    /// Added after using Minimize.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Minimized", fileName = "Minimized")]
    public class Minimized : VolatileStatus
    {
        /// <summary>
        /// Multiplier to apply to certain moves when targeted.
        /// Accuracy will also be bypassed.
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
        /// Does this status bypass all accuracy checks when being targeted by a move?
        /// </summary>
        public override bool DoesBypassAllAccuracyChecksWhenTargeted(Battler owner,
                                                                     Move move,
                                                                     Battler user,
                                                                     BattleManager battleManager) =>
            MovePowerMultipliers.ContainsKey(move);

        /// <summary>
        /// Get the power of a move that is going to hit this battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Battler being hit, owner of this status.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float GetMovePowerMultiplierWhenHit(BattleManager battleManager,
                                                            Move move,
                                                            Battler user,
                                                            Battler target)
        {
            float multiplier = MovePowerMultipliers.GetValueOrDefault(move, 1);

            return multiplier * base.GetMovePowerMultiplierWhenUsingMove(target, move, target, battleManager);
        }
    }
}