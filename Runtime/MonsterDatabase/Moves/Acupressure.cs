using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Acupressure.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Acupressure", fileName = "Acupressure")]
    public class Acupressure : Move
    {
        /// <summary>
        /// Chance of changing each stat.
        /// </summary>
        private const float chance = 1 / 7f;

        /// <summary>
        /// Change the stages.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="user"></param>
        /// <param name="targets">Move targets.</param>
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
            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                bool changeBattleStat = false;

                Stat statToChange = Stat.Attack;
                BattleStat battleStatToChange = BattleStat.Evasion;

                switch (battleManager.RandomProvider.Value01())
                {
                    case < chance:
                        statToChange = Stat.Attack;
                        break;
                    case < chance * 2:
                        statToChange = Stat.Defense;
                        break;
                    case < chance * 3:
                        statToChange = Stat.SpecialAttack;
                        break;
                    case < chance * 4:
                        statToChange = Stat.SpecialDefense;
                        break;
                    case < chance * 5:
                        statToChange = Stat.Speed;
                        break;
                    case < chance * 6:
                        battleStatToChange = BattleStat.Accuracy;
                        changeBattleStat = true;
                        break;
                    default:
                        battleStatToChange = BattleStat.Evasion;
                        changeBattleStat = true;
                        break;
                }

                if (changeBattleStat)
                    yield return battleManager.BattlerStats.ChangeStatStage(target,
                                                                            battleStatToChange,
                                                                            2,
                                                                            user,
                                                                            ignoreAbilities: ignoresAbilities);
                else
                    yield return battleManager.BattlerStats.ChangeStatStage(target,
                                                                            statToChange,
                                                                            2,
                                                                            user,
                                                                            ignoreAbilities: ignoresAbilities);
            }
        }
    }
}