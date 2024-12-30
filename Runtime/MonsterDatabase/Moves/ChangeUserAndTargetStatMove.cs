using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for a move that changes the stats of the user and the target.
    /// </summary>
    public abstract class ChangeUserAndTargetStatMove : SetVolatileStatusMove
    {
        /// <summary>
        /// Stat to change on the user and the target.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected List<Stat> StatsToAffect;

        /// <summary>
        /// Dialog to show when the move is executed.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected string EffectDialogKey;

        /// <summary>
        /// Set a volatile status on both the target and user.
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
            if (Status == null) yield break;

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers
                                              .GetBattlerFromBattleIndex(targetType,
                                                                         targetIndex);

                if (WillMoveFail(battleManager,
                                 localizer,
                                 userType,
                                 userIndex,
                                 targetType,
                                 targetIndex,
                                 ignoresAbilities))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                List<object> targetExtraData = new();

                List<object> userExtraData = new();

                foreach (Stat stat in StatsToAffect)
                {
                    targetExtraData.AddRange(new object[]
                                             {
                                                 stat, GetNewStatValue(battleManager, user, target, target, stat)
                                             });

                    userExtraData.AddRange(new object[]
                                           {
                                               stat, GetNewStatValue(battleManager, user, target, user, stat)
                                           });
                }

                yield return battleManager.Statuses.AddStatus(Status,
                                                              CalculateCountdown(battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  targetType,
                                                                  targetIndex),
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities,
                                                              targetExtraData.ToArray());

                yield return battleManager.Statuses.AddStatus(Status,
                                                              CalculateCountdown(battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  userType,
                                                                  userIndex),
                                                              user,
                                                              userType,
                                                              userIndex,
                                                              false,
                                                              userExtraData.ToArray());

                yield return DialogManager.ShowDialogAndWait(EffectDialogKey,
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            user.GetNameOrNickName(localizer),
                                                                            target.GetNameOrNickName(localizer)
                                                                        });
            }

            finishedCallback.Invoke(true);
        }

        /// <summary>
        /// Get the new value for the stat of a monster.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="affectedBattler">Battle to calculate the stat for.</param>
        /// <param name="stat">Stat to affect.</param>
        /// <returns>The new value.</returns>
        protected abstract uint GetNewStatValue(BattleManager battleManager,
                                                Battler user,
                                                Battler target,
                                                Battler affectedBattler,
                                                Stat stat);
    }
}