using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnEvolution
{
    /// <summary>
    /// Data class for an item effect that can prevent a monster from evolving.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Evolution/AllowEvolutionItemEffect",
                     fileName = "AllowEvolutionItemEffect")]
    public class AllowEvolutionItemEffect : MonsterDatabaseScriptable<AllowEvolutionItemEffect>
    {
        /// <summary>
        /// Does this effect allow evolution?
        /// </summary>
        [SerializeField]
        private bool AllowsEvolution;

        /// <summary>
        /// Allow the monster to evolve?
        /// </summary>
        /// <param name="item">Item of this effect.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="evolutionData">Evolution data to use.</param>
        /// <returns>True if it allows evolution.</returns>
        public bool AllowEvolution(Item item, MonsterInstance holder, EvolutionData evolutionData) => AllowsEvolution;
    }
}