using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Badges
{
    /// <summary>
    /// Data structure that represents a badge in the game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Badges/Badge", fileName = "Badge")]
    public class Badge : LocalizableMonsterDatabaseScriptable<Badge>
    {
        /// <summary>
        /// Base localization root for badges.
        /// </summary>
        protected override string BaseLocalizationRoot => "Badges/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// Image for this badge.
        /// </summary>
        [PreviewField(200)]
        public Sprite Image;
    }
}