using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move PartingShot.
    /// It uses the same logic as UTurn.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/PartingShot", fileName = "PartingShot")]
    public class PartingShot : StageChangeMove
    {
        // TODO: Animation.

        /// <summary>
        /// List of held items that prevent switching.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Item> OwnHeldItemsThatPreventSwitching;

        /// <summary>
        /// List of held items that prevent switching.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Item> TargetHeldItemsThatPreventSwitching;

        /// <summary>
        /// Hit the target and then switch out.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
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

            if (user.CanUseHeldItemInBattle(battleManager) && OwnHeldItemsThatPreventSwitching.Contains(user.HeldItem))
                yield break;

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                if (target.CanUseHeldItemInBattle(battleManager)
                 && TargetHeldItemsThatPreventSwitching.Contains(target.HeldItem))
                    yield break;
            }

            yield return battleManager.BattleManagerBattlerSwitch.ForceSwitchBattler(userType,
                userIndex,
                userType,
                userIndex,
                this,
                null,
                false,
                ignoresAbilities,
                true);

            yield return DialogManager.WaitForDialog;
        }
    }
}