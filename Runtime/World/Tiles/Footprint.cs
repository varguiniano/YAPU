using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour that shows a footprint on a tile.
    /// </summary>
    public class Footprint : WhateverBehaviour<Footprint>
    {
        /// <summary>
        /// Sprites to use per direction.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<CharacterController.Direction, Sprite> SpritesPerDirection;

        /// <summary>
        /// Seconds until it fades.
        /// </summary>
        [SerializeField]
        private float FadeTime;

        /// <summary>
        /// Max alpha for the footprints to have.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float MaxAlpha;

        /// <summary>
        /// Set the alpha to 0 on enable.
        /// </summary>
        private void OnEnable() => GetCachedComponent<SpriteRenderer>().DOFade(0, 0);

        /// <summary>
        /// Stop all routines.
        /// </summary>
        private void OnDisable()
        {
            StopAllCoroutines();

            GetCachedComponent<SpriteRenderer>().DOKill();
        }

        /// <summary>
        /// Show a footprint and fade after a while.
        /// </summary>
        public void ShowFootprint(CharacterController.Direction direction) =>
            StartCoroutine(ShowFootprintRoutine(direction));

        /// <summary>
        /// Show a footprint and fade after a while.
        /// </summary>
        private IEnumerator ShowFootprintRoutine(CharacterController.Direction direction)
        {
            GetCachedComponent<SpriteRenderer>().sprite = SpritesPerDirection[direction];

            yield return GetCachedComponent<SpriteRenderer>().DOFade(MaxAlpha, 0).WaitForCompletion();

            yield return GetCachedComponent<SpriteRenderer>().DOFade(0, FadeTime).WaitForCompletion();

            Destroy(gameObject);
        }
    }
}