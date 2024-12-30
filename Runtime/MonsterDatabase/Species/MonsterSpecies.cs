using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Species
{
    /// <summary>
    /// Data class for monster species.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Species", fileName = "Species")]
    public class MonsterSpecies : LocalizableMonsterDatabaseScriptable<MonsterSpecies>
    {
        /// <summary>
        /// All species start with Species/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Species/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;
    }
}