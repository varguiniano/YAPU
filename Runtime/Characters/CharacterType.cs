using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Data class for a character type.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Characters/Type", fileName = "CharacterType")]
    public class CharacterType : WhateverScriptable<CharacterType>
    {
        /// <summary>
        /// Localizable name for this character type.
        /// </summary>
        [FoldoutGroup("Localization")]
        public string LocalizableName;

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        private void RefreshLocalizableNames() => LocalizableName = "Characters/Type/" + name;

        /// <summary>
        /// Base price money this trainer gives.
        /// </summary>
        [FoldoutGroup("Battle")]
        public int PriceMoney = 100;

        /// <summary>
        /// Does the player get a friendship boost when battling this character?
        /// </summary>
        [FoldoutGroup("Battle")]
        public bool FriendshipBoostWhenBattled;

        /// <summary>
        /// Front sprite of this character type.
        /// </summary>
        [PreviewField(100)]
        public Sprite FrontSprite;
        
        /// <summary>
        /// Animated back sprite of this character type.
        /// </summary>
        [PreviewField(100)]
        public Material BackSprite;
        
        /// <summary>
        /// World sprites for this character.
        /// </summary>
        public CharacterWorldSprites WorldSprites;

        /// <summary>
        /// Map icon for this character.
        /// </summary>
        [PreviewField]
        public Sprite MapIcon;
    }
}