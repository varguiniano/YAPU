﻿using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that retriggers the item's terrain set callback when hit by a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/RetriggerOnTerrainSetAfterHit",
                     fileName = "RetriggerOnTerrainSetAfterHit")]
    public class RetriggerOnTerrainSetAfterHit : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Move user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHitByMove(Item item,
                                                   DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            yield return item.OnTerrainSet(battleManager, battleManager.Scenario.Terrain, battler, finished);
        }
    }
}