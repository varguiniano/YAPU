using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios
{
    /// <summary>
    /// Data class for battle scenarios.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/BattleScenario", fileName = "BattleScenario")]
    public class BattleScenario : LocalizableMonsterDatabaseScriptable<BattleScenario>
    {
        /// <summary>
        /// Prefab for this scenario's background.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public GameObject BackgroundPrefab;
        
        /// <summary>
        /// Move to be used by nature power when this terrain is on.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Effect")]
        public Move NaturePowerMove;

        /// <summary>
        /// Base localization root for the asset.
        /// </summary>
        protected override string BaseLocalizationRoot => "BattleScenario/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;
    }
}