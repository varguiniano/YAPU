using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for when a monster is enraged by Rage Fist.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/RageFistStatus", fileName = "RageFistStatus")]
    public class RageFistStatus : VolatileStatus
    {
        /// <summary>
        /// Dictionary of battlers and the times they've been hit.
        /// </summary>
        private readonly Dictionary<Battler, uint> hitTimes = new();

        /// <summary>
        /// Get the hit times of a battler.
        /// </summary>
        public bool GetHitTimes(Battler battler, out uint hits) => hitTimes.TryGetValue(battler, out hits);

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break; // No dialog.
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
            yield break; // No dialog.
        }

        /// <summary>
        /// Called when the holder is hit by a move.
        /// Increase the amount of hit times if it's a damaging move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finished">Callback stating the multiplier for the effectiveness and if it will force survive.</param>
        public override IEnumerator OnHitByMove(DamageMove move,
                                                float effectiveness,
                                                Battler battler,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                bool ignoresAbilities,
                                                Action<float, bool> finished)
        {
            if (!hitTimes.TryAdd(battler, 1)) hitTimes[battler]++;

            yield return base.OnHitByMove(move, effectiveness, battler, battleManager, moveUser, ignoresAbilities, finished);
        }

        /// <summary>
        /// Clear the dictionary on battle end.
        /// </summary>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            hitTimes.Clear();

            return base.OnBattleEnded(battler);
        }
    }
}