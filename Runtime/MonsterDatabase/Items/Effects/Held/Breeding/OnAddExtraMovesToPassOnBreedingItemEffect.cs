using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect adds extra moves to pass on breeding.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnAddExtraMovesToPassOnBreeding",
                     fileName = "OnAddExtraMovesToPassOnBreeding")]
    public class
        OnAddExtraMovesToPassOnBreedingItemEffect : MonsterDatabaseScriptable<OnAddExtraMovesToPassOnBreedingItemEffect>
    {
        /// <summary>
        /// Moves to pass.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private List<Move> Moves;

        /// <summary>
        /// Callback that allows the monster to add extra moves to the baby mon when breeding.
        /// this is useful for effects like Light Ball.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="item">Item this effect belongs to.</param>
        /// <param name="isMother">Is this mon the mother?</param>
        /// <param name="otherParent">Reference to the other parent.</param>
        /// <param name="babySpecies">Species of the baby.</param>
        /// <param name="babyForm">Form of the baby.</param>
        /// <returns>A list of the moves to include.</returns>
        public IEnumerable<Move> AddExtraLearntMovesWhenBreeding(MonsterInstance owner,
                                                                 Item item,
                                                                 bool isMother,
                                                                 MonsterInstance otherParent,
                                                                 MonsterEntry babySpecies,
                                                                 Form babyForm) =>
            CheckCondition(owner, item, isMother, otherParent, babySpecies, babyForm) ? Moves : new Move[] { };

        /// <summary>
        /// Check the condition to add the moves.
        /// </summary>
        protected virtual bool CheckCondition(MonsterInstance owner,
                                              Item item,
                                              bool isMother,
                                              MonsterInstance otherParent,
                                              MonsterEntry babySpecies,
                                              Form babyForm) =>
            true;
    }
}