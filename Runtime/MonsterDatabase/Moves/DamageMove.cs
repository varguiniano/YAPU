using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class that represents a move that inflicts damage.
    /// </summary>
    public abstract class DamageMove : Move
    {
        /// <summary>
        /// Power of this move.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [Range(-1, 250)]
        [SerializeField]
        private int Power;

        /// <summary>
        /// Number of stages this move adds to the critical stage.
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [PropertyRange(0, 3)]
        [SerializeField]
        private byte CriticalStageModifier;

        /// <summary>
        /// Does the effect of burn apply?
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [SerializeField]
        [HideIf("@MoveCategory != Category.Physical")]
        protected bool BurnEffectApplies = true;

        /// <summary>
        /// Is this a slicing move?
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        public bool IsSlicingMove;

        /// <summary>
        /// Is this a punching move?
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        public bool IsPunchingMove;

        /// <summary>
        /// Is this a biting move?
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        public bool IsBitingMove;

        /// <summary>
        /// Used to keep track of the last damage that was made.
        /// To be used by moves with recoil or similar effects that depend on the damage done.
        /// </summary>
        protected int LastDamageMade;

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">Direct reference to the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected hits of this move.</param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
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
            yield return ExecuteDamageEffect(battleManager,
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

        /// <summary>
        /// Deal damage to the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        protected virtual IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Battler user,
                                                          List<(BattlerType Type, int Index)> targets,
                                                          int hitNumber,
                                                          int expectedMoveHits,
                                                          float externalPowerMultiplier,
                                                          bool ignoresAbilities,
                                                          Action<bool> finishedCallback,
                                                          bool forceSurvive = false)
        {
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                yield return ExecuteDamageEffect(battleManager,
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
            }
        }

        /// <summary>
        /// Deal damage to a target.
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
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        protected virtual IEnumerator ExecuteDamageEffect(BattleManager battleManager,
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
            bool bypassSubstitute = user.CanUseAbility(battleManager, false)
                                 && user.GetAbility()
                                        .ByPassesSubstitute(targetType,
                                                            targetIndex,
                                                            battleManager,
                                                            userType,
                                                            userIndex,
                                                            this);

            float effectiveness = CalculateEffectiveness(user, target, ignoresAbilities, battleManager);

            if (effectiveness == 0)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                LastDamageMade = 0;
                finishedCallback.Invoke(false);

                yield break;
            }

            yield return target.OnHitByMove(this,
                                            effectiveness,
                                            bypassSubstitute,
                                            ignoresAbilities,
                                            battleManager,
                                            user,
                                            (newEffectiveness, willForceSurvive) =>
                                            {
                                                effectiveness = newEffectiveness;
                                                forceSurvive |= willForceSurvive;
                                            });

            switch (effectiveness)
            {
                case > 1:

                    DialogManager.ShowDialog("Battle/Move/SuperEffective",
                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                             acceptInput: false);

                    break;
                case < 1:
                    DialogManager.ShowDialog("Battle/Move/NotVeryEffective",
                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                             acceptInput: false);

                    break;
            }

            float criticalChance = 0;

            yield return MonsterMathHelper.CalculateCriticalChance(user,
                                                                   CriticalStageModifier,
                                                                   target,
                                                                   battleManager,
                                                                   this,
                                                                   ignoresAbilities,
                                                                   newChance => criticalChance = newChance);

            float criticalRandom = battleManager.RandomProvider.Value01();

            bool isCritical = criticalRandom < criticalChance;

            float damage = 0;

            yield return CalculateMoveDamage(battleManager,
                                             localizer,
                                             user,
                                             target,
                                             hitNumber,
                                             expectedMoveHits,
                                             isCritical,
                                             effectiveness,
                                             externalPowerMultiplier,
                                             ignoresAbilities,
                                             targets,
                                             calculatedDamage => damage = calculatedDamage);

            int roundedDamage = Mathf.Max(Mathf.RoundToInt(damage), 1);

            Logger.Info("The move's damage is " + roundedDamage + ".");

            bool substituteTookHit = false;

            uint previousHP = target.CurrentHP;

            yield return battleManager.BattlerHealth.ChangeLife(targetType,
                                                                targetIndex,
                                                                userType,
                                                                userIndex,
                                                                -roundedDamage,
                                                                this,
                                                                ignoreAbilities: ignoresAbilities,
                                                                forceSurvive: forceSurvive,
                                                                finished: (damageTaken, substituteWasEnabled) =>
                                                                          {
                                                                              LastDamageMade = -damageTaken;
                                                                              substituteTookHit = substituteWasEnabled;
                                                                          });

            if (isCritical)
            {
                DialogManager.ShowDialog("Battle/Move/Critical",
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                         acceptInput: false);

                user.ExtraData.TimesLandedCriticalHitLastBattle++;
            }

            yield return DialogManager.WaitForDialog;

            yield return target.AfterHitByMove(this,
                                               effectiveness,
                                               user,
                                               LastDamageMade,
                                               previousHP,
                                               isCritical,
                                               substituteTookHit,
                                               ignoresAbilities,
                                               hitNumber,
                                               expectedMoveHits,
                                               battleManager,
                                               localizer);

            if (hitNumber == expectedMoveHits - 1)
                yield return target.AfterHitByMultihitMove(this,
                                                           effectiveness,
                                                           user,
                                                           substituteTookHit,
                                                           ignoresAbilities,
                                                           battleManager,
                                                           localizer);

            finishedCallback.Invoke(true);
        }

        /// <summary>
        /// Calculate the effectiveness of the move.
        /// </summary>
        protected virtual float CalculateEffectiveness(Battler user,
                                                       Battler target,
                                                       bool ignoresAbilities,
                                                       BattleManager battleManager)
        {
            target.GetEffectivenessOfMove(user,
                                          this,
                                          ignoresAbilities,
                                          battleManager,
                                          true,
                                          out float effectiveness);

            return effectiveness;
        }

        /// <summary>
        /// Calculate the damage of a move.
        /// Based on: https://bulbapedia.bulbagarden.net/wiki/Stat#Generations_III_onward
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="hitNumber">Number of the current hit in multihit moves.</param>
        /// <param name="totalHits">Total number of hits.</param>
        /// <param name="isCritical">Is it a critical move?</param>
        /// <param name="typeEffectiveness">Type effectiveness.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="finished">Callback with the amount of damage it deals.</param>
        protected virtual IEnumerator CalculateMoveDamage(BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Battler user,
                                                          Battler target,
                                                          int hitNumber,
                                                          int totalHits,
                                                          bool isCritical,
                                                          float typeEffectiveness,
                                                          float externalPowerMultiplier,
                                                          bool ignoresAbilities,
                                                          List<(BattlerType Type, int Index)> allTargets,
                                                          Action<float> finished)
        {
            (BattlerType userType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            (BattlerType targetType, int _) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            float attackDefense =
                GetAttackDefenseDamageMultiplier(battleManager, user, target, isCritical, ignoresAbilities);

            float numerator = (2 * user.StatData.Level / 5f + 2)
                            * GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
                            * attackDefense
                            * externalPowerMultiplier;

            float baseMultiplier = numerator / 50 + 2;

            float targets = allTargets.Count > 1 ? .75f : 1;

            float weather = battleManager.Scenario.GetWeather(out Weather currentWeather)
                                ? currentWeather.CalculateMovesDamageMultiplier(this,
                                                                                    user,
                                                                                    battleManager)
                                : 1f;

            float terrain = battleManager.Scenario.Terrain != null
                                ? battleManager.Scenario.Terrain.CalculateMovesDamageMultiplier(this,
                                    user,
                                    target,
                                    battleManager)
                                : 1f;

            float critical = isCritical ? 1.5f : 1;
            float random = battleManager.RandomProvider.Range(.85f, 1);

            (MonsterType firstType, MonsterType secondType) = user.GetTypes(battleManager.YAPUSettings);

            float stabMultiplier = 1;

            bool isStab = GetMoveTypeInBattle(user, battleManager) == firstType
                       || GetMoveTypeInBattle(user, battleManager) == secondType;

            if (isStab)
                yield return user.OnCalculateStabDamageWhenUsing(this,
                                                                 stabMultiplier,
                                                                 target,
                                                                 typeEffectiveness,
                                                                 isCritical,
                                                                 battleManager,
                                                                 localizer,
                                                                 newMultiplier => stabMultiplier *= newMultiplier);

            float stab = isStab ? 1.5f * stabMultiplier : 1;

            float status = GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                        && BurnEffectApplies
                        && user.IsAffectedByBurnDamageReduction(battleManager)
                               ? user.GetStatus() == null
                                     ? 1f
                                     : user.GetStatus().OnCalculateMoveDamage(this, user, battleManager)
                               : 1f;

            float additional = 1f;

            yield return user.OnCalculateMoveDamageWhenUsing(this,
                                                             additional,
                                                             target,
                                                             typeEffectiveness,
                                                             isCritical,
                                                             hitNumber,
                                                             totalHits,
                                                             ignoresAbilities,
                                                             battleManager,
                                                             localizer,
                                                             allTargets,
                                                             newMultiplier => additional = newMultiplier);

            if (battleManager.BattleType != BattleType.SingleBattle)
                foreach (Battler ally in battleManager.Battlers.GetBattlersFighting(userType)
                                                      .Where(candidate => candidate != target))
                    yield return ally.OnCalculateMoveDamageWhenAllyUsing(this,
                                                                         additional,
                                                                         user,
                                                                         target,
                                                                         typeEffectiveness,
                                                                         isCritical,
                                                                         ignoresAbilities,
                                                                         battleManager,
                                                                         localizer,
                                                                         newMultiplier => additional = newMultiplier);

            yield return target.OnCalculateMoveDamageWhenTargeted(this,
                                                                  additional,
                                                                  user,
                                                                  ignoresAbilities,
                                                                  battleManager,
                                                                  newMultiplier => additional = newMultiplier);

            foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(targetType))
                yield return statusSlot.Key.OnCalculateMoveDamageWhenTargeted(this,
                                                                              additional,
                                                                              user,
                                                                              target,
                                                                              ignoresAbilities,
                                                                              battleManager,
                                                                              localizer,
                                                                              newMultiplier =>
                                                                                  additional = newMultiplier);

            if (battleManager.BattleType != BattleType.SingleBattle)
                foreach (Battler ally in battleManager.Battlers.GetBattlersFighting(targetType)
                                                      .Where(candidate => candidate != target))
                    yield return ally.OnCalculateMoveDamageWhenAllyTargeted(this,
                                                                            additional,
                                                                            user,
                                                                            target,
                                                                            ignoresAbilities,
                                                                            battleManager,
                                                                            newMultiplier =>
                                                                                additional = newMultiplier);

            finished.Invoke(baseMultiplier
                          * targets
                          * weather
                          * terrain
                          * critical
                          * random
                          * stab
                          * typeEffectiveness
                          * status
                          * additional);
        }

        /// <summary>
        /// Get the multiplier for the attack and defense part of the damage formula.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <returns>The attack/defense multiplier.</returns>
        protected virtual float GetAttackDefenseDamageMultiplier(BattleManager battleManager,
                                                                 Battler user,
                                                                 Battler target,
                                                                 bool isCritical,
                                                                 bool ignoresAbilities) =>
            GetAttackDefenseDamageMultiplier(battleManager,
                                             user,
                                             target,
                                             isCritical,
                                             ignoresAbilities,
                                             GetAttackStatForDamageCalculation(battleManager,
                                                                               user,
                                                                               target,
                                                                               ignoresAbilities),
                                             GetDefenseStatForDamageCalculation(battleManager,
                                                                                    user,
                                                                                    target,
                                                                                    ignoresAbilities));

        /// <summary>
        /// Get the multiplier for the attack and defense part of the damage formula.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="attackStat">Attack stat to use.</param>
        /// <param name="defenseStat">Defense stat to use.</param>
        /// <returns>The attack/defense multiplier.</returns>
        protected virtual float GetAttackDefenseDamageMultiplier(BattleManager battleManager,
                                                                 Battler user,
                                                                 Battler target,
                                                                 bool isCritical,
                                                                 bool ignoresAbilities,
                                                                 Stat attackStat,
                                                                 Stat defenseStat)
        {
            float attack =
                GetAttackValueForDamageCalculation(battleManager,
                                                   user,
                                                   target,
                                                   isCritical,
                                                   attackStat,
                                                   ignoresAbilities);

            float defenseStage =
                (user.CanUseAbility(battleManager, false)
              && user.GetAbility().IgnoreDefenseStageWhenWhenUsingMove())
             || DoesIgnoreDefenseStageWhenUsingMove(battleManager,
                                                    user,
                                                    target,
                                                    isCritical,
                                                    attackStat,
                                                    ignoresAbilities)
                    ? 1
                    : MonsterMathHelper.GetStageMultiplier(target, defenseStat, isCritical);

            float defense = MonsterMathHelper.CalculateStat(target, defenseStat, battleManager) * defenseStage;

            return attack / defense;
        }

        /// <summary>
        /// Get the attack stat to use for damage calculation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <returns>The stat to use.</returns>
        protected virtual Stat
            GetAttackStatForDamageCalculation(BattleManager battleManager,
                                              Battler user,
                                              Battler target,
                                              bool ignoresAbilities) =>
            GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                ? Stat.Attack
                : Stat.SpecialAttack;

        /// <summary>
        /// Get the attack stat to use for damage calculation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <returns>The stat to use.</returns>
        protected virtual Stat
            GetDefenseStatForDamageCalculation(BattleManager battleManager,
                                               Battler user,
                                               Battler target,
                                               bool ignoresAbilities) =>
            GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                ? Stat.Defense
                : Stat.SpecialDefense;

        /// <summary>
        /// Get the value of the attack stat to use for damage calculation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="attackStat">Stat to use.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <returns>The value of that stat.</returns>
        protected virtual float GetAttackValueForDamageCalculation(BattleManager battleManager,
                                                                   Battler user,
                                                                   Battler target,
                                                                   bool isCritical,
                                                                   Stat attackStat,
                                                                   bool ignoresAbilities)
        {
            float attackStage =
                target.CanUseAbility(battleManager, ignoresAbilities)
             && target.GetAbility().IgnoreAttackStageWhenWhenTargeted()
                    ? 1
                    : MonsterMathHelper.GetStageMultiplier(user, attackStat, isCritical);

            return MonsterMathHelper.CalculateStat(user, attackStat, battleManager) * attackStage;
        }

        /// <summary>
        /// Does this move ignore the defense stage?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="attackStat">Stat to use.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <returns>True if it does.</returns>
        protected virtual bool DoesIgnoreDefenseStageWhenUsingMove(BattleManager battleManager,
                                                                   Battler user,
                                                                   Battler target,
                                                                   bool isCritical,
                                                                   Stat attackStat,
                                                                   bool ignoresAbilities) =>
            false;

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <returns>The move's power.</returns>
        public virtual int GetMovePower(MonsterInstance owner = null) => Power;

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="hitNumber">In multihit moves, the number of this hit.</param>
        /// <returns>The move's power.</returns>
        public virtual int GetMovePowerInBattle(BattleManager battleManager,
                                                Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                int hitNumber = 0)
        {
            float userMultiplier =
                user.GetMovePowerMultiplierWhenUsingMove(battleManager, this, target, ignoresAbilities);

            float targetMultiplier =
                target?.GetMovePowerMultiplierWhenHit(battleManager, this, user, ignoresAbilities, false) ?? 1;

            return (int) (GetMovePower(user) * userMultiplier * targetMultiplier);
        }
    }
}