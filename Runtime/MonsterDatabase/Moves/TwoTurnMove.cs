using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that take two turns to perform.
    /// While it inherits from damage move, it doesn't have to be a damage move as the effect is overriden,
    /// however, this is useful for inheritors to access the same damage algorithms.
    /// </summary>
    public abstract class TwoTurnMove : DamageMove
    {
        /// <summary>
        /// Reference to the status that will store the build up for this move.
        /// </summary>
        [FoldoutGroup("Two Turn Effect")]
        [SerializeField]
        protected TwoTurnMoveBuildUp BuildUpStatus;

        /// <summary>
        /// Weathers that allow to bypass charging.
        /// </summary>
        [FoldoutGroup("Two Turn Effect")]
        [SerializeField]
        private List<Weather> WeatherByPasses;

        /// <summary>
        /// Items that allow to bypass charging.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Two Turn Effect")]
        [SerializeField]
        private List<Item> ConsumableItemByPasses;

        /// <summary>
        /// Set the Volatile status to use the move next turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">Direct reference to the user of the move.</param>
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

            bool byPass = battleManager.Scenario.GetWeather(out Weather weather) && WeatherByPasses.Contains(weather);

            if (!byPass
             && battler.CanUseHeldItemInBattle(battleManager)
             && ConsumableItemByPasses.Contains(battler.HeldItem))
            {
                byPass = true;

                yield return battler.ConsumeItemInBattle(battleManager);
            }

            if (byPass)
            {
                // We have to show the dialog here since the battle manager doesn't show it on the first turn.
                if (!ShowUsedDialog)
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                                 localizableModifiers: false,
                                                                 modifiers: new[]
                                                                            {
                                                                                battler.GetNameOrNickName(battleManager
                                                                                   .Localizer),
                                                                                battleManager.Localizer[LocalizableName]
                                                                            },
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

                List<Transform> targetPositions = new();

                foreach ((BattlerType targetType, int targetIndex) in targets)
                    targetPositions.Add(battleManager.GetMonsterSprite(targetType, targetIndex).transform);

                yield return PlaySecondTurnAnimation(battleManager,
                                                     battleManager.BattleSpeed,
                                                     userType,
                                                     userIndex,
                                                     battler,
                                                     sprite.transform,
                                                     targets,
                                                     targetPositions,
                                                     ignoresAbilities);

                yield return ExecuteSecondEffect(battleManager,
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
            else
            {
                yield return battleManager.Statuses.AddStatus(BuildUpStatus,
                                                              2,
                                                              userType,
                                                              userIndex,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities,
                                                              targets);

                finishedCallback.Invoke(true);
            }

            yield return FirstTurnExtraEffect(battleManager, userType, userIndex, ignoresAbilities, targets);
        }

        /// <summary>
        /// Used to add an extra effect to the first turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="targets">Targets of the move.</param>
        protected virtual IEnumerator FirstTurnExtraEffect(BattleManager battleManager,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           bool ignoresAbilities,
                                                           List<(BattlerType Type, int Index)> targets)
        {
            yield break;
        }

        /// <summary>
        /// Animation to fall back to.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        [ShowIf(nameof(UsesFallbackAnimation))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        protected Move SecondTurnFallbackAnimation;

        /// <summary>
        /// Play the animation for the second turn of this move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public virtual IEnumerator PlaySecondTurnAnimation(BattleManager battleManager,
                                                           float speed,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           Transform userPosition,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           List<Transform> targetPositions,
                                                           bool ignoresAbilities)
        {
            if (UsesFallbackAnimation)
                yield return SecondTurnFallbackAnimation.PlayAnimation(battleManager,
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
        /// Execute the effect for the second turn of this move.
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
        public abstract IEnumerator ExecuteSecondEffect(BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        BattlerType userType,
                                                        int userIndex,
                                                        Battler user,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        int hitNumber,
                                                        int expectedHits,
                                                        float externalPowerMultiplier,
                                                        bool ignoresAbilities,
                                                        Action<bool> finishedCallback);
    }
}