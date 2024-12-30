using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball based on if we are in a dungeon or the time of the day.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallBasedOnDungeonOrTimeOfDay",
                     fileName = "BallBasedOnDungeonOrTimeOfDay")]
    public class BallBasedOnDungeonOrTimeOfDay : BallBasedOnTimeOfDay
    {
        /// <summary>
        /// Multiplier for dungeons.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        private float DungeonMultiplier;

        /// <summary>
        /// Get the catch multiplier of this ball based on the dungeon or time of day.
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

            if (battleManager.Scenario.CurrentScene.IsDungeon)
                yield return base.GetCatchMultiplier(battleManager, battler, ownBattler, callback);
            else
                callback.Invoke(DungeonMultiplier);
        }
    }
}