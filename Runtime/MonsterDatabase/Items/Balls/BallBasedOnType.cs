using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on specific types.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnType", fileName = "BallBasedOnType")]
    public class BallBasedOnType : Ball
    {
        /// <summary>
        /// Multiplier to apply to specific types. If not set it defaults.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private SerializableDictionary<MonsterType, float> MultipliersByType;

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

            (MonsterType firstType, MonsterType secondType) = battler.GetTypes(battleManager.YAPUSettings);

            if (MultipliersByType.TryGetValue(firstType, out float value))
                callback.Invoke(value);
            else if (MultipliersByType.TryGetValue(secondType, out value))
                callback.Invoke(value);
            else
                callback.Invoke(BasicCatchMultiplier);
        }
    }
}