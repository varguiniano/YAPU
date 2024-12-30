using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;

#if UNITY_EDITOR
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Data behaviour that contains information about a tile.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/TileData", fileName = "TileData")]
    public class TileData : WhateverScriptable<TileData>
    {
        /// <summary>
        /// Type of this tile.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<TileBase, TileType> Types;

        /// <summary>
        /// Count of the types in the database.
        /// </summary>
        public int TypeCount => Types.Count;

        /// <summary>
        /// Get the type of a tile.
        /// </summary>
        /// <param name="key">Tile to check.</param>
        public TileType this[TileBase key] => Types[key];

        #if UNITY_EDITOR

        /// <summary>
        /// Add a group of tiles of the same type.
        /// </summary>
        /// <param name="tiles">Tiles to add.</param>
        /// <param name="type">Type to add.</param>
        [Button]
        [HideInPlayMode]
        private void AddGroup(List<TileBase> tiles, TileType type)
        {
            foreach (TileBase tile in tiles.Where(tile => !Types.ContainsKey(tile))) Types[tile] = type;
        }

        /// <summary>
        /// Populate with the missing tiles.
        /// </summary>
        [FoldoutGroup("Utils")]
        [Button]
        [HideInPlayMode]
        private void Populate()
        {
            foreach (TileBase tileBase in AssetManagementUtils.FindAssetsByType<TileBase>()
                                                              .Where(tileBase => !Types.ContainsKey(tileBase)))
                Types[tileBase] = TileType.NonExistent;
        }

        /// <summary>
        /// Remove the colliders from all tiles.
        /// </summary>
        [FoldoutGroup("Utils")]
        [Button]
        [HideInPlayMode]
        private void RemoveColliders()
        {
            foreach (TileBase tileBase in Types.Keys)
            {
                RuleTile tile = tileBase as RuleTile;

                if (tile == null) continue;

                bool dirty = false;

                if (tile.m_DefaultColliderType != Tile.ColliderType.None)
                {
                    tile.m_DefaultColliderType = Tile.ColliderType.None;
                    dirty = true;
                }

                foreach (RuleTile.TilingRule rule in tile.m_TilingRules.Where(rule => rule.m_ColliderType
                                                                               != Tile.ColliderType.None))
                {
                    rule.m_ColliderType = Tile.ColliderType.None;
                    dirty = true;
                }

                if (dirty) EditorUtility.SetDirty(tile);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Populate with the missing tiles.
        /// </summary>
        [FoldoutGroup("Utils")]
        [Button]
        [HideInPlayMode]
        private void CheckInvalid()
        {
            foreach (KeyValuePair<TileBase, TileType> pair in Types)
                if (pair.Value == TileType.NonExistent)
                    Logger.Error(pair.Key.name + " is marked as non existent!");
        }

        /// <summary>
        /// Populate with the missing tiles.
        /// </summary>
        [FoldoutGroup("Utils")]
        [Button]
        [HideInPlayMode]
        private void RemoveNulls()
        {
            Types.SerializedList = Types.Where(pair => pair.Key != null).ToList();
            Types.OnAfterDeserialize();
        }

        #endif
    }
}