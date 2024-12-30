using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that can check the compatibility of a monster on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/MonsterCompatibility/NotAnEggAndNotInEggGroups",
                     fileName = "NotAnEggAndNotInEggGroups")]
    public class NotAnEggAndNotInEggGroups : MonsterCompatibilityChecker
    {
        /// <summary>
        /// Egg groups incompatible for this checker.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllEggGroups))]
        #endif
        private List<EggGroup> IncompatibleEggGroups;

        /// <summary>
        /// Localization key to use when the monster is not compatible.
        /// </summary>
        public override string GetNotCompatibleLocalizationKey(MonsterInstance monster) =>
            monster.EggData.IsEgg
                ? "Dialogs/MonsterCompatibility/IsEgg"
                : "Dialogs/MonsterCompatibility/Incompatible";

        /// <summary>
        /// Check if a monster is compatible.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="settings"></param>
        /// <returns>True if it is compatible.</returns>
        public override bool IsMonsterCompatible(MonsterInstance monster, YAPUSettings settings) =>
            !monster.EggData.IsEgg && !IncompatibleEggGroups.Any(group => monster.FormData.EggGroups.Contains(group));
    }
}