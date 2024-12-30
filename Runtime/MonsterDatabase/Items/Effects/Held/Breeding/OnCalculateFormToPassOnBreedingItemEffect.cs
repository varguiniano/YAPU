using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect that modifies the form to pass on breeding.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnCalculateFormToPassOnBreeding",
                     fileName = "OnCalculateFormToPassOnBreeding")]
    public class
        OnCalculateFormToPassOnBreedingItemEffect : MonsterDatabaseScriptable<OnCalculateFormToPassOnBreedingItemEffect>
    {
        /// <summary>
        /// Force pass the parent form if available?
        /// </summary>
        [SerializeField]
        private bool ForcePassParentFormIfSameSpecies;

        /// <summary>
        /// Calculate the nature to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's form if it is of the same species?</returns>
        public bool OnCalculateFormToPassOnBreeding(MonsterInstance holder,
                                                    MonsterInstance otherParent,
                                                    bool isMother)
        {
            if (!ForcePassParentFormIfSameSpecies) return false;

            if (isMother) return true;

            return holder.Species == otherParent.Species;
        }
    }
}