using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Behaviour in charge of blinking a map element.
    /// </summary>
    public class BlinkMapElement : WhateverBehaviour<BlinkMapElement>
    {
        /// <summary>
        /// Start the blinking.
        /// </summary>
        private void OnEnable() => StartCoroutine(BlinkRoutine());

        /// <summary>
        /// Routine to keep blinking the element.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlinkRoutine()
        {
            Color color = GetCachedComponent<SpriteRenderer>().color;

            while (true)
            {
                color.a = 255;
                GetCachedComponent<SpriteRenderer>().color = color;

                yield return new WaitForSeconds(1f);

                color.a = 0;
                GetCachedComponent<SpriteRenderer>().color = color;

                yield return new WaitForSeconds(1f);
            }
        }
    }
}