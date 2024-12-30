using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect adds extra moves to pass on breeding when the baby is a specific species.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnAddExtraMovesToPassToSpeciesOnBreeding",
                     fileName = "OnAddExtraMovesToPassToSpeciesOnBreeding")]
    public class OnAddExtraMovesToPassToSpeciesOnBreeding : OnAddExtraMovesToPassOnBreedingItemEffect
    {
        /// <summary>
        /// Species to add the moves to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        private MonsterEntry TargetSpecies;

        /// <summary>
        /// Check the condition to add the moves.
        /// </summary>
        protected override bool CheckCondition(MonsterInstance owner,
                                               Item item,
                                               bool isMother,
                                               MonsterInstance otherParent,
                                               MonsterEntry babySpecies,
                                               Form babyForm) =>
            babySpecies == TargetSpecies;
    }
}