using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle
{
    /// <summary>
    /// Data class representing a repel item effect.
    /// This has no repel info, that is stored in the actual repel object.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OutOfBattle/UseRepel",
                     fileName = "UseRepel")]
    [InfoBox("This has no repel info, that is stored in the actual repel object.")]
    public class UseRepel : UseOutOfBattleItemEffect
    {
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
            Repel repel = item as Repel;

            if (repel == null) yield break;

            yield return playerCharacter.AddRepelSteps(repel);

            finished.Invoke(true);
        }
    }
}