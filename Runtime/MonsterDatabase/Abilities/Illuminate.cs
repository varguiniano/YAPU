using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Illuminate.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Illuminate", fileName = "Illuminate")]
    public class Illuminate : PreventStatChangeAbility
    {
        /// <summary>
        /// Always duplicate the chance of having encounters.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public override float
            OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            2;
    }
}