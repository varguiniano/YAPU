using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on the level.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnLevel", fileName = "BallBasedOnLevel")]
    public class BallBasedOnLevel : Ball
    {
        /// <summary>
        /// The multiplier to get if the level ratio is above the given.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private SerializableDictionary<float, float> MultipliersPerRatio;

        /// <summary>
        /// Get the catch multiplier of this ball based on the battler.
        /// Algorithm based on https://bulbapedia.bulbagarden.net/wiki/Level_Ball#Manual_activation.
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

            if (ownBattler.StatData.Level <= battler.StatData.Level)
            {
                callback.Invoke(BasicCatchMultiplier);
                yield break;
            }

            float ratio = (float) ownBattler.StatData.Level / battler.StatData.Level;

            float multiplier = BasicCatchMultiplier;

            foreach (KeyValuePair<float, float> pair in MultipliersPerRatio)
                if (pair.Key < ratio)
                    multiplier = pair.Value;

            callback.Invoke(multiplier);
        }
    }
}