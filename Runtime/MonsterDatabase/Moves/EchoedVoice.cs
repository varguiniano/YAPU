using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move EchoedVoice.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/EchoedVoice", fileName = "EchoedVoice")]
    public class EchoedVoice : DamageMove
    {
        /// <summary>
        /// Each time it's used consecutively, the power multiplier increases by this amount.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        private float ConsecutiveUseMultiplierIncrement = 1;

        /// <summary>
        /// Maximum power.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        private int PowerCap = 200;

        /// <summary>
        /// Last it was used in the current battle.
        /// -1 if it hasn't been used yet.
        /// </summary>
        private int lastTurnUsed = -1;

        /// <summary>
        /// Current multiplier for the power.
        /// </summary>
        private float currentPowerMultiplier = 1;

        /// <summary>
        /// Increment after using if it was used last turn and update the variables.
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

            if (WasUsedLastTurn(battleManager)) currentPowerMultiplier += ConsecutiveUseMultiplierIncrement;
            lastTurnUsed = battleManager.TurnCounter;
        }

        /// <summary>
        /// Get the move power in battle.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            // We add one increment because this way it will show correctly in the UI.
            // To prevent more power when actually used, the multiplier will increment after using the move.
            float multiplier = WasUsedLastTurn(battleManager)
                                   ? currentPowerMultiplier + ConsecutiveUseMultiplierIncrement
                                   : 1;

            return Mathf.Clamp(Mathf.RoundToInt(base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
                                              * multiplier),
                               1,
                               PowerCap);
        }

        /// <summary>
        /// Reset the last turn and the power multiplier.
        /// </summary>
        /// <param name="battler"></param>
        /// <param name="battleManager"></param>
        /// <returns></returns>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            yield return base.OnBattleEnded(battler, battleManager);
            lastTurnUsed = -1;
            currentPowerMultiplier = 1;
        }

        /// <summary>
        /// Was the move used last turn?
        /// </summary>
        private bool WasUsedLastTurn(BattleManager battleManager) =>
            lastTurnUsed >= 0 && battleManager.TurnCounter == lastTurnUsed + 1;
    }
}