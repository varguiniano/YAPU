﻿using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster.Evolution;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on if the mon can evolve with an item.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnItemEvolution", fileName = "BallBasedOnItemEvolution")]
    public class BallBasedOnItemEvolution : Ball
    {
        /// <summary>
        /// The evolution item that the mon needs to evolve.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private Item EvolutionItem;

        /// <summary>
        /// Multiplier to apply if it can evolve.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        public float Multiplier;

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

            foreach (EvolutionData evolutionData in battler.FormData.Evolutions)
                if (evolutionData is EvolveOnItemUse evolveOnItemUse && evolveOnItemUse.ItemToUse == EvolutionItem)
                {
                    callback.Invoke(Multiplier);
                    yield break;
                }

            callback.Invoke(BasicCatchMultiplier);
        }
    }
}