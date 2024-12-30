using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items
{
    /// <summary>
    /// Data class that represents the category of an item.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Category", fileName = "ItemCategory")]
    public class ItemCategory : LocalizableMonsterDatabaseScriptable<ItemCategory>
    {
        /// <summary>
        /// Localization string for item categories.
        /// </summary>
        protected override string BaseLocalizationRoot => "Item/Category/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Category icon.
        /// </summary>
        [PreviewField(100)]
        public Sprite Icon;
    }
}