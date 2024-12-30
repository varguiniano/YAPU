using UnityEngine;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateEncounterChance
{
    /// <summary>
    /// Data class for an item effect that modifies the encounter chance for wild encounters.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateEncounterChanceItemEffect/SimpleModifier",
                     fileName = "OnCalculateEncounterChanceItemEffect")]
    public class OnCalculateEncounterChanceItemEffect : MonsterDatabaseScriptable<OnCalculateEncounterChanceItemEffect>
    {
        /// <summary>
        /// Modifier to apply.
        /// </summary>
        [SerializeField]
        private float Modifier = 1;

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public float OnCalculateEncounterChance(EncounterType encounterType) => Modifier;
    }
}