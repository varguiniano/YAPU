using UnityEngine;
using UnityEngine.Tilemaps;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour that overrides the type of a tile by returning the type of the tile below it.
    /// </summary>
    public class TileTypeOfTileBelow : WhateverBehaviour<TileTypeOfTileBelow>, ITileTypeOverride
    {
        /// <summary>
        /// Get the type override.
        /// </summary>
        /// <param name="gridController">Reference to the Grid Controller of the tilemap the tile is in.</param>
        /// <param name="tilemap">Tilemap the tile is in.</param>
        /// <param name="tile">Reference to the actual tile.</param>
        /// <param name="tilePosition">Position of the tile.</param>
        /// <param name="sortOrder">Current sort order of the tile.</param>
        public TileType GetOverride(GridController gridController,
                                    Tilemap tilemap,
                                    TileBase tile,
                                    Vector3Int tilePosition,
                                    int sortOrder) =>
            gridController.GetTypeOfTileDirectlyBelowSortOrder(tilePosition, sortOrder);
    }
}