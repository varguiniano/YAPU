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
    /// Data class for a move that sets a terrain.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/SetTerrainMove", fileName = "SetTerrainMove")]
    public class SetTerrainMove : Move
    {
        /// <summary>
        /// Reference to the terrain the move will set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        private Terrain TerrainToSet;

        /// <summary>
        /// Countdown for the Terrain to last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Countdown;

        /// <summary>
        /// No need for animation since the terrain will have an entry animation.
        /// </summary>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            yield break;
        }

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
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
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            int customCountdown = battler.CanUseHeldItemInBattle(battleManager)
                                      ? battler.HeldItem.CalculateTerrainDuration(battler, TerrainToSet, battleManager)
                                      : -2;

            yield return
                battleManager.Scenario.SetTerrain(TerrainToSet,
                                                  customCountdown != -2
                                                      ? customCountdown
                                                      : Countdown);

            finishedCallback.Invoke(true);
        }
    }
}