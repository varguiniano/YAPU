using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move ShedTail.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/ShedTail", fileName = "ShedTail")]
    public class ShedTail : Substitute
    {
        /// <summary>
        /// Setup the substitute and switch.
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

            uint maxHP = MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager);

            uint hpCost = (uint) (maxHP * HPCost);

            yield return battleManager.BattlerHealth.ChangeLife(userType,
                                                                userIndex,
                                                                userType,
                                                                userIndex,
                                                                -(int) hpCost,
                                                                playAudio: false);

            battler.Substitute.MaxHP = hpCost;
            battler.Substitute.CurrentHP = hpCost;
            battler.Substitute.SubstituteEnabled = true;

            SubstituteData substituteData = user.Substitute;

            yield return battleManager.GetMonsterSprite(userType, userIndex).HideSubstitute(battleManager);

            Battler newMonster = null;

            yield return battleManager.BattleManagerBattlerSwitch.ForceSwitchBattler(userType,
                     userIndex,
                     userType,
                     userIndex,
                     this,
                     null,
                     false,
                     ignoresAbilities,
                     true,
                     index =>
                     {
                         newMonster = battleManager.Battlers.GetBattlerFromBattleIndex(userType, index);
                     });

            yield return DialogManager.WaitForDialog;

            if (newMonster == null)
            {
                finishedCallback.Invoke(true);
                yield break;
            }

            newMonster.Substitute = substituteData;

            yield return battleManager.GetMonsterSprite(userType, userIndex).ShowSubstitute(battleManager.BattleSpeed);

            finishedCallback.Invoke(true);
        }
    }
}