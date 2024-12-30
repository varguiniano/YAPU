using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetIcons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for implementing monster moves.
    /// </summary>
    public abstract class Move : DocumentableMonsterDatabaseScriptable<Move>, ICommandParameter
    {
        /// <summary>
        /// Should this move be indexed as a usable move?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool ShouldIndex = true;

        /// <summary>
        /// Type of this move.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [ValueDropdown("GetAllMonsterTypes")]
        [SerializeField]
        private MonsterType MoveType;

        /// <summary>
        /// Category of this move.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [SerializeField]
        private Category MoveCategory;

        /// <summary>
        /// Base power points for this move.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [OnValueChanged(nameof(OnBasePowerPointsChanged))]
        public byte BasePowerPoints;

        /// <summary>
        /// Max power points for this move.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [OnValueChanged(nameof(OnBasePowerPointsChanged))]
        public byte MaxPowerPoints;

        /// <summary>
        /// Priority of this move.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [PropertyRange(-7, 5)]
        [SerializeField]
        private int Priority;

        /// <summary>
        /// Most status moves are not affected by type effectiveness but some like Thunder Wave are.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [ShowIf("@MoveCategory == Category.Status")]
        [Tooltip("Most status moves are not affected by type effectiveness but some like Thunder Wave are.")]
        [SerializeField]
        private bool AffectedByTypeEffectivenessMatrix;

        /// <summary>
        /// Is the move affected by type effectiveness?
        /// </summary>
        public bool IsAffectedByTypeEffectiveness =>
            MoveCategory != Category.Status || AffectedByTypeEffectivenessMatrix;

        /// <summary>
        /// Does this move make contact with the defender?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [SerializeField]
        private bool MakesContact;

        /// <summary>
        /// Is this move affected by protecting?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool AffectedByProtect;

        /// <summary>
        /// Is this move affected by magic coat?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool AffectedByMagicCoat;

        /// <summary>
        /// Is this move affected by snatching?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool AffectedBySnatch;

        /// <summary>
        /// Is this move affected by mirroring?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool AffectedByMirror;

        /// <summary>
        /// Is this move affected by king's rock?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool AffectedByKingsRock;

        /// <summary>
        /// Does this move bypass substitutes?
        /// </summary>
        [FoldoutGroup("Damage Stats")]
        [HideIf("@MoveCategory == Category.Status")]
        public bool DamageBypassesSubstitute;

        /// <summary>
        /// Is this sound based?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool SoundBased;

        /// <summary>
        /// Is this a ball move?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool IsBallMove;

        /// <summary>
        /// Is this a wind move?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        public bool IsWindMove;

        /// <summary>
        /// Can this move override effectiveness?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [ShowIf(nameof(IsAffectedByTypeEffectiveness))]
        public bool OverridesEffectiveness;

        /// <summary>
        /// Overrides applied to effectiveness depending on the target type.
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [ShowIf("@" + nameof(OverridesEffectiveness) + " && " + nameof(IsAffectedByTypeEffectiveness))]
        public SerializableDictionary<MonsterType, float> EffectivenessOverride;

        /// <summary>
        /// Flag to mark that this move has infinite accuracy.
        /// </summary>
        [FoldoutGroup("Targeting")]
        [SerializeField]
        private bool InfiniteAccuracy;

        /// <summary>
        /// Accuracy of this move.
        /// </summary>
        [FormerlySerializedAs("Accuracy")]
        [FoldoutGroup("Targeting")]
        [PropertyRange(0, 100)]
        [HideIf(nameof(InfiniteAccuracy))]
        [SerializeField]
        protected int BaseAccuracy;

        /// <summary>
        /// Can the weather modify the accuracy?
        /// </summary>
        [FoldoutGroup("Targeting")]
        [HideIf(nameof(InfiniteAccuracy))]
        [SerializeField]
        private bool WeatherCanModifyAccuracy;

        [FoldoutGroup("Targeting")]
        [HideIf("@" + nameof(InfiniteAccuracy) + " || !" + nameof(WeatherCanModifyAccuracy))]
        [SerializeField]
        [Tooltip("-1 equals infinite.")]
        private SerializableDictionary<Weather, int> WeatherAccuracyModifiers;

        /// <summary>
        /// Possible targets this move can have.
        /// </summary>
        [FoldoutGroup("Targeting")]
        public bool CanHaveMultipleTargets;

        /// <summary>
        /// Possible targets this move can have.
        /// </summary>
        [FoldoutGroup("Targeting")]
        public PossibleTargets MovePossibleTargets;

        /// <summary>
        /// Is this a multi hit move?
        /// </summary>
        [FoldoutGroup("Multi Hit")]
        public bool IsMultiHit;

        /// <summary>
        /// Chance of hitting a number of times.
        /// </summary>
        [FoldoutGroup("Multi Hit")]
        [ShowIf(nameof(IsMultiHit))]
        public SerializableDictionary<float, int> HitChances;

        /// <summary>
        /// Types immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Abilities immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<Ability> ImmuneAbilities;

        /// <summary>
        /// Held items immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<Item> ImmuneHeldItems;

        /// <summary>
        /// List of volatiles that are immune to this move.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        protected List<VolatileStatus> ImmuneVolatileStatuses;

        /// <summary>
        /// List of terrains that are immune to this move.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        protected List<Terrain> ImmuneTerrains;

        /// <summary>
        /// List of global statuses that are immune to this move.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllGlobalStatuses))]
        #endif
        protected List<GlobalStatus> ImmuneGlobalStatuses;

        /// <summary>
        /// Show a used dialog when the move is used?
        /// </summary>
        [FoldoutGroup("Dialogs")]
        public bool ShowUsedDialog = true;

        /// <summary>
        /// Used when the move doesn't have an animation and we want to fallback to another move's animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected bool UsesFallbackAnimation;

        /// <summary>
        /// Animation to fall back to.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        [ShowIf(nameof(UsesFallbackAnimation))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        protected Move FallbackAnimation;

        /// <summary>
        /// Can this move be used outside of battle?
        /// </summary>
        [FoldoutGroup("Out of battle")]
        public bool CanBeUsedOutsideBattle;

        /// <summary>
        /// All move's localizable strings should start by Moves/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Moves/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// Get the move category.
        /// </summary>
        public virtual Category GetMoveCategory(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager) =>
            MoveCategory;

        /// <summary>
        /// Get the type of this move out of battle.
        /// </summary>
        /// <param name="monster">Owner of the move.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The type of the move.</returns>
        public virtual MonsterType GetMoveType(MonsterInstance monster, YAPUSettings settings)
        {
            MonsterType type = GetOriginalMoveType();

            return monster == null ? type : monster.GetAbility().GetMoveType(this, monster, type);
        }

        /// <summary>
        /// Get the type of this move in battle.
        /// </summary>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The type of the move.</returns>
        public virtual MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager)
        {
            MonsterType type = GetOriginalMoveType();

            if (battler.CanUseAbility(battleManager, false))
                type = battler.GetAbility().GetMoveTypeInBattle(this, battler, battleManager, type);

            return type;
        }

        /// <summary>
        /// Get the original type of this move.
        /// </summary>
        public MonsterType GetOriginalMoveType() => MoveType;

        /// <summary>
        /// Get the move's priority.
        /// </summary>
        /// <param name="owner">Move owner.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications for abilities or items that modify the priority.</param>
        /// <returns>The priority modifier.</returns>
        public virtual int GetPriority(Battler owner,
                                       List<Battler> targets,
                                       BattleManager battleManager,
                                       bool showNotifications = true) =>
            Priority + owner.GetMovePriorityModifier(this, targets, Priority, battleManager, showNotifications);

        /// <summary>
        /// Calculate the precision of this move before performing it.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The calculated precision.</returns>
        public virtual float CalculateAccuracy(Battler user,
                                               Battler target,
                                               bool ignoresAbilities,
                                               BattleManager battleManager)
        {
            int evasionStage = (user.CanUseAbility(battleManager, false)
                             && user.GetAbility().IgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing())
                            || DoesIgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing(user,
                                   target,
                                   ignoresAbilities,
                                   battleManager)
                                   ? 0
                                   : target.GetEvasionStage(battleManager, ignoresAbilities);

            int accuracyStage = target.CanUseAbility(battleManager, ignoresAbilities)
                             && user.GetAbility().IgnoreAccuracyWhenCalculatingMoveAccuracyWhenTargeted()
                                    ? 0
                                    : user.GetAccuracyStage(battleManager, false);

            short stage = (short) Mathf.Clamp(accuracyStage - evasionStage, -6, 6);
            float stageMultiplier = MonsterMathHelper.GetStageMultiplier(stage, BattleStat.Accuracy);
            float accuracy = GetPreStageAccuracy(user, target, ignoresAbilities, battleManager) * stageMultiplier;

            if (target.CanUseAbility(battleManager, ignoresAbilities))
            {
                (float abilityStageMultiplier, bool isClamped, (int Min, int Max) clamp) = target.GetAbility()
                   .OnCalculateAccuracyWhenTargeted(this,
                                                    user,
                                                    target,
                                                    battleManager);

                accuracy *= abilityStageMultiplier;

                if (isClamped) accuracy = Mathf.Clamp(accuracy, clamp.Min, clamp.Max);
            }

            if (target.CanUseHeldItemInBattle(battleManager))
                accuracy *= target.HeldItem.OnCalculateAccuracyWhenTargeted(this,
                                                                            user,
                                                                            target,
                                                                            battleManager);

            return accuracy;
        }

        /// <summary>
        /// Does this move ignore the target's evasion when calculating the move accuracy?
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it does.</returns>
        protected virtual bool DoesIgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing(Battler user,
            Battler target,
            bool ignoresAbilities,
            BattleManager battleManager) =>
            false;

        /// <summary>
        /// Get the pre stage accuracy of the move.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The calculated precision.</returns>
        public virtual int GetPreStageAccuracy(Battler user,
                                               Battler target,
                                               bool ignoresAbilities,
                                               BattleManager battleManager)
        {
            int accuracy = BaseAccuracy;

            // ReSharper disable once InvertIf
            if (battleManager.Scenario.GetWeather(out Weather weather))
            {
                if (WeatherCanModifyAccuracy
                 && WeatherAccuracyModifiers.TryGetValue(weather, out int weatherAccuracy))
                    accuracy = weatherAccuracy;

                accuracy = Mathf.RoundToInt(accuracy * weather.GetGeneralAccuracyModifier());
            }

            return Mathf.RoundToInt(accuracy
                                  * user.GetMoveAccuracyMultiplierWhenUsed(battleManager,
                                                                           target,
                                                                           this,
                                                                           ignoresAbilities)
                                  * (target?.GetMoveAccuracyMultiplierWhenTargeted(battleManager,
                                         user,
                                         this,
                                         ignoresAbilities)
                                  ?? 1));
        }

        /// <summary>
        /// Get the accuracy out of battle.
        /// </summary>
        /// <returns></returns>
        public virtual int GetOutOfBattleAccuracy() => BaseAccuracy;

        /// <summary>
        /// Check if this move has infinite accuracy out of battle.
        /// </summary>
        public virtual bool HasInfiniteAccuracy() => InfiniteAccuracy;

        /// <summary>
        /// Check if this move has infinite accuracy in battle.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it has.</returns>
        public virtual bool HasInfiniteAccuracy(Battler user, Battler target, BattleManager battleManager)
        {
            if (WeatherCanModifyAccuracy
             && battleManager.Scenario.GetWeather(out Weather weather)
             && WeatherAccuracyModifiers.ContainsKey(weather)
             && WeatherAccuracyModifiers[weather] == -1)
                return true;

            return InfiniteAccuracy;
        }

        /// <summary>
        /// Cast this to a damage move if it is.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The object as a damage move.</returns>
        public virtual DamageMove GetDamageMove(Battler user, Battler target, BattleManager battleManager) =>
            this as DamageMove;

        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        public virtual int GetNumberOfHits(BattleManager battleManager,
                                           BattlerType userType,
                                           int userIndex,
                                           List<(BattlerType Type, int Index)> targets,
                                           bool ignoresAbilities)
        {
            (bool modified, int numberOfHits) = battleManager.Battlers
                                                             .GetBattlerFromBattleIndex(userType, userIndex)
                                                             .GetNumberOfHitsOfMultihitMove(battleManager,
                                                                  this,
                                                                  ignoresAbilities,
                                                                  targets);

            if (modified) return numberOfHits;

            if (!IsMultiHit) return 1;

            float chance = battleManager.RandomProvider.Value01();

            foreach (KeyValuePair<float, int> pair in HitChances)
                if (chance <= pair.Key)
                    return pair.Value;

            return 1;
        }

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="customFailMessage">Custom message that some effects can display.</param>
        public virtual bool WillMoveFail(BattleManager battleManager,
                                         ILocalizer localizer,
                                         BattlerType userType,
                                         int userIndex,
                                         ref List<(BattlerType Type, int Index)> targets,
                                         int hitNumber,
                                         int expectedHits,
                                         bool ignoresAbilities,
                                         out string customFailMessage)
        {
            customFailMessage = "";

            List<(BattlerType Type, int Index)> failedTargets = new();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach ((BattlerType TargetType, int TargetIndex) target in targets)
                if (WillMoveFail(battleManager,
                                 localizer,
                                 userType,
                                 userIndex,
                                 target.TargetType,
                                 target.TargetIndex,
                                 ignoresAbilities))
                    failedTargets.Add(target);

            if (targets.Count == failedTargets.Count) return true;

            // If there are valid targets still, remove the failed ones but don't fail the entire move.
            foreach ((BattlerType, int) failedTarget in failedTargets) targets.Remove(failedTarget);

            return false;
        }

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        internal virtual bool WillMoveFail(BattleManager battleManager,
                                           ILocalizer localizer,
                                           BattlerType userType,
                                           int userIndex,
                                           BattlerType targetType,
                                           int targetIndex,
                                           bool ignoresAbilities)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            (MonsterType type1, MonsterType type2) = target.GetTypes(battleManager.YAPUSettings);

            return !target.CanBattle
                || ImmuneTypes.Contains(type1)
                || ImmuneTypes.Contains(type2)
                || (target.CanUseAbility(battleManager, ignoresAbilities)
                 && ImmuneAbilities.Contains(target.GetAbility()))
                || target.VolatileStatuses.Any(slot => ImmuneVolatileStatuses.Contains(slot.Key))
                || ImmuneTerrains.Contains(battleManager.Scenario.Terrain)
                || battleManager.Statuses.GetGlobalStatuses().Any(status => ImmuneGlobalStatuses.Contains(status.Key))
                || (target.CanUseHeldItemInBattle(battleManager) && ImmuneHeldItems.Contains(target.HeldItem))
                || target.IsImmuneToMove(this, userType, userIndex, ignoresAbilities, battleManager);
        }

        /// <summary>
        /// Called when the final targets are about to be selected.
        /// This allows the move to reselect different targets on certain conditions.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User type.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="targets">Current preselected targets.</param>
        internal virtual List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets) =>
            targets;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        public virtual IEnumerator PlayAnimation(BattleManager battleManager,
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
                yield return FallbackAnimation.PlayAnimation(battleManager,
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
        public abstract IEnumerator ExecuteEffect(BattleManager battleManager,
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

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public virtual bool HasSecondaryEffect() => false;

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
        /// <param name="ignoresAbilities">Does this move ignore abilies?</param>
        public virtual IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
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
            yield break;
        }

        /// <summary>
        /// Does the move make contact?
        /// </summary>
        /// <returns></returns>
        public virtual bool DoesMoveMakeContact(Battler user,
                                                Battler target,
                                                BattleManager battleManager,
                                                bool ignoresAbilities) =>
            MakesContact;

        /// <summary>
        /// Callback that can be used to alter the normal order of battle.
        /// </summary>
        /// <param name="moveOwner">Owner of the move.</param>
        /// <param name="lastAdded">Last added battler.</param>
        /// <param name="order">Current calculated order.</param>
        /// <param name="battlers">Battlers that will perform actions this turn.</param>
        /// <param name="actions">Actions that will be performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns> An int value.
        /// -1 -> Go immediately before the last action added.
        /// 0 -> Follow normal ordering.
        /// 1 -> Go immediately after the last action added.
        /// </returns>
        public virtual int OnActionAddedToOrder(Battler moveOwner,
                                                Battler lastAdded,
                                                ref Queue<Battler> order,
                                                List<Battler> battlers,
                                                SerializableDictionary<Battler, BattleAction> actions,
                                                BattleManager battleManager) =>
            0;

        /// <summary>
        /// Called when the move fails.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        public virtual IEnumerator OnMoveFailed(BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                List<(BattlerType Type, int Index)> targets,
                                                float externalPowerMultiplier)
        {
            yield break;
        }

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterTurnPostStatus(Battler battler,
                                                       BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the battle ends to clean up any data the move may have stored.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the base power points are changed to adjust the max ones.
        /// </summary>
        private void OnBasePowerPointsChanged() =>
            MaxPowerPoints = (byte) Mathf.Clamp(MaxPowerPoints, BasePowerPoints, int.MaxValue);

        /// <summary>
        /// Use the move outside of battle.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="mapSceneLauncher"></param>
        public virtual IEnumerator UseOutOfBattle(PlayerCharacter playerCharacter,
                                                  MonsterInstance monster,
                                                  ILocalizer localizer,
                                                  MapSceneLauncher mapSceneLauncher)
        {
            if (playerCharacter.Region.IsMoveLockedByBadge(this, out Badge badge))
                if (!playerCharacter.GlobalGameData.HasBadge(badge, playerCharacter.Region))
                {
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/BlockedByBadge",
                                                                 modifiers: new[]
                                                                            {
                                                                                badge.LocalizableName, LocalizableName
                                                                            });

                    yield break;
                }

            if (!playerCharacter.CanUseMove(monster, this, out List<ActorMoveTarget> targets))
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            yield return DialogManager.CloseMenus();
            yield return playerCharacter.UseMove(monster, this, targets);
        }

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => localizer[LocalizableName];

        /// <summary>
        /// Get the icon to use in the editor.
        /// </summary>
        [AssetIcon] // TODO: Optional.
        public Sprite EditorICon => MoveType == null ? null : MoveType.IconOverColor;

        /// <summary>
        /// Enumeration of move categories.
        /// </summary>
        public enum Category
        {
            Physical,
            Special,
            Status
        }

        /// <summary>
        /// Enumeration of the possible targets of the move.
        /// </summary>
        public enum PossibleTargets
        {
            Self,
            Adjacent,
            Allies,
            AdjacentAllies,
            Enemies,
            AdjacentEnemies,
            AllButSelf,
            All,
            AlliesAndSelf
        }
    }
}