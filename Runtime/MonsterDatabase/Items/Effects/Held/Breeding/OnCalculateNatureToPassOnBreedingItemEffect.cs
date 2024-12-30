using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect that modifies the nature to pass on breeding.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnCalculateNatureToPassOnBreeding",
                     fileName = "OnCalculateNatureToPassOnBreeding")]
    public class OnCalculateNatureToPassOnBreedingItemEffect : MonsterDatabaseScriptable<
        OnCalculateNatureToPassOnBreedingItemEffect>
    {
        /// <summary>
        /// Force pass the parent nature?
        /// </summary>
        [SerializeField]
        private bool ForcePassParentNature;

        /// <summary>
        /// Calculate the nature to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's nature?</returns>
        public bool OnCalculateNatureToPassOnBreeding(MonsterInstance holder,
                                                      MonsterInstance otherParent,
                                                      MonsterEntry species,
                                                      Form form,
                                                      bool isMother) =>
            ForcePassParentNature;
    }
}