using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that can check the compatibility of a monster on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/MonsterCompatibility/DoesntHaveMaxSheen",
                     fileName = "DoesntHaveMaxSheen")]
    public class DoesntHaveMaxSheenChecker : MonsterCompatibilityChecker
    {
        /// <summary>
        /// Localization key to use when the monster is not compatible.
        /// </summary>
        public override string GetNotCompatibleLocalizationKey(MonsterInstance monster) =>
            monster.EggData.IsEgg ? "Dialogs/MonsterCompatibility/Egg" : "Dialogs/MonsterCompatibility/CantEat";

        /// <summary>
        /// Check if a monster is compatible.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="settings"></param>
        /// <returns>True if it is compatible.</returns>
        public override bool IsMonsterCompatible(MonsterInstance monster, YAPUSettings settings) =>
            !monster.EggData.IsEgg && monster.Sheen < settings.MaxSheen;
    }
}