using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that has a different multiplier on the first turn.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/FirstTurnBall", fileName = "FirstTurnBall")]
    public class FirstTurnBall : Ball
    {
        /// <summary>
        /// Multiplier to use on the first turn.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private float FirstTurnMultiplier;

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

            callback.Invoke(battleManager.TurnCounter == 0 ? FirstTurnMultiplier : BasicCatchMultiplier);
        }
    }
}