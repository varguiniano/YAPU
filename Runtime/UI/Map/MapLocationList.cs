using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// General container for all the map locations in the map screen.
    /// </summary>
    public class MapLocationList : WhateverBehaviour<MapLocationList>
    {
        /// <summary>
        /// List of all the map locations.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        public List<MapLocation> MapLocations { get; private set; }

        /// <summary>
        /// Retrieve all the map locations.
        /// </summary>
        private void OnEnable() => MapLocations = GetComponentsInChildren<MapLocation>().ToList();

        /// <summary>
        /// Find the location os a scene.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>The location, if found.</returns>
        public MapLocation FindLocation(SceneInfoAsset scene)
        {
            foreach (MapLocation mapLocation in MapLocations.Where(mapLocation => mapLocation.IsSceneInLocation(scene)))
                return mapLocation;

            Logger.Error("Scene not found on this map!");
            return null;
        }
    }
}