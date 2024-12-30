using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Controller for the player icon displayed on the map.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMapIcon : WhateverBehaviour<PlayerMapIcon>
    {
        /// <summary>
        /// Set the player from their character data and their map location.
        /// </summary>
        /// <param name="characterData">Player character data.</param>
        /// <param name="mapLocation">Map location to place on.</param>
        public void SetPlayer(CharacterData characterData, MapLocation mapLocation)
        {
            if (mapLocation == null)
            {
                GetCachedComponent<Transform>().position = new Vector3(0, 10, 0);
                return;
            }

            GetCachedComponent<SpriteRenderer>().sprite = characterData.CharacterType.MapIcon;
            GetCachedComponent<Transform>().position = mapLocation.Transform.position;
        }
    }
}