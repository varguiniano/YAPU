using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing a damage move that has a chance to change stages after hitting.
    /// </summary>
    public abstract class StageChanceDamageMove : DamageMove
    {
        /// <summary>
        /// Does this move change stats?
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        private bool ChangeStats;

        /// <summary>
        /// Do stat changes affect the user?
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        [ShowIf(nameof(ChangeStats))]
        protected bool StatChangesAffectSelf;

        /// <summary>
        /// Stat changes it produces and the chance to produce them.
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        [ShowIf(nameof(ChangeStats))]
        private SerializableDictionary<float, SerializableDictionary<Stat, short>> StatChanges;

        /// <summary>
        /// Does this move change battle stats?
        /// </summary>
        [FoldoutGroup("Battle Stats")]
        [SerializeField]
        private bool ChangeBattleStats;

        /// <summary>
        /// Battle stat changes it produces and the chance to produce them.
        /// </summary>
        [FoldoutGroup("Battle Stats")]
        [SerializeField]
        [ShowIf(nameof(ChangeBattleStats))]
        private SerializableDictionary<float, SerializableDictionary<BattleStat, short>> BattleStatChanges;

        /// <summary>
        /// Does this move change the critical stage?
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        private bool ChangeCritical;

        /// <summary>
        /// Critical change it produces.
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        [ShowIf(nameof(ChangeCritical))]
        private short CriticalChange;

        /// <summary>
        /// Chance to change critical.
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        [ShowIf(nameof(ChangeCritical))]
        private float CriticalChance;

        /// <summary>
        /// Deal the damage and calculate a chance to set status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="targetType">Target of the move.</param>
        /// <param name="targetIndex">Target of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="targets">All targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedMoveHits"></param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        protected override IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           Battler target,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedMoveHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           Action<bool> finishedCallback,
                                                           bool forceSurvive = false)
        {
            yield return base.ExecuteDamageEffect(battleManager,
                                                  localizer,
                                                  userType,
                                                  userIndex,
                                                  user,
                                                  targetType,
                                                  targetIndex,
                                                  target,
                                                  targets,
                                                  hitNumber,
                                                  expectedMoveHits,
                                                  externalPowerMultiplier,
                                                  ignoresAbilities,
                                                  finishedCallback,
                                                  forceSurvive);

            if (StatChangesAffectSelf && user.CanBattle)
                yield return ApplyStageChanges(battleManager,
                                               userType,
                                               userIndex,
                                               targetType,
                                               targetIndex,
                                               targets,
                                               ignoresAbilities);
        }

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Execute the secondary effect of the move.
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
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            if (StatChangesAffectSelf) yield break;

            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                if (!target.IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                yield return ApplyStageChanges(battleManager,
                                               userType,
                                               userIndex,
                                               targetData.Type,
                                               targetData.Index,
                                               targets,
                                               ignoresAbilities);
            }
        }

        /// <summary>
        /// Apply the stage changes after the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Target of the move.</param>
        /// <param name="targetIndex">Target of the move.</param>
        /// <param name="targets">All targets of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        protected virtual IEnumerator ApplyStageChanges(BattleManager battleManager,
                                                        BattlerType userType,
                                                        int userIndex,
                                                        BattlerType targetType,
                                                        int targetIndex,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        bool ignoresAbilities)
        {
            float chance;

            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (ChangeStats)
                foreach (KeyValuePair<float, SerializableDictionary<Stat, short>> pair in StatChanges)
                {
                    chance = battleManager.RandomProvider.Value01()
                           / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, this, battleManager);

                    if (!(chance <= pair.Key)) continue;

                    foreach (KeyValuePair<Stat, short> statChange in pair.Value)
                        yield return battleManager.BattlerStats.ChangeStatStage(StatChangesAffectSelf
                                                                                        ? userType
                                                                                        : targetType,
                                                                                    StatChangesAffectSelf
                                                                                        ? userIndex
                                                                                        : targetIndex,
                                                                                    statChange.Key,
                                                                                    (short) (statChange.Value
                                                                                              * GetStageChangeMultiplier(battleManager,
                                                                                                    userType,
                                                                                                    userIndex,
                                                                                                    targets)),
                                                                                    userType,
                                                                                    userIndex,
                                                                                    ignoreAbilities: ignoresAbilities);
                }

            if (ChangeBattleStats)
                foreach (KeyValuePair<float, SerializableDictionary<BattleStat, short>> pair in BattleStatChanges)
                {
                    chance = battleManager.RandomProvider.Value01()
                           / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, this, battleManager);

                    if (!(chance <= pair.Key)) continue;

                    foreach (KeyValuePair<BattleStat, short> statChange in pair.Value)
                        yield return battleManager.BattlerStats.ChangeStatStage(targetType,
                                                                                    targetIndex,
                                                                                    statChange.Key,
                                                                                    (short) (statChange.Value
                                                                                              * GetStageChangeMultiplier(battleManager,
                                                                                                    userType,
                                                                                                    userIndex,
                                                                                                    targets)),
                                                                                    userType,
                                                                                    userIndex,
                                                                                    ignoreAbilities: ignoresAbilities);
                }

            if (!ChangeCritical) yield break;

            chance = battleManager.RandomProvider.Value01()
                   / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, this, battleManager);

            if (chance <= CriticalChance)
                yield return battleManager.BattlerStats.ChangeCriticalStage(targetType,
                                                                            targetIndex,
                                                                            (short) (CriticalChange
                                                                                      * GetStageChangeMultiplier(battleManager,
                                                                                            userType,
                                                                                            userIndex,
                                                                                            targets)),
                                                                            userType,
                                                                            userIndex);
        }

        /// <summary>
        /// Get a multiplier for the stage set in the data.
        /// Useful for inheritors to apply modifications.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="targets">Move targets.</param>
        /// <returns>The multiplier to apply.</returns>
        protected virtual float GetStageChangeMultiplier(BattleManager battleManager,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         List<(BattlerType Type, int Index)> targets) =>
            1;
    }
}