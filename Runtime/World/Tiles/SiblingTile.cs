using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Rule tile for a rule tile with siblings that connect to it.
    /// Based on the Unity docs: https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.6/manual/CustomRulesForRuleTile.html
    /// </summary>
    [CreateAssetMenu(menuName = "2D/Tiles/SiblingTile", fileName = "SiblingTile")]
    public class SiblingTile : RuleTile<SiblingTile.Neighbor>
    {
        /// <summary>
        /// Siblings of this tile.
        /// </summary>
        [SerializeField]
        private List<TileBase> Siblings = new();

        /// <summary>
        /// Does the default green arrow include the sibling?
        /// </summary>
        [SerializeField]
        private bool DefaultIncludesSibling = true;

        /// <summary>
        /// Class representing a neighbour.
        /// </summary>
        // ReSharper disable once AccessToStaticMemberViaDerivedType
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Sibling = 3;
        }

        /// <summary>
        /// Used to check if a neighbour matches a sibling.
        /// </summary>
        /// <param name="neighbor">Neighbour type.</param>
        /// <param name="tile">Neighbour tile.</param>
        /// <returns>True if it matches.</returns>
        public override bool RuleMatch(int neighbor, TileBase tile) =>
            neighbor switch
            {
                Neighbor.Sibling => Siblings.Contains(tile),
                2 => DefaultIncludesSibling && !Siblings.Contains(tile) && base.RuleMatch(neighbor, tile),
                _ => (DefaultIncludesSibling && Siblings.Contains(tile)) || base.RuleMatch(neighbor, tile)
            };
    }
}