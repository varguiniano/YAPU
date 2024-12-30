using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Base class for tabs shown in the single monster dex screen.
    /// </summary>
    public abstract class MonsterDexTab : HidableUiElement<MonsterDexTab>
    {
        /// <summary>
        /// Sprite for the title.
        /// </summary>
        [FoldoutGroup("TabConfig")]
        public Sprite TitleSprite;

        /// <summary>
        /// Show the tab when this monster has been seen but not caught?
        /// </summary>
        [FoldoutGroup("TabConfig")]
        public bool ShowWhenNotCaught;

        /// <summary>
        /// Show the info tip on the bottom bar?
        /// </summary>
        [FoldoutGroup("TabConfig")]
        public bool ShowInfoTip;

        /// <summary>
        /// Set the monster data in this tab.
        /// </summary>
        /// <param name="entry">Monster to display.</param>
        /// <param name="formEntry">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        /// <param name="playerCharacter">Player character reference.</param>
        public abstract void SetData(MonsterDexEntry entry, FormDexEntry formEntry, MonsterGender gender, PlayerCharacter playerCharacter);

        /// <summary>
        /// Called when the select button is pressed on the parent screen.
        /// </summary>
        public virtual void OnSelectPressedOnParentScreen()
        {
        }
    }
}