using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move SpitUp.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/SpitUp", fileName = "SpitUp")]
    public class SpitUp : DamageMove
    {
        /// <summary>
        /// Reference to the stockpile status.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllLayeredVolatileStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private StockpileStatus Stockpile;

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
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
            if (!battleManager.Statuses.HasStatus(Stockpile, userType, userIndex))
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finishedCallback.Invoke(false);
                yield break;
            }

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

            yield return battleManager.Statuses.RemoveStatus(Stockpile, userType, userIndex);
        }

        /// <summary>
        /// Multiply the base power by the stockpile layers.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (Stockpile.LayerCount.ContainsKey(user) ? Stockpile.LayerCount[user] : 0)
          * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}