using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based only on the target level.
    /// Equation from: https://bulbapedia.bulbagarden.net/wiki/Nest_Ball
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnTargetLevel", fileName = "BallBasedOnTargetLevel")]
    public class BallBasedOnTargetLevel : Ball
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
            if (battler.FormData.IsUltraBeast)
            {
                callback.Invoke(UltraBeastMultiplier);
                yield break;
            }

            if (battler.StatData.Level is > 0 and < 30)
                callback.Invoke((41 - battler.StatData.Level) / 10f);
            else
                callback.Invoke(BasicCatchMultiplier);
        }
    }
}