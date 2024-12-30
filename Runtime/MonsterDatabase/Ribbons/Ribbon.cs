using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons
{
    /// <summary>
    /// Ribbon a monster can have.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Ribbon", fileName = "Ribbon")]
    public class Ribbon : LocalizableMonsterDatabaseScriptable<Ribbon>
    {
        /// <summary>
        /// All ribbons should start with Ribbons/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Ribbons/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// Sprite for this ribbon.
        /// </summary>
        [PreviewField(100)]
        public Sprite Sprite;
    }
}