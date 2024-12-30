using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers
{
    /// <summary>
    /// Data class for foggy weather outside battles.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/Weathers/Fog", fileName = "Fog")]
    public class Fog : OutOfBattleWeather
    {
        /// <summary>
        /// Start this weather.
        /// </summary>
        public override IEnumerator StartWeather(PlayerCharacter playerCharacter)
        {
            yield return playerCharacter.FX.ShowFog();
        }

        /// <summary>
        /// End this weather.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player's character.</param>
        /// <param name="isDestroyingCharacter">Is the character being destroyed?</param>
        public override IEnumerator EndWeather(PlayerCharacter playerCharacter, bool isDestroyingCharacter)
        {
            if (!isDestroyingCharacter) yield return playerCharacter.FX.ShowFog(false);
        }
    }
}