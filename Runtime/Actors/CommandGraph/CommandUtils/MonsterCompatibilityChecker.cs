using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that can check the compatibility of a monster on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/MonsterCompatibility/AlwaysCompatible", fileName = "AlwaysCompatible")]
    public class MonsterCompatibilityChecker : MonsterDatabaseScriptable<MonsterCompatibilityChecker>
    {
        /// <summary>
        /// Localization key to use when the monster is not compatible.
        /// </summary>
        public virtual string GetNotCompatibleLocalizationKey(MonsterInstance monster) => "";

        /// <summary>
        /// Check if a monster is compatible.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>True if it is compatible.</returns>
        public virtual bool IsMonsterCompatible(MonsterInstance monster, YAPUSettings settings) => true;
    }
}