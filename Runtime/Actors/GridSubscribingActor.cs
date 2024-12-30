using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.World;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Base class for actors that subscribe themselves to the grid.
    /// </summary>
    public abstract class GridSubscribingActor : Actor
    {
        /// <summary>
        /// Normally actors that subscribe to the grid will block movement.
        /// </summary>
        public override bool BlocksMovement => true;

        /// <summary>
        /// Reference to the current grid.
        /// </summary>
        [HideInInspector]
        public GridController CurrentGrid;

        /// <summary>
        /// Cached reference to the attached transform.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Subscribe to the grid.
        /// </summary>
        protected override void OnEnable()
        {
            CurrentGrid = FindObjectsByType<GridController>(FindObjectsSortMode.None)
               .First(grid => grid.gameObject.scene == gameObject.scene);

            CurrentGrid.ActorEnterGrid(this);

            base.OnEnable();
        }

        /// <summary>
        /// Exit the grid.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            CurrentGrid.ActorExitGrid(this);
        }

        /// <summary>
        /// Get the current grid.
        /// </summary>
        public override GridController GetCurrentGrid() => CurrentGrid;
    }
}