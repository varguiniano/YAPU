using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the VitalSpirit ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/VitalSpirit", fileName = "VitalSpirit")]
    public class VitalSpirit : Ability
    {
        // Immunities are implemented in the statuses.

        /// <summary>
        /// Only use the upper half.
        /// </summary>
        /// <param name="monster">Owner of the ability.</param>
        /// <param name="encounter">Encounter type.</param>
        /// <param name="minimum">Minimum level.</param>
        /// <param name="maximum">Maximum level.</param>
        /// <returns>The new limits.</returns>
        public override (byte minimum, byte maximum)
            ModifyEncounterLevels(MonsterInstance monster, EncounterType encounter, byte minimum, byte maximum) =>
            ((byte) (minimum + (maximum - minimum) / 2f), maximum);
    }
}