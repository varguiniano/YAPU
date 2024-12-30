using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on the turn count of the battle.
    /// Equation from: https://bulbapedia.bulbagarden.net/wiki/Timer_Ball
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnTurnCount", fileName = "BallBasedOnTurnCount")]
    public class BallBasedOnTurnCount : Ball
    {
        /// <summary>
        /// Get the catch multiplier of this ball based on the battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <param name="callback"></param>
        /// <returns>A float with the multiplier.</returns>
        public override IEnumerator GetCatchMultiplier(BattleManager battleManager,
                                                       Battler battler,
                                                       Battler ownBattler,
                                                       Action<float> callback)
        {
            callback.Invoke(battler.FormData.IsUltraBeast
                                ? UltraBeastMultiplier
                                : Mathf.Min(1 + battleManager.TurnCounter * 1229 / 4096f, 4f));

            yield break;
        }
    }
}