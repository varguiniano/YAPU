using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Base class for actors that are represented by an item.
    /// </summary>
    public class ActorItem : GridSubscribingActor
    {
        /// <summary>
        /// Block movement only if it's not a hidden item.
        /// </summary>
        public override bool BlocksMovement => !HiddenItem;

        /// <summary>
        /// Item represented by this actor.
        /// </summary>
        [PropertyOrder(-.5f)]
        [OnValueChanged(nameof(OnItemUpdated))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        public Item Item;

        /// <summary>
        /// Is the item invisible?
        /// </summary>
        [PropertyOrder(-.5f)]
        [Tooltip("Is the item invisible?")]
        [OnValueChanged(nameof(OnItemUpdated))]
        [SerializeField]
        private bool HiddenItem;

        /// <summary>
        /// Reference to the attached sprite.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(Sprite) + " == null")]
        private SpriteRenderer Sprite;

        /// <summary>
        /// Reference to the attached shadow sprite.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(Shadow) + " == null")]
        private SpriteRenderer Shadow;

        /// <summary>
        /// Called when the item is updated.
        /// Only to be used in editor.
        /// </summary>
        private void OnItemUpdated()
        {
            Sprite.sprite = Item == null || HiddenItem ? null : Item.Icon;
            Shadow.enabled = Sprite.sprite != null;
        }

        /// <summary>
        /// Update the name of the object.
        /// Only to be used in editor.
        /// </summary>
        [Button]
        [ShowIf("@" + nameof(Item) + " != null")]
        [PropertyOrder(-1)]
        private void UpdateName() => name = Item.name;

        /// <summary>
        /// Enable or disable the sprite.
        /// </summary>
        /// <param name="enable">Enable or disable?</param>
        public void EnableSprite(bool enable = true) => Sprite.enabled = enable;

        #if UNITY_EDITOR
        /// <summary>
        /// Get all the items in the project.
        /// </summary>
        private List<Item> GetAllItems() => MonsterDatabaseData.GetAllItems();
        #endif
    }
}