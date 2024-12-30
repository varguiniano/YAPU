using Sirenix.OdinInspector;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Characters
{
    /// <summary>
    /// Controller for a behaviour that displays a character world sprite.
    /// </summary>
    public class CharacterWorldIcon : WhateverBehaviour<CharacterWorldIcon>
    {
        /// <summary>
        /// Set the icon from a character.
        /// </summary>
        /// <param name="character">Character to set.</param>
        [Button]
        public void SetIcon(CharacterData character) =>
            GetCachedComponent<Image>().sprite =
                character.GetLooking(CharacterController.Direction.Down, false, false, false, false);
    }
}