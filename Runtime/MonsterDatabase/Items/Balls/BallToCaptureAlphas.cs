using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that has a different multiplier when capturing alphas.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallToCaptureAlphas", fileName = "BallToCaptureAlphas")]
    public class BallToCaptureAlphas : Ball
    {
        /// <summary>
        /// Multiplier when the monster is an alpha.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private float AlphaCatchMultiplier = 5f;

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
            if (battler.OriginData.IsAlpha)
                callback.Invoke(AlphaCatchMultiplier);
            else
                yield return base.GetCatchMultiplier(battleManager, battler, ownBattler, callback);
        }
    }
}