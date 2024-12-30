using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDex;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on if the monster has been caught before.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnDexCaught", fileName = "BallBasedOnDexCaught")]
    public class BallBasedOnDexCaught : Ball
    {
        /// <summary>
        /// Multipliers to apply if the monster has been caught before.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private float CaughtMultiplier;

        /// <summary>
        /// Get the catch multiplier of this ball based on the time of day.
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

            MonsterDexEntry monsterDexEntry = null;

            yield return battleManager.Dex.GetEntry(battler.Species,
                                                    entry =>
                                                    {
                                                        monsterDexEntry = entry;
                                                    });

            callback.Invoke(monsterDexEntry.HasMonsterBeenCaught
                                ? CaughtMultiplier
                                : BasicCatchMultiplier);
        }
    }
}