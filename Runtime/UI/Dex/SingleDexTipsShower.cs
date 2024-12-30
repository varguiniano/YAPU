using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Behaviour in charge of showing the tips for the general single dex screen or the ones for a submenu.
    /// </summary>
    public class SingleDexTipsShower : WhateverBehaviour<SingleDexTipsShower>
    {
        /// <summary>
        /// Reference to the general tips.
        /// </summary>
        [SerializeField]
        private HidableUiElement GeneralTips;

        /// <summary>
        /// Reference to the submenu tips.
        /// </summary>
        [SerializeField]
        private HidableUiElement SubmenuTips;

        /// <summary>
        /// Reference to the submenu tips.
        /// </summary>
        [SerializeField]
        private HidableUiElement CustomTip;

        /// <summary>
        /// Reference to the text for a custom tip.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro CustomTipText;

        /// <summary>
        /// Start with the general tips.
        /// </summary>
        private void OnEnable() => SwitchToGeneral();

        /// <summary>
        /// Switch to the general tips.
        /// </summary>
        public void SwitchToGeneral()
        {
            GeneralTips.Show();
            SubmenuTips.Show(false);
            CustomTip.Show(false);
        }

        /// <summary>
        /// Switch to the submenu tips.
        /// </summary>
        public void SwitchToSubmenu(bool showCustom = false, string localizableKey = "")
        {
            GeneralTips.Show(false);
            SubmenuTips.Show();
            CustomTipText.SetValue(localizableKey);
            CustomTip.Show(showCustom);
        }
    }
}