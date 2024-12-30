using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Menu selector that has scrolling.
    /// </summary>
    public class ScrollableMenuSelector : MenuSelector
    {
        /// <summary>
        /// Reference to the scroll.
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        private ScrollRect Scroll;
        
        /// <summary>
        /// Reorder stuff to first scroll and then update the arrow position.
        /// </summary>
        /// <param name="index">Index of the item to select.</param>
        /// <param name="playAudio">Play the navigation audio?</param>
        /// <param name="updateArrow">Should update the arrow when selecting?</param>
        /// <param name="force">Force reselection?</param>
        public override void Select(int index, bool playAudio = true, bool updateArrow = true, bool force = false)
        {
            base.Select(index, playAudio, false, force);

            UpdateScroll();

            UpdateSelectorArrowPosition();
        }
        
        /// <summary>
        /// Update the scroll position based on the current button.
        /// </summary>
        private void UpdateScroll()
        {
            if (MenuOptions.Count < 2) return;
            Scroll.verticalNormalizedPosition = 1 - (float)CurrentSelection / (MenuOptions.Count - 1);
        }
    }
}