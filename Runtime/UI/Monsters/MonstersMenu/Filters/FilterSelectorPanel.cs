using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for the panel that shows filter selectors.
    /// </summary>
    public class FilterSelectorPanel : WhateverBehaviour<FilterSelectorPanel>
    {
        /// <summary>
        /// Menu open position.
        /// </summary>
        [SerializeField]
        private Transform OpenPosition;

        /// <summary>
        /// Menu closed position.
        /// </summary>
        [SerializeField]
        private Transform ClosedPosition;

        /// <summary>
        /// Cached reference to the attached transform.
        /// </summary>
        private Transform Panel
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Panel.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Set the data.
        /// </summary>
        private void OnEnable() => Panel.localPosition = ClosedPosition.localPosition;

        /// <summary>
        /// Slide the panel in.
        /// </summary>
        public IEnumerator SlideIn()
        {
            yield return Panel.DOLocalMove(OpenPosition.localPosition, .25f)
                              .SetEase(Ease.OutBack)
                              .WaitForCompletion();
        }

        /// <summary>
        /// Slide the panel out.
        /// </summary>
        public IEnumerator SlideOut()
        {
            yield return Panel.DOLocalMove(ClosedPosition.localPosition, .25f).SetEase(Ease.InBack).WaitForCompletion();
        }
    }
}