using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Class representing a summary tab.
    /// </summary>
    public abstract class SummaryTab : HidableUiElement<SummaryTab>
    {
        /// <summary>
        /// Use a sprite in the tab title instead of a text.
        /// </summary>
        [FoldoutGroup("TabConfig")]
        public bool UseSpriteInsteadOfText;

        /// <summary>
        /// Localizable key for the title.
        /// </summary>
        [FoldoutGroup("TabConfig")]
        [HideIf(nameof(UseSpriteInsteadOfText))]
        public string TitleLocalizableKey;

        /// <summary>
        /// Sprite for the title.
        /// </summary>
        [FoldoutGroup("TabConfig")]
        [ShowIf(nameof(UseSpriteInsteadOfText))]
        public Sprite TitleSprite;

        /// <summary>
        /// Used to mark if the tab has a submenu.
        /// </summary>
        [FoldoutGroup("TabConfig")]
        public bool HasSubMenu;

        /// <summary>
        /// Set the monster data in this tab.
        /// </summary>
        /// <param name="monster">Monster reference.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public abstract void SetData(MonsterInstance monster, BattleManager battleManager);

        /// <summary>
        /// Called when the submenu is entered.
        /// </summary>
        public virtual void EnterSubmenu()
        {
        }
    }
}