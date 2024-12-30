using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the SandForce ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SandForce", fileName = "SandForce")]
    public class SandForce : Ability
    {
        /// <summary>
        /// Weather compatible with this ability.
        /// </summary>
        [SerializeField]
        private Weather CompatibleWeather;

        /// <summary>
        /// Moves types compatible.
        /// </summary>
        [SerializeField]
        private List<MonsterType> CompatibleMoveTypes;

        /// <summary>
        /// Power multiplier to apply in the compatible weather to compatible types.
        /// </summary>
        [SerializeField]
        private float PowerMultiplier = 1.3f;

        /// <summary>
        /// Multiply if in compatible weather.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities)
        {
            float multiplier = base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);

            if (battleManager.Scenario.GetWeather(out Weather weather)
             && weather == CompatibleWeather
             && CompatibleMoveTypes.Contains(move.GetMoveTypeInBattle(user, battleManager)))
                multiplier *= PowerMultiplier;

            return multiplier;
        }
    }
}