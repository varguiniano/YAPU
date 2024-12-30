using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for all abilities.
    /// </summary>
    public abstract class Ability : DocumentableMonsterDatabaseScriptable<Ability>
    {
        /// <summary>
        /// All localized strings of abilities should start with Abilities/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Abilities/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// Abilities that are immune to this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [SerializeField]
        [FoldoutGroup("Immunities")]
        protected List<Ability> AbilitiesImmuneTo;

        /// <summary>
        /// Is this an ignorable ability?
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Immunities")]
        public bool IsIgnorable;

        /// <summary>
        /// Show a notification with this ability's name and the icon of its owner.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="preventDuplicates">If there is already one in queue, don't add another.</param>
        public void ShowAbilityNotification(MonsterInstance owner, bool preventDuplicates = false) =>
            DialogManager.Notifications.QueueIconTextNotification(owner.GetIcon(),
                                                                  LocalizableName,
                                                                  dontDuplicate: preventDuplicates);

        /// <summary>
        /// Does this ability affect the user of a move, ability, etc on the battler that has this ability?
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <param name="owner">Owner of this ability.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it does.</returns>
        public bool AffectsUserOfEffect(Battler user,
                                        Battler owner,
                                        bool effectIgnoresAbilities,
                                        BattleManager battleManager) =>
            user == owner
         || user == null
         || !(user.CanUseAbility(battleManager, effectIgnoresAbilities)
           && AbilitiesImmuneTo.Contains(user.GetAbility()));

        #region Battle State Machine

        /// <summary>
        /// Called when the battler has started.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler the status is attached to.</param>
        public virtual IEnumerator OnBattleStarted(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler this ability is attached to.</param>
        /// <param name="battleManager"></param>
        public virtual IEnumerator OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called after the battle has happened.
        /// </summary>
        /// <param name="monster">The monster owner of the ability.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public virtual IEnumerator AfterBattle(MonsterInstance monster, ILocalizer localizer)
        {
            yield break;
        }

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public virtual IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Called when the monster is withdrawn from battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that left the battle.</param>
        public virtual IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Called to determine the priority of the battler inside its priority bracket.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Finished callback stating if it should go first (1), last (-1) or normal (0).</param>
        public virtual IEnumerator OnDeterminePriority(Battler battler,
                                                       BattleManager battleManager,
                                                       Action<int> finished)
        {
            finished.Invoke(0);
            yield break;
        }

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="user">User of the action.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterAction(Battler owner,
                                               BattleAction action,
                                               Battler user,
                                               BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public virtual IEnumerator AfterTurnPreStatus(Battler battler,
                                                      BattleManager battleManager,
                                                      ILocalizer localizer)
        {
            yield break;
        }

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public virtual IEnumerator AfterTurnPostStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            yield break;
        }

        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public virtual bool CanSwitch(Battler battler,
                                      BattleManager battleManager,
                                      BattlerType userType,
                                      int userIndex,
                                      Move userMove,
                                      Item item,
                                      bool itemBelongsToUser,
                                      bool showMessages) =>
            true;

        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="other">The battler trying to run away.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public virtual bool CanOpponentSwitch(Battler owner,
                                              Battler other,
                                              BattleManager battleManager,
                                              BattlerType userType,
                                              int userIndex,
                                              Move userMove,
                                              Item item,
                                              bool itemBelongsToUser,
                                              bool showMessages) =>
            true;

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>If it can run away, and if it overrides all other effects preventing run away.</returns>
        public virtual (bool, bool) CanRunAway(Battler battler, BattleManager battleManager, bool showMessages) =>
            (true, false);

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="other">The battler trying to run away.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>If it can run away, and if it overrides all other effects preventing run away.</returns>
        public virtual (bool, bool) CanOpponentMonsterRunAway(Battler owner,
                                                              Battler other,
                                                              BattleManager battleManager,
                                                              bool showMessages) =>
            (true, false);

        /// <summary>
        /// Called when the battler runs away.
        /// </summary>
        public virtual IEnumerator OnRunAway(Battler owner, BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region General Data

        /// <summary>
        /// Does this ability ignore others when using a move?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <returns>True if it ignores other abilities.</returns>
        public virtual bool IgnoresOtherAbilities(BattleManager battleManager, Battler owner, Move move) => false;

        /// <summary>
        /// Get the monster weight out of battle.
        /// </summary>
        /// <param name="monster">The owner of the ability.</param>
        /// <returns>The modifier and the multiplier to apply to the weight.</returns>
        public virtual (float, float) GetMonsterWeight(MonsterInstance monster) => (0, 1);

        /// <summary>
        /// Get the monster weight in battle.
        /// </summary>
        /// <param name="battler">The owner of the ability.</param>
        /// <returns>The modifier and the multiplier to apply to the weight.</returns>
        public virtual (float, float) GetMonsterWeightInBattle(Battler battler) => (0, 1);

        /// <summary>
        /// Can other monsters use their ability?
        /// </summary>
        public virtual bool CanOtherMonsterUseAbility(Battler owner, Battler other, BattleManager battleManager) =>
            true;

        /// <summary>
        /// Does this ability ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotificationIfPrevented"></param>
        /// <returns>True if this ability forces the monster to be grounded and true if this ability prevents grounding.</returns>
        public virtual (bool, bool) IsGrounded(Battler battler,
                                               BattleManager battleManager,
                                               bool showNotificationIfPrevented) =>
            (false, false);

        /// <summary>
        /// Does this ability bypass the substitute?
        /// </summary>
        /// <param name="targetType">Target of the effect.</param>
        /// <param name="targetIndex">Target of the effect.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect, owner of the ability.</param>
        /// <param name="userIndex">User of the effect, owner of the ability.</param>
        /// <param name="move">If the effect is a move, reference to it.</param>
        /// <returns>True if it bypasses.</returns>
        public virtual bool ByPassesSubstitute(BattlerType targetType,
                                               int targetIndex,
                                               BattleManager battleManager,
                                               BattlerType userType,
                                               int userIndex,
                                               Move move = null) =>
            false;

        /// <summary>
        /// Called when calculating a stat of the monster that has this ability.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public virtual float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) => 1;

        /// <summary>
        /// Called when calculating the accuracy stage of the monster that has this ability.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public virtual float OnCalculateAccuracyStage(Battler monster, BattleManager battleManager) => 1;

        /// <summary>
        /// Called when calculating the evasion stage of the monster that has this ability.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public virtual float OnCalculateEvasionStage(Battler monster, BattleManager battleManager) => 1;

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public virtual IEnumerator OnStatChange(Battler owner,
                                                Stat stat,
                                                short modifier,
                                                BattlerType userType,
                                                int userIndex,
                                                BattleManager battleManager,
                                                Ability changingAbility,
                                                Action<short> callback)
        {
            callback.Invoke(modifier);

            yield break;
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public virtual IEnumerator OnStatChange(Battler owner,
                                                BattleStat stat,
                                                short modifier,
                                                BattlerType userType,
                                                int userIndex,
                                                BattleManager battleManager,
                                                Action<short> callback)
        {
            callback.Invoke(modifier);

            yield break;
        }

        /// <summary>
        /// Callback after a stat stage has been changed.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterStatChanged(Battler owner,
                                                    Stat stat,
                                                    short modifier,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Callback after a stat stage has been changed.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterStatChanged(Battler owner,
                                                    BattleStat stat,
                                                    short modifier,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region Health

        /// <summary>
        /// Check if this ability should trigger force survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">Amount the HP is going to change. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        /// <returns>True if force survive should be triggered.</returns>
        public virtual bool ShouldForceSurvive(Battler owner,
                                               int amount,
                                               BattleManager battleManager,
                                               BattlerType userType,
                                               int userIndex,
                                               bool isSecondaryDamage,
                                               Move userMove = null) =>
            false;

        /// <summary>
        /// Called after the monster has survived, if it was this ability the one that made it survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">The actual amount of Hp that was changed. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        public virtual IEnumerator OnForceSurvive(Battler owner,
                                                  int amount,
                                                  BattleManager battleManager,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  bool isSecondaryDamage,
                                                  Move userMove = null)
        {
            yield break;
        }

        /// <summary>
        /// Calculate the multiplier to use this monster's HP is being drained.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="drainer">Monster draining the HP.</param>
        /// <returns>The multiplier to apply.</returns>
        public virtual float CalculateDrainerDrainHPMultiplier(Battler owner,
                                                               Battler drainer,
                                                               BattleManager battleManager) =>
            1;

        /// <summary>
        /// Called when another battler has fainted.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="otherType">Fainted battler.</param>
        /// <param name="otherIndex">Fainted battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnOtherBattlerFainted(Battler owner,
                                                         BattlerType otherType,
                                                         int otherIndex,
                                                         BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when this battler knocks out another battler.
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="owner">Owner of the status</param>
        /// <param name="otherType">Fainted battler.</param>
        /// <param name="otherIndex">Fainted battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnKnockedOutBattler(Battler owner,
                                                       BattlerType otherType,
                                                       int otherIndex,
                                                       BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when this battler is knocked out by another battler
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="userType">User of the effect.</param>
        /// <param name="userIndex">User of the effect.</param>
        /// <param name="userMove">Move used for the knock out, if any.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnKnockedOutByBattler(Battler owner,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         Move userMove,
                                                         BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region Moves

        /// <summary>
        /// Get the type of a move out of battle.
        /// </summary>
        /// <param name="move">Move to calculate.</param>
        /// <param name="monster">Owner of the move.</param>
        /// <param name="currentType">Current type of the move.</param>
        /// <returns>The type of the move.</returns>
        public virtual MonsterType GetMoveType(Move move, MonsterInstance monster, MonsterType currentType) =>
            currentType;

        /// <summary>
        /// Get the type of this a in battle.
        /// </summary>
        /// <param name="move">Move to calculate.</param>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="currentType">Current type of the move.</param>
        /// <returns>The type of the move.</returns>
        public virtual MonsterType GetMoveTypeInBattle(Move move,
                                                       Battler battler,
                                                       BattleManager battleManager,
                                                       MonsterType currentType) =>
            currentType;

        /// <summary>
        /// Called to determine the priority of a move the owner is going to use.
        /// </summary>
        /// <param name="move">Move.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="currentPriority">Priority of the move before modifications.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications for abilities or items that modify the priority.</param>
        /// <returns>Modifier to apply to the priority.</returns>
        public virtual int GetMovePriorityModifier(Move move,
                                                   Battler owner,
                                                   List<Battler> targets,
                                                   int currentPriority,
                                                   BattleManager battleManager,
                                                   bool showNotifications) =>
            0;

        /// <summary>
        /// Is this monster immune to the given move for reasons other than those moves immunities?
        /// Example: Immunity to sound moves.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move to check.</param>
        /// <param name="userType">User of that move.</param>
        /// <param name="userIndex">User of that move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if immune.</returns>
        public virtual bool IsImmuneToMove(Battler owner,
                                           Move move,
                                           BattlerType userType,
                                           int userIndex,
                                           BattleManager battleManager) =>
            false;

        /// <summary>
        /// Allow the user of a move to modify the effectiveness of a move when attacking.
        /// </summary>
        public virtual void ModifyMultiplierOfTypesWhenAttacking(Battler owner,
                                                                 Battler target,
                                                                 Move move,
                                                                 BattleManager battleManager,
                                                                 ref float multiplier)
        {
        }

        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>Tuple stating if the number has been modified and the new value.</returns>
        public virtual (bool, int) GetNumberOfHitsOfMultihitMove(BattleManager battleManager,
                                                                 Battler owner,
                                                                 Move move,
                                                                 List<(BattlerType Type, int Index)> targets) =>
            (false, 1);

        /// <summary>
        /// Called to modify the effectiveness after doing the type calculation.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="move">Move used.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications when calculating?</param>
        /// <param name="effectiveness">Current effectiveness.</param>
        public virtual void ModifyEffectivenessAfterTypeCalculationWhenTargeted(Battler owner,
                                                                    Battler user,
                                                                    Move move,
                                                                    BattleManager battleManager,
                                                                    bool showNotifications,
                                                                    ref float effectiveness)
        {
        }

        /// <summary>
        /// Called when calculating a move's stab damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness">Move's effectiveness.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateStabDamageWhenUsing(DamageMove move,
                                                                  float multiplier,
                                                                  Battler user,
                                                                  Battler target,
                                                                  float effectiveness,
                                                                  bool isCritical,
                                                                  BattleManager battleManager,
                                                                  ILocalizer localizer,
                                                                  Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness">Move's effectiveness.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="hitNumber">Number of the current hit.</param>
        /// <param name="expectedHitNumber">Expected number of hits.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateMoveDamageWhenUsing(DamageMove move,
                                                                  float multiplier,
                                                                  Battler user,
                                                                  Battler target,
                                                                  float effectiveness,
                                                                  bool isCritical,
                                                                  int hitNumber,
                                                                  int expectedHitNumber,
                                                                  bool ignoresAbilities,
                                                                  List<(BattlerType Type, int Index)> allTargets,
                                                                  BattleManager battleManager,
                                                                  ILocalizer localizer,
                                                                  Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage when an ally uses it.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Ally using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness"></param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateMoveDamageWhenAllyUsing(Battler owner,
                                                                      DamageMove move,
                                                                      float multiplier,
                                                                      Battler user,
                                                                      Battler target,
                                                                      float effectiveness,
                                                                      bool isCritical,
                                                                      BattleManager battleManager,
                                                                      ILocalizer localizer,
                                                                      Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage on itself.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                     float multiplier,
                                                                     Battler user,
                                                                     Battler target,
                                                                     BattleManager battleManager,
                                                                     Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage on an ally.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateMoveDamageWhenAllyTargeted(Battler owner,
                                                                         DamageMove move,
                                                                         float multiplier,
                                                                         Battler user,
                                                                         Battler target,
                                                                         BattleManager battleManager,
                                                                         Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="modifier">Critical stage modifier to use.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Has the chance been changed?</returns>
        public virtual bool OnCalculateCriticalChance(Battler owner,
                                                      Battler target,
                                                      BattleManager battleManager,
                                                      Move move,
                                                      ref float multiplier,
                                                      ref byte modifier,
                                                      ref bool alwaysHit) =>
            false;

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Target of the move.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Multiplier to apply to the chance.</returns>
        public virtual float OnCalculateCriticalChanceWhenTargeted(Battler owner,
                                                                   Battler user,
                                                                   BattleManager battleManager,
                                                                   Move move,
                                                                   out bool alwaysHit)
        {
            alwaysHit = false;
            return 1;
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="ignoresAbilities"></param>
        /// <returns>A multiplier to apply to the power.</returns>
        public virtual float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                 Move move,
                                                                 Battler user,
                                                                 Battler target,
                                                                 bool ignoresAbilities) =>
            1;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public virtual float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                               Battler user,
                                                               Battler target,
                                                               Move move,
                                                               bool ignoresAbilities) =>
            1;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when being the target of a move.
        /// </summary>
        public virtual float GetMoveAccuracyMultiplierWhenTargeted(Battler owner,
                                                                   BattleManager battleManager,
                                                                   Battler user,
                                                                   Move move) =>
            1;

        /// <summary>
        /// Is this monster currently affect by burn damage reduction?
        /// </summary>
        public virtual bool IsAffectedByBurnDamageReduction(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Is this monster currently affect by the paralysis speed reduction?
        /// </summary>
        public virtual bool IsAffectedByParalysisSpeedReduction(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Is this monster affect by recoil?
        /// </summary>
        public virtual bool IsAffectedByRecoil(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Ignore the targets evasion when calculating move accuracy?
        /// </summary>
        public virtual bool IgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing() => false;

        /// <summary>
        /// Ignore the users accuracy when calculating move accuracy?
        /// </summary>
        public virtual bool IgnoreAccuracyWhenCalculatingMoveAccuracyWhenTargeted() => false;

        /// <summary>
        /// Ignore the users attack when targeted by a move?
        /// </summary>
        public virtual bool IgnoreAttackStageWhenWhenTargeted() => false;

        /// <summary>
        /// Ignore the targets defense when using a move?
        /// </summary>
        public virtual bool IgnoreDefenseStageWhenWhenUsingMove() => false;

        /// <summary>
        /// Does this ability bypass all accuracy checks when using a move?
        /// </summary>
        public virtual bool DoesBypassAllAccuracyChecksWhenUsing(Battler owner,
                                                                 Move move,
                                                                 Battler target,
                                                                 BattleManager battleManager) =>
            false;

        /// <summary>
        /// Does this ability bypass all accuracy checks when targeted by a move?
        /// </summary>
        public virtual bool DoesBypassAllAccuracyChecksWhenTargeted(Battler owner,
                                                                    Move move,
                                                                    Battler user,
                                                                    BattleManager battleManager) =>
            false;

        /// <summary>
        /// Called when calculating the move's accuracy and targeting the holder.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual (float Multiplier, bool ForceClamp, (int Min, int Max) Clamp) OnCalculateAccuracyWhenTargeted(
            Move move,
            Battler user,
            Battler target,
            BattleManager battleManager) =>
            (1, false, (0, 0));

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has the same move been reflected?</param>
        /// <param name="finished">Callback stating if the move will still be used, the new targets for the move.</param>
        public virtual IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                 Battler user,
                                                                 Move move,
                                                                 BattleManager battleManager,
                                                                 List<(BattlerType Type, int Index)> targets,
                                                                 bool hasBeenReflected,
                                                                 Action<bool, List<(BattlerType Type, int Index)>>
                                                                     finished)
        {
            finished.Invoke(true, targets);
            yield break;
        }

        /// <summary>
        /// Called when a move that targets this battler is about to execute its effect.
        /// Abilities like Lightning Rod or Flash Fire can replace this effect for something else
        /// and prevent the original effect from executing.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public virtual IEnumerator ShouldReplaceMoveEffectWhenHit(Battler owner,
                                                                  Move move,
                                                                  Battler user,
                                                                  BattleManager battleManager,
                                                                  Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Called to check if the battler can perform the secondary effect of a move when using it.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="battleManager"></param>
        /// <returns>True if it can.</returns>
        public virtual bool CanPerformSecondaryEffectOfMove(Battler owner,
                                                            List<(BattlerType Type, int Index)> targets,
                                                            Move move,
                                                            BattleManager battleManager) =>
            true;

        /// <summary>
        /// Get a multiplier to apply to the chance of the secondary effect of a move.
        /// Ex: Pledges or Serene Grace.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="battleManager"></param>
        /// <returns>The multiplier to apply.</returns>
        public virtual float GetMultiplierForChanceOfSecondaryEffectOfMove(Battler owner,
                                                                           List<(BattlerType Type, int Index)> targets,
                                                                           Move move,
                                                                           BattleManager battleManager) =>
            1;

        /// <summary>
        /// Is the monster affected by secondary effects of damage moves?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="attacker">Attacking battler.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="damageDealt">Damage that the move dealt.</param>
        /// <param name="battleManager"></param>
        /// <returns>True if it is affected.</returns>
        public virtual bool IsAffectedBySecondaryEffectsOfDamageMove(Battler owner,
                                                                     Battler attacker,
                                                                     DamageMove move,
                                                                     int damageDealt,
                                                                     BattleManager battleManager) =>
            true;

        /// <summary>
        /// Is the monster affected by secondary damage effects?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">Owner of the effect.</param>
        /// <param name="damageDealt">Damage that the move dealt.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it is affected.</returns>
        public virtual bool IsAffectedBySecondaryDamageEffects(Battler owner,
                                                               Battler user,
                                                               int damageDealt,
                                                               BattleManager battleManager) =>
            true;

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="move">Move they will use.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Move's targets.</param>
        /// <param name="ignoreStatus">Does the move ignore the battler's status?</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public virtual IEnumerator OnAboutToPerformMove(Move move,
                                                        Battler owner,
                                                        BattleManager battleManager,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        bool ignoreStatus,
                                                        bool ignoresAbilities,
                                                        Action<bool> finished)
        {
            yield break;
        }

        /// <summary>
        /// Called when the owner is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="finished">Callback stating the new effectiveness.</param>
        public virtual IEnumerator OnHitByMove(DamageMove move,
                                               float effectiveness,
                                               Battler owner,
                                               BattleManager battleManager,
                                               Battler moveUser,
                                               Action<float> finished)
        {
            finished.Invoke(effectiveness);
            yield break;
        }

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="damageDealt">Damage dealt by the move.</param>
        /// <param name="previousHP">HP it had before being hit.</param>
        /// <param name="wasCritical">Was it a critical hit?</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterHitByMove(DamageMove move,
                                                  float effectiveness,
                                                  Battler owner,
                                                  Battler user,
                                                  int damageDealt,
                                                  uint previousHP,
                                                  bool wasCritical,
                                                  bool substituteTookHit,
                                                  bool ignoresAbilities,
                                                  int hitNumber,
                                                  int expectedMoveHits,
                                                  BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called after the owner uses a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="owner">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterHittingWithMove(Move move,
                                                        Battler owner,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called after the owner uses a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="owner">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterUsingMove(Move move,
                                                  Battler owner,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region Statuses

        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAddStatus(Status status,
                                                BattlerType targetType,
                                                int targetIndex,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAddStatus(VolatileStatus status,
                                                BattlerType targetType,
                                                int targetIndex,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAnyMonsterAddStatus(Status status,
                                                          Battler owner,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAnyMonsterAddStatus(VolatileStatus status,
                                                          Battler owner,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Called when the a status is added to the owner.
        /// </summary>
        /// <param name="owner">Reference to the monster instance.</param>
        /// <param name="userType">Monster that caused the status.</param>
        /// <param name="userIndex">Monster that caused the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnStatusAddedInBattle(Battler owner,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         BattleManager battleManager)

        {
            yield break;
        }

        /// <summary>
        /// Called to modify the sleep countdown when added to this monster.
        /// </summary>
        /// <returns>Multiplier to apply to the sleep.</returns>
        public virtual float CalculateSleepCountDownMultiplier(Battler owner, BattleManager battleManager) => 1;

        /// <summary>
        /// Calculate the multiplier to apply when this mon takes poison damage.
        /// </summary>
        public virtual float CalculatePoisonDamageMultiplier(Battler owner, BattleManager battleManager) => 1;

        /// <summary>
        /// Called when the monster flinches.
        /// </summary>
        public virtual IEnumerator OnFlinched(Battler owner, BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region Item

        /// <summary>
        /// Check if this battler allows other battlers to use a held item.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="other">Battler attempting to use the item.</param>
        /// <param name="itemToUse">Item they want to use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual bool CanOtherMonsterUseHeldItem(Battler owner,
                                                       Battler other,
                                                       Item itemToUse,
                                                       BattleManager battleManager) =>
            true;

        /// <summary>
        /// Called when the monster is considering eating a berry.
        /// The ability may modify the HP threshold at which the berry is eaten.
        /// </summary>
        /// <param name="berry">Berry to eat.</param>
        /// <param name="threshold">Current threshold.</param>
        /// <param name="battler">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The new threshold.</returns>
        public virtual float ModifyBerryHPThreshold(Berry berry,
                                                    float threshold,
                                                    Battler battler,
                                                    BattleManager battleManager) =>
            threshold;

        /// <summary>
        /// Called when the battler eats a berry.
        /// </summary>
        /// <param name="berry">Berry eaten.</param>
        /// <param name="battler">Battler owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnBerryEaten(Berry berry, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        #endregion

        #region Scenario

        /// <summary>
        /// Does the weather have effect?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual bool DoesWeatherHaveEffect(Battler owner, BattleManager battleManager) => true;

        #endregion

        #region Out of battle

        /// <summary>
        /// Modify the minimum and maximum levels of an encounter.
        /// </summary>
        /// <param name="monster">Owner of the ability.</param>
        /// <param name="encounter">Encounter type.</param>
        /// <param name="minimum">Minimum level.</param>
        /// <param name="maximum">Maximum level.</param>
        /// <returns>The new limits.</returns>
        public virtual (byte minimum, byte maximum)
            ModifyEncounterLevels(MonsterInstance monster, EncounterType encounter, byte minimum, byte maximum) =>
            (minimum, maximum);

        /// <summary>
        /// Callback that allows the monster to modify the nature of an wild mon encounter.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="encounterType">Type of encounter.</param>
        /// <returns>The modified nature or null if not modified.</returns>
        public virtual Nature ModifyEncounterNature(MonsterInstance owner, EncounterType encounterType) => null;

        /// <summary>
        /// Modify the possible wild encounters that can be found.
        /// </summary>
        /// <param name="possibleEncounters">Current possible encounters.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="sceneInfo">Info for the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        public virtual void ModifyPossibleEncounters(ref List<WildEncounter> possibleEncounters,
                                                     MonsterInstance owner,
                                                     SceneInfo sceneInfo,
                                                     EncounterType encounterType)
        {
        }

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public virtual float OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            1;

        /// <summary>
        /// Called after a wild level has been chosen on an encounter, last chance to prevent it.
        /// Ex: Keen eye.
        /// </summary>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="level">Level of the wild monster.</param>
        public virtual bool ShouldPreventEncounter(EncounterType encounterType, MonsterInstance owner, byte level) =>
            false;

        /// <summary>
        /// Callback that allows this monster to modify the steps needed for an egg cycle.
        /// </summary>
        /// <returns>The modifier to apply.</returns>
        public virtual float ModifyStepsNeededForEggCycle(MonsterInstance owner) => 1;

        #endregion
    }
}