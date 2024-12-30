using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Saves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags
{
    /// <summary>
    /// Data class representing a bag that can be owned by the player or others.
    /// The bag contains all items they have.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Bag", fileName = "Bag")]
    public class Bag : SavableObject
    {
        /// <summary>
        /// Money this bag has.
        /// </summary>
        public uint Money;

        /// <summary>
        /// Dictionary that stores all items and the amount they have in the bag separated by category.
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private SerializableDictionary<Item, int> Items = new();

        /// <summary>
        /// Get the total number of available distinct items
        /// </summary>
        public int AvailableDistinctItems => Items.Count;

        /// <summary>
        /// Items registered so that they can be used in a shortcut menu.
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private List<Item> RegisteredItems = new();

        /// <summary>
        /// Does the bag contain the given item?
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if it does.</returns>
        public bool Contains(Item item) => Items.ContainsKey(item);

        /// <summary>
        /// Get the amount of an item.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Amount of that item.</returns>
        public int GetItemAmount(
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllItems))]
            #endif
            Item item)
        {
            if (Items.TryGetValue(item, out int amount)) return amount;

            Logger.Error("No such item in this bag.");
            return -1;
        }

        /// <summary>
        /// Change the amount of an item in the bag.
        /// This method adds or removes item from the bag as necessary.
        /// It doesn't care if they are stackable, that's only for UI visuals.
        /// </summary>
        /// <param name="item">Item to modify.</param>
        /// <param name="amount">Amount to modify.</param>
        /// <returns>-1  if item is null.
        /// 0  if item existed and has not been removed.
        /// 1 if the item was created.
        /// 2 if the item was removed.</returns>
        [Button]
        public int ChangeItemAmount(
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllItems))]
            #endif
            Item item,
            int amount)
        {
            if (item == null) return -1;

            int itemCreated = 0;

            if (Items.ContainsKey(item))
                Items[item] += amount;
            else
            {
                Items[item] = amount;
                itemCreated = 1;
            }

            if (Items[item] > 0) return itemCreated;

            DeregisterItem(item);
            Items.Remove(item);
            return 2;
        }

        /// <summary>
        /// Get the index of an item.
        /// </summary>
        /// <param name="item">Item to get the index of.</param>
        /// <returns>Its index.</returns>
        public int GetIndexOfItem(Item item)
        {
            int index = 0;

            foreach (KeyValuePair<Item, int> pair in Items)
            {
                if (pair.Key == item) return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Get an item by its index.
        /// </summary>
        /// <param name="index">Index of the item to get.</param>
        /// <returns>The item for that index.</returns>
        public Item GetItemFromIndex(int index)
        {
            int i = 0;

            foreach (KeyValuePair<Item, int> pair in Items)
            {
                if (i == index) return pair.Key;
                i++;
            }

            return null;
        }

        /// <summary>
        /// Get the registered items list.
        /// </summary>
        public List<Item> GetRegisteredItems() => RegisteredItems;

        /// <summary>
        /// Set an item as registered.
        /// </summary>
        /// <param name="item">Item to register.</param>
        public void RegisterItem(Item item)
        {
            if (GetItemAmount(item) == 0)
            {
                Logger.Error("This item is not on the bag.");
                return;
            }

            if (RegisteredItems.Contains(item))
            {
                Logger.Warn("This item is already registered.");
                return;
            }

            if (!item.CanBeRegistered)
            {
                Logger.Error("This item cannot be registered.");
                return;
            }

            RegisteredItems.Add(item);
        }

        /// <summary>
        /// Is an Item registered?
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if it is.</returns>
        public bool IsItemRegistered(Item item) => RegisteredItems.Contains(item);

        /// <summary>
        /// Remove a registered item from the registered list.
        /// </summary>
        /// <param name="item">Item to deregister.</param>
        public void DeregisterItem(Item item)
        {
            if (!RegisteredItems.Contains(item))
            {
                Logger.Warn("This item is not registered.");
                return;
            }

            RegisteredItems.Remove(item);
        }

        /// <summary>
        /// Get the items of a specific category.
        /// </summary>
        /// <param name="category">Category to get.</param>
        /// <returns>A dictionary of items and amounts.</returns>
        public Dictionary<Item, int> GetItemsForCategory(ItemCategory category)
        {
            Items.OnBeforeSerialize(); // We call this to update the serialized list and do linq with it.

            return Items.Where(pair => pair.Key.ItemCategory == category)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Get a list of all available item types in this bag.
        /// </summary>
        /// <returns>A list with all the item classes.</returns>
        public List<Item> GetAllAvailableItemTypes()
        {
            Items.OnBeforeSerialize(); // We call this to update the serialized list and do linq with it.

            return Items.Where(pair => pair.Value > 0)
                        .ToDictionary(pair => pair.Key, pair => pair.Value)
                        .Keys.ToList();
        }

        /// <summary>
        /// Randomly populate the bag for a battle.
        /// </summary>
        public void PopulateRandomlyForBattle(int maxNumberOfItemTypes,
                                              int maxNumberOfSingleItem,
                                              MonsterDatabaseInstance database)
        {
            Items.Clear();

            List<Item> itemPool = database.GetItems()
                                          .Where(item => item.CanBeUsedInBattle
                                                      || item.CanBeUsedInBattleOnTarget
                                                      || item.CanBeUsedInBattleOnTargetMove)
                                          .ToList();

            for (int i = 0; i < Random.Range(0, maxNumberOfItemTypes + 1); ++i)
            {
                Item candidate;

                do
                    candidate = itemPool.Random();
                while (Contains(candidate));

                ChangeItemAmount(candidate, Random.Range(1, maxNumberOfSingleItem + 1));
            }
        }

        /// <summary>
        /// Clone this bag.
        /// </summary>
        public Bag Clone()
        {
            Bag bag = CreateInstance<Bag>();

            bag.Money = Money;
            bag.Items = Items.ShallowClone();
            bag.RegisteredItems = RegisteredItems.ShallowClone();

            return bag;
        }

        /// <summary>
        /// Clone another roster and save it here.
        /// </summary>
        public void CopyFrom(Bag other)
        {
            if (other == null) return;

            Money = other.Money;
            Items = other.Items.ShallowClone();
            RegisteredItems = RegisteredItems.ShallowClone();

            #if UNITY_EDITOR
            if (Application.isPlaying) return;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            #endif
        }

        /// <summary>
        /// Save this data to a serialized string.
        /// </summary>
        /// <param name="serializer">String serializer to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>The serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableBag(this));

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings"></param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableBag readData = serializer.From<SavableBag>(data);

            yield return WaitAFrame;

            readData.LoadBag(this, monsterDatabase);
        }

        /// <summary>
        /// Reset the data to its default values.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            Money = 0;
            Items = new SerializableDictionary<Item, int>();
            RegisteredItems = new List<Item>();
            yield break;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Populate the bag with [number] of each item.
        /// <param name="number">Number of items of each type to add.</param>
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public void PopulateWithAllItems(int number = 100)
        {
            Items.Clear();
            RegisteredItems.Clear();

            foreach (Item item in GetAllItems()) ChangeItemAmount(item, number);
        }

        #endif

        /// <summary>
        /// Version of the bag class that can be serialized to a string.
        /// </summary>
        [Serializable]
        public class SavableBag
        {
            /// <summary>
            /// Money.
            /// </summary>
            public uint Money;

            /// <summary>
            /// Items.
            /// </summary>
            public SerializableDictionary<int, int> Items;

            /// <summary>
            /// Registered items.
            /// </summary>
            public List<int> RegisteredItems;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="bag">Original bag.</param>
            public SavableBag(Bag bag)
            {
                Money = bag.Money;

                Items = new SerializableDictionary<int, int>();

                foreach (KeyValuePair<Item, int> pair in bag.Items) Items[pair.Key.name.GetHashCode()] = pair.Value;

                RegisteredItems = bag.RegisteredItems.Select(item => item.name.GetHashCode()).ToList();
            }

            /// <summary>
            /// Load back the data into the bag.
            /// </summary>
            /// <param name="bag">Bag to load the data back into.</param>
            /// <param name="database">Database reference.</param>
            public void LoadBag(Bag bag, MonsterDatabaseInstance database)
            {
                bag.Money = Money;

                bag.Items = new SerializableDictionary<Item, int>();

                foreach (KeyValuePair<int, int> pair in Items) bag.Items[database.GetItemByHash(pair.Key)] = pair.Value;

                bag.RegisteredItems = RegisteredItems.Select(database.GetItemByHash).ToList();
            }
        }
    }
}