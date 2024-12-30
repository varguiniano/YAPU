using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers
{
    /// <summary>
    /// Base class for weathers that happen outside of battle.
    /// </summary>
    public abstract class OutOfBattleWeather : MonsterDatabaseScriptable<OutOfBattleWeather>
    {
        /// <summary>
        /// Weather to set if battle is entered in this weather.
        /// </summary>
        public Weather InBattleWeather;

        /// <summary>
        /// Icon for this weather.
        /// </summary>
        [PreviewField]
        public Sprite Icon;

        /// <summary>
        /// Start this weather.
        /// </summary>
        public abstract IEnumerator StartWeather(PlayerCharacter playerCharacter);

        /// <summary>
        /// End this weather.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player's character.</param>
        /// <param name="isDestroyingCharacter">Is the character being destroyed?</param>
        public abstract IEnumerator EndWeather(PlayerCharacter playerCharacter, bool isDestroyingCharacter);
    }
}