using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the EarlyBird ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/EarlyBird", fileName = "EarlyBird")]
    public class EarlyBird : Ability
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float SleepCountdownMultiplier = .5f;

        /// <summary>
        /// Called to modify the sleep countdown when added to this monster.
        /// </summary>
        /// <returns>Multiplier to apply to the sleep.</returns>
        public override float CalculateSleepCountDownMultiplier(Battler owner, BattleManager battleManager) =>
            SleepCountdownMultiplier;
    }
}