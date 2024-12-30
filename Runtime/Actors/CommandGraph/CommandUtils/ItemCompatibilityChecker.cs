using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that can check the compatibility of an item on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/ItemCompatibility/AlwaysCompatible", fileName = "AlwaysCompatible")]
    public class ItemCompatibilityChecker : MonsterDatabaseScriptable<ItemCompatibilityChecker>
    {
        /// <summary>
        /// Allow choosing medicine.
        /// </summary>
        public bool AllowMedicine = true;

        /// <summary>
        /// Allow choosing balls.
        /// </summary>
        public bool AllowBalls = true;

        /// <summary>
        /// Allow choosing battle items.
        /// </summary>
        public bool AllowBattleItems = true;

        /// <summary>
        /// Allow choosing berries.
        /// </summary>
        public bool AllowBerries = true;

        /// <summary>
        /// Allow choosing other items.
        /// </summary>
        public bool AllowOther = true;

        /// <summary>
        /// Allow choosing TMs.
        /// </summary>
        public bool AllowMoveMachines = true;

        /// <summary>
        /// Allow choosing treasure.
        /// </summary>
        public bool AllowTreasure = true;

        /// <summary>
        /// Allow choosing crafting items.
        /// </summary>
        public bool AllowCrafting = true;

        /// <summary>
        /// Allow choosing key items.
        /// </summary>
        public bool AllowKey = true;

        /// <summary>
        /// Get the array of tabs that should be shown.
        /// </summary>
        public bool[] TabFilter =>
            new[]
            {
                AllowMedicine,
                AllowBalls,
                AllowBattleItems,
                AllowBerries,
                AllowOther,
                AllowMoveMachines,
                AllowTreasure,
                AllowCrafting,
                AllowKey
            };

        /// <summary>
        /// Check if an item is compatible.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if it is compatible.</returns>
        public virtual bool IsItemCompatible(Item item) => true;
    }
}