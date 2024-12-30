using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Controller for a location in the map screen.
    /// </summary>
    public class MapLocation : WhateverBehaviour<MapLocation>
    {
        /// <summary>
        /// First scene in this location.
        /// This is the one that will be used for naming.
        /// </summary>
        public SceneInfoAsset FirstScene => ScenesInLocation[0];

        /// <summary>
        /// Scenes that are in this location.
        /// </summary>
        [SerializeField]
        private List<SceneInfoAsset> ScenesInLocation;

        /// <summary>
        /// Reference to the fly icon.
        /// </summary>
        [SerializeField]
        private SpriteRenderer FlyIcon;

        /// <summary>
        /// Reference to the encounter icon.
        /// </summary>
        [SerializeField]
        private SpriteRenderer EncounterIcon;
        
        /// <summary>
        /// Reference to the objective icon.
        /// </summary>
        [SerializeField]
        private SpriteRenderer ObjectiveIcon;

        /// <summary>
        /// Can you fly to this location?
        /// </summary>
        public bool Flyable;

        /// <summary>
        /// Can the player fly here?
        /// </summary>
        public bool CanPlayerFlyHere
        {
            set => FlyIcon.enabled = value;
        }

        /// <summary>
        /// Does this location have an encounter?
        /// </summary>
        public bool HasEncounter
        {
            set => EncounterIcon.enabled = value;
        }

        /// <summary>
        /// Is the current quest objective in this location?
        /// </summary>
        public bool IsCurrentObjectiveHere
        {
            set => ObjectiveIcon.enabled = value;
        }

        /// <summary>
        /// Location to fly to.
        /// </summary>
        [ShowIf(nameof(Flyable))]
        public SceneLocation FlyLocation;

        /// <summary>
        /// Reference to the attached sprite Transform.
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
        /// Check if a scene is in this location.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>True if it is.</returns>
        public bool IsSceneInLocation(SceneInfoAsset scene) => ScenesInLocation.Contains(scene);
    }
}