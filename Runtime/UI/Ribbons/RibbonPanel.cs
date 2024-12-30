using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Ribbons
{
    /// <summary>
    /// Panel to show a ribbon on the UI.
    /// </summary>
    public class RibbonPanel : WhateverBehaviour<RibbonPanel>
    {
        /// <summary>
        /// Set the ribbon to show.
        /// </summary>
        /// <param name="ribbon">Ribbon to show.</param>
        public void SetRibbon(Ribbon ribbon) => GetCachedComponent<Image>().sprite = ribbon.Sprite;
    }
}