using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour attached to the edge of a waterfall, be it top or bottom.
    /// </summary>
    public class WaterfallEdge : WhateverBehaviour<WaterfallEdge>
    {
        /// <summary>
        /// Is this edge top or bottom?
        /// </summary>
        public EdgeType EdgeType;
    }

    /// <summary>
    /// Types of waterfall edges.
    /// </summary>
    public enum EdgeType
    {
        Top,
        Bottom
    }
}