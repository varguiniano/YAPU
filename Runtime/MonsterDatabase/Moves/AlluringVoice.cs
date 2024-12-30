using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move AlluringVoice.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fairy/AlluringVoice", fileName = "AlluringVoice")]
    public class AlluringVoice : StatusChanceDamageMove
    {
        /// <summary>
        /// Execute the secondary effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                if (!target.IncreasedStatsThisTurn) yield break;

                yield return AddStatusesToTarget(battleManager,
                                                 localizer,
                                                 userType,
                                                 userIndex,
                                                 user,
                                                 targets,
                                                 targetData,
                                                 target,
                                                 hitNumber,
                                                 expectedHits,
                                                 externalPowerMultiplier,
                                                 ignoresAbilities);
            }
        }
    }
}