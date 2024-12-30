using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the SandVeil ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SandVeil", fileName = "SandVeil")]
    public class SandVeil : Ability
    {
        /// <summary>
        /// Weather compatible with this ability.
        /// </summary>
        [SerializeField]
        private Weather CompatibleWeather;

        /// <summary>
        /// Weather compatible with this ability.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather OutOfBattleCompatibleWeather;

        /// <summary>
        /// Reduce accuracy on a sandstorm.
        /// </summary>
        public override float
            GetMoveAccuracyMultiplierWhenTargeted(Battler owner,
                                                  BattleManager battleManager,
                                                  Battler user,
                                                  Move move) =>
            base.GetMoveAccuracyMultiplierWhenTargeted(owner, battleManager, user, move)
          * (battleManager.Scenario.GetWeather(out Weather weather) && weather == CompatibleWeather ? .8f : 1f);

        /// <summary>
        /// Reduce encounter chance by 50% on the weather.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public override float
            OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            playerCharacter.CurrentWeather == OutOfBattleCompatibleWeather
                ? .5f
                : base.OnCalculateEncounterChance(playerCharacter, encounterType);
    }
}