using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Map;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Data class for a region asset.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/Region", fileName = "Region")]
    public class Region : WhateverScriptable<Region>
    {
        /// <summary>
        /// Localizable string for the region name.
        /// </summary>
        public string LocalizableName;

        /// <summary>
        /// Reference to the sprite that holds this map.
        /// </summary>
        [FoldoutGroup("Map")]
        public Sprite MapSprite;

        /// <summary>
        /// Location list to use when opening the map screen.
        /// </summary>
        [FoldoutGroup("Map")]
        public MapLocationList MapLocationList;

        /// <summary>
        /// List of moves that are locked by more than one badge.
        /// </summary>
        [FoldoutGroup("Badges")]
        [ReadOnly]
        [ShowIf("@" + nameof(DupedLockedMoves) + ".Count > 0")]
        [InfoBox("The following moves are locked by more than one badge. This is not supported.",
                 InfoMessageType.Error)]
        [SerializeField]
        private List<Move> DupedLockedMoves = new();

        /// <summary>
        /// Dictionary holding the badges and the moves they lock.
        /// </summary>
        [FoldoutGroup("Badges")]
        [OnCollectionChanged(nameof(ValidateLockedMoves))]
        public SerializableDictionary<Badge, List<Move>> Badges;

        /// <summary>
        /// Check if a move is locked by a badge in this region.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <param name="badge">Badge locking the move.</param>
        /// <returns>True if it is locked.</returns>
        public bool IsMoveLockedByBadge(Move move, out Badge badge)
        {
            badge = null;

            foreach (KeyValuePair<Badge, List<Move>> candidate in Badges)
                if (candidate.Value.Contains(move))
                {
                    badge = candidate.Key;
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Initialize the editor inspector.
        /// </summary>
        [OnInspectorInit]
        private void InitInspector() => ValidateLockedMoves();

        /// <summary>
        /// Validate the locked moves.
        /// </summary>
        [FoldoutGroup("Badges")]
        [Button]
        private void ValidateLockedMoves()
        {
            List<Move> checkedMoves = new();
            DupedLockedMoves = new List<Move>();

            foreach (KeyValuePair<Badge, List<Move>> pair in Badges)
            {
                foreach (Move move in pair.Value)
                    if (checkedMoves.Contains(move))
                        DupedLockedMoves.Add(move);
                    else
                        checkedMoves.Add(move);
            }
        }
    }
}