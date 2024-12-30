using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for a damage move that hits harder if used multiple times in a row.
    /// </summary>
    public class IncrementalDamageMove : DamageMove
    {
        /// <summary>
        /// Multiplier to the power by the number of times the move has been used in a row.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        private float ComboMultiplier = 2f;

        /// <summary>
        /// Max power the move can have.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        private int MaxPower = 160;

        /// <summary>
        /// Data structure to store the times each battler has used this move in a row.
        /// </summary>
        private readonly Dictionary<Battler, int> timesUsed = new();

        /// <summary>
        /// Store the number of times this move has been used in a row per battler.
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
            if (timesUsed.ContainsKey(user)
             && user.LastPerformedAction.LastAction == BattleAction.Type.Move
             && user.LastPerformedAction.LastMoveSuccessful
             && user.LastPerformedAction.LastMove == this)
                timesUsed[user]++;
            else
                timesUsed[user] = 1;

            return base.ExecuteEffect(battleManager,
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
        }

        /// <summary>
        /// Multiply the power by the number of times used in a row.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int power = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (timesUsed.TryGetValue(user, out int times))
                power = Mathf.Clamp(Mathf.RoundToInt(power * times * ComboMultiplier), power, MaxPower);

            return power;
        }

        /// <summary>
        /// Called when the battle ends to clean up the turn used data.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            timesUsed.Clear();

            return base.OnBattleEnded(battler, battleManager);
        }
    }
}