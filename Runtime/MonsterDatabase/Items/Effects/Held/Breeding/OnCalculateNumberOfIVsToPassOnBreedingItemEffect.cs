using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect that modifies the number of IVs to pass on breeding.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnCalculateNumberOfIVsSimpleModifier",
                     fileName = "OnCalculateNumberOfIVsSimpleModifier")]
    public class
        OnCalculateNumberOfIVsToPassOnBreedingItemEffect : MonsterDatabaseScriptable<
            OnCalculateNumberOfIVsToPassOnBreedingItemEffect>
    {
        /// <summary>
        /// Modifier to apply.
        /// </summary>
        [SerializeField]
        private int Modifier;

        /// <summary>
        /// Calculate the number of IVs to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The number to pass. -1 if unchanged.</returns>
        public int OnCalculateNumberOfIVsToPassOnBreeding(MonsterInstance holder,
                                                          MonsterInstance otherParent,
                                                          MonsterEntry species,
                                                          Form form,
                                                          bool isMother) =>
            Modifier;
    }
}