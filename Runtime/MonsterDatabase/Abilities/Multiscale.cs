using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Multiscale ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Multiscale", fileName = "Multiscale")]
    public class Multiscale : Ability
    {
        /// <summary>
        /// Reduce to half when at full HP.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                      float multiplier,
                                                                      Battler user,
                                                                      Battler target,
                                                                      BattleManager battleManager,
                                                                      Action<float> finished)
        {
            finished.Invoke(multiplier * (user.GetStats(battleManager)[Stat.Hp] == user.CurrentHP ? 0.5f : 1));
            yield break;
        }
    }
}