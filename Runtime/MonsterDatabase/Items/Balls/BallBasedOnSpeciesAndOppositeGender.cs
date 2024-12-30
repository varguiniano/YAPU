using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that has a better multiplier with the same species and opposite gender.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnSpeciesAndOppositeGender",
                     fileName = "BallBasedOnSpeciesAndOppositeGender")]
    public class BallBasedOnSpeciesAndOppositeGender : Ball
    {
        /// <summary>
        /// Same species multiplier.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        protected float SameSpecies = 1f;

        /// <summary>
        /// Opposite gender multiplier.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        protected float OppositeGender = 1f;

        /// <summary>
        /// Same species and opposite gender multiplier.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        protected float SameSpeciesAndOppositeGender = 8f;

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
                callback.Invoke(UltraBeastMultiplier);

            // Same species and gender.
            else if (battler.Species == ownBattler.Species
                  && battler.PhysicalData.Gender == ownBattler.PhysicalData.Gender)
                callback.Invoke(SameSpecies);

            // Different species and gender.
            else if (battler.Species != ownBattler.Species
                  && battler.PhysicalData.Gender != ownBattler.PhysicalData.Gender)
                callback.Invoke(OppositeGender);

            // Same species and opposite gender.
            else if (battler.Species == ownBattler.Species
                  && battler.PhysicalData.Gender != ownBattler.PhysicalData.Gender)
                callback.Invoke(SameSpeciesAndOppositeGender);

            callback.Invoke(BasicCatchMultiplier);

            yield break;
        }
    }
}