using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on the encounter type.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnEncounterType", fileName = "BallBasedOnEncounterType")]
    public class BallBasedOnEncounterType : Ball
    {
        /// <summary>
        /// The multiplier to get for each encounter.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private SerializableDictionary<EncounterType, float> MultipliersPerEncounter;

        /// <summary>
        /// Get the catch multiplier of this ball based on the encounter.
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
            EncounterType encounter = battleManager.Scenario.EncounterType;

            callback.Invoke(!MultipliersPerEncounter.TryGetValue(encounter, out float value)
                                ? BasicCatchMultiplier
                                : value);

            yield break;
        }
    }
}