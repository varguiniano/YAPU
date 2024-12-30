using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing a damage move that recoils the user.
    /// </summary>
    public abstract class DamageWithRecoilMove : DamageMove
    {
        /// <summary>
        /// Percentage used for recoil damage.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [Range(0, 1)]
        public float RecoilPercentage;

        /// <summary>
        /// Deal the damage as any damage move and then recoil.
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
        /// <param name="finishedCallback"></param>
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

            if (LastDamageMade == 0) yield break;

            if (user.CanUseAbility(battleManager, false) && !user.GetAbility().IsAffectedByRecoil(user, battleManager))
                yield break;

            int recoilDamage =
                Mathf.RoundToInt(GetValueForRecoil(battleManager, userType, userIndex, targets)
                               * RecoilPercentage);

            yield return DialogManager.ShowDialogAndWait("Battle/Move/Recoil",
                                                         localizableModifiers: false,
                                                         modifiers: battleManager
                                                                   .Battlers
                                                                   .GetBattlerFromBattleIndex(userType, userIndex)
                                                                   .GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(userType,
                                                                userIndex,
                                                                userType,
                                                                userIndex,
                                                                -recoilDamage,
                                                                isSecondaryDamage: true);
        }

        /// <summary>
        /// Get the value needed to the recoil damage.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>The value to be used when calculating the recoil.</returns>
        protected abstract float GetValueForRecoil(BattleManager battleManager,
                                                   BattlerType userType,
                                                   int userIndex,
                                                   List<(BattlerType Type, int Index)> targets);
    }
}