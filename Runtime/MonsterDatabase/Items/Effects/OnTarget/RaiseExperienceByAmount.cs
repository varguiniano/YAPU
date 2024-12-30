using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Item effect to raise experience to a monster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RaiseExperienceByAmount",
                     fileName = "RaiseExperienceByAmount")]
    public class RaiseExperienceByAmount : RaiseExperience
    {
        /// <summary>
        /// Amount to raise.
        /// </summary>
        [SerializeField]
        private uint Amount;

        /// <summary>
        /// Get the amount by which to raise the experience.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <returns>The amount to raise.</returns>
        protected override uint GetRaiseAmount(MonsterInstance monsterInstance,
                                               YAPUSettings settings,
                                               ExperienceLookupTable experienceLookupTable) =>
            Amount;
    }
}