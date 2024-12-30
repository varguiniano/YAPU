using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that changes the addition counter based on the target weight.
    /// Based on the Heavy ball algorithm: https://bulbapedia.bulbagarden.net/wiki/Heavy_Ball
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnWeight", fileName = "BallBasedOnWeight")]
    public class BallBasedOnWeight : Ball
    {
        /// <summary>
        /// The addition to get if the weight is equal or above the threshold.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private SerializableDictionary<float, float> AdditionPerWeight;

        /// <summary>
        /// Get a number to add instead of a multiplier to the formula based on the battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <returns>A number to add to the formula.</returns>
        public override float GetCatchAddition(BattleManager battleManager, Battler battler, Battler ownBattler)
        {
            if (battler.FormData.IsUltraBeast) return 0;

            float addition = 0;

            foreach (KeyValuePair<float, float> pair in AdditionPerWeight)
                if (battler.GetWeight(battleManager, false) >= pair.Key)
                    addition = pair.Value;

            return addition;
        }
    }
}