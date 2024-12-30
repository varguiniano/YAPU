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
    /// Data class for Present.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Present", fileName = "Present")]
    public class Present : Move
    {
        /// <summary>
        /// Regen move to use.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private HPPercentageRegenMove RegenMove;

        /// <summary>
        /// Damage move to use.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private DamageMove DamageMove;

        /// <summary>
        /// Last present roll.
        /// </summary>
        private float lastRoll;

        /// <summary>
        /// Roll the chance to be damage or hit, as well as the multiplier for the power.
        /// This roll is stored for the execution.
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
            lastRoll = battleManager.RandomProvider.Value01();

            if (lastRoll < .2f)
                yield return RegenMove.PlayAnimation(battleManager,
                                                     speed,
                                                     userType,
                                                     userIndex,
                                                     user,
                                                     userPosition,
                                                     targets,
                                                     targetPositions,
                                                     ignoresAbilities);
            else
                yield return DamageMove.PlayAnimation(battleManager,
                                                      speed,
                                                      userType,
                                                      userIndex,
                                                      user,
                                                      userPosition,
                                                      targets,
                                                      targetPositions,
                                                      ignoresAbilities);
        }

        /// <summary>
        /// Execute the effect of the move.
        /// Calculate if it should heal and if not, calculate a multiplier for the power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected hits of this move.</param>
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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                switch (lastRoll)
                {
                    case < .2f:
                    {
                        yield return RegenMove.ExecuteEffect(battleManager,
                                                             localizer,
                                                             userType,
                                                             userIndex,
                                                             user,
                                                             new List<(BattlerType Type, int Index)> { (targetType, targetIndex) },
                                                             hitNumber,
                                                             expectedHits,
                                                             externalPowerMultiplier,
                                                             ignoresAbilities,
                                                             finishedCallback);

                        finishedCallback.Invoke(true);
                        yield break;
                    }
                    case < .3f:
                        externalPowerMultiplier *= 3;
                        break;
                    case < .6f:
                        externalPowerMultiplier *= 2;
                        break;
                }

                yield return DamageMove.ExecuteEffect(battleManager,
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
        }
    }
}