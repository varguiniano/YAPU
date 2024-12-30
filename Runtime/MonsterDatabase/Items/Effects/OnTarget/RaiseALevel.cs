using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Raise a level of the target monster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RaiseALevel", fileName = "RaiseALevel")]
    public class RaiseALevel : RaiseExperience
    {
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
            (uint)(monsterInstance.GetExperienceForNextLevel(experienceLookupTable)
                 - monsterInstance.StatData.CurrentLevelExperience);
    }
}