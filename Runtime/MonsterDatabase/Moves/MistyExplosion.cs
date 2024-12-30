using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move MistyExplosion.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fairy/MistyExplosion", fileName = "MistyExplosion")]
    public class MistyExplosion : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Terrain that boosts this move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        private Terrain BoostingTerrain;

        /// <summary>
        /// Multiplier the terrain applies.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float TerrainMultiplier = 1.5f;

        /// <summary>
        /// Faint the target after using the move.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);

            yield return battleManager.BattlerHealth.ChangeLife(user, user, -(int)user.CurrentHP, this);
        }

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int power = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (battleManager.Scenario.Terrain == BoostingTerrain
             && battleManager.Scenario.Terrain.IsAffected(user, battleManager))
                power = Mathf.RoundToInt(power * TerrainMultiplier);

            return power;
        }
    }
}