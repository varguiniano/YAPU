using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Behaviour to control the icon of the move category.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MoveCategoryIcon : WhateverBehaviour<MoveCategoryIcon>
    {
        /// <summary>
        /// Sprite corresponding to each category.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Move.Category, Sprite> SpritePerCategory;

        /// <summary>
        /// Set the icon corresponding to that category.
        /// </summary>
        /// <param name="category">Category to set.</param>
        [Button("Set Icon")]
        public void SetIcon(Move.Category category) => GetCachedComponent<Image>().sprite = SpritePerCategory[category];
    }
}