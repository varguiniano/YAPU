using System;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Behaviour that detects when the map cursor enters a location.
    /// </summary>
    public class MapCursorLocationDetector : WhateverBehaviour<MapCursorLocationDetector>
    {
        /// <summary>
        /// Event raised when a location is entered.
        /// </summary>
        public Action<MapLocation> EnteredLocation;

        /// <summary>
        /// Event raised when a location is entered.
        /// </summary>
        public Action<MapLocation> ExitedLocation;

        /// <summary>
        /// Called when we enter another collider.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            MapLocation mapLocation = col.GetComponent<MapLocation>();

            if (mapLocation != null) EnteredLocation?.Invoke(mapLocation);
        }

        /// <summary>
        /// Called when we exit another collider.
        /// </summary>
        private void OnTriggerExit2D(Collider2D col)
        {
            MapLocation mapLocation = col.GetComponent<MapLocation>();

            if (mapLocation != null) ExitedLocation?.Invoke(mapLocation);
        }
    }
}