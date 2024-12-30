using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects
{
    /// <summary>
    /// Item effect to fish.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OutOfBattle/FishingEffect", fileName = "FishingEffect")]
    public class FishingEffect : UseOutOfBattleItemEffect
    {
        /// <summary>
        /// Level of the fishing rod to use.
        /// </summary>
        [PropertyRange(0, 2)]
        [SerializeField]
        private byte RodLevel;

        /// <summary>
        /// Use out of battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        PlayerSettings playerSettings,
                                        PlayerCharacter playerCharacter,
                                        ILocalizer localizer,
                                        Action<bool> finished)
        {
            yield return DialogManager.CloseMenus();

            yield return playerCharacter.Fish(RodLevel);
        }
    }
}