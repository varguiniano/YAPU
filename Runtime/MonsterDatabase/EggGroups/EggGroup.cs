using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups
{
    /// <summary>
    /// Class representing a monster egg group.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/EggGroup", fileName = "EggGroup")]
    public class EggGroup : LocalizableMonsterDatabaseScriptable<EggGroup>
    {
        /// <summary>
        /// All egg groups should start with EggGroups/.
        /// </summary>
        protected override string BaseLocalizationRoot => "EggGroups/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;
    }
}