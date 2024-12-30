using System;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Saves;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Class to store a location data in a scene.
    /// </summary>
    [Serializable]
    public class SceneLocation
    {
        /// <summary>
        /// Scene in which to locate.
        /// </summary>
        public SceneInfoAsset Scene;

        /// <summary>
        /// Location inside the scene.
        /// </summary>
        public Vector2Int Location;
    }

    /// <summary>
    /// Version of the scene location class that can be serialized to a string.
    /// </summary>
    [Serializable]
    public class SavableSceneLocation
    {
        /// <summary>
        /// Scene reference.
        /// </summary>
        public string Scene;

        /// <summary>
        /// Location inside the scene.
        /// </summary>
        public Vector2Int Location;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sceneLocation">Original scene location.</param>
        public SavableSceneLocation(SceneLocation sceneLocation)
        {
            Scene = sceneLocation.Scene.name;
            Location = sceneLocation.Location;
        }

        /// <summary>
        /// Transform back to a scene location.
        /// </summary>
        /// <param name="worldDatabase">Reference to the world database.</param>
        /// <returns>The scene location.</returns>
        public SceneLocation ToSceneLocation(WorldDatabase worldDatabase) =>
            new()
            {
                Scene = worldDatabase.GetSceneByName(Scene),
                Location = Location
            };
    }
}