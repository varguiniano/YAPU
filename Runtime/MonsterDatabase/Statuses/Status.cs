using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Object representing a monster's status.
    /// </summary>
    public abstract class Status : LocalizableMonsterDatabaseScriptable<Status>
    {
        /// <summary>
        /// All status should start with Status/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Status/";

        /// <summary>
        /// Get the localization key for this status' dialog when it starts.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LocalizableStatusStartKey;

        /// <summary>
        /// Get the localization key for this status' dialog when it ticks.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LocalizableStatusTickKey;

        /// <summary>
        /// Get the localization key for this status' dialog when it ends.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LocalizableStatusEndKey;

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// List of weathers that are immune to this status.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<Weather> ImmuneWeathers;

        /// <summary>
        /// List of terrains that are immune to this status.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        protected List<Terrain> ImmuneTerrains;

        /// <summary>
        /// List of types that are immune to this status.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<MonsterType> ImmuneTypes;

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
        /// Catch multiplier of this status.
        /// </summary>
        [FoldoutGroup("Effect")]
        public float CatchMultiplier = 1f;

        /// <summary>
        /// Status this status converts into when going out of battle.
        /// </summary>
        [FoldoutGroup("Effect")]
        public Status OutOfBattleDefault;

        /// <summary>
        /// Status icon.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [PreviewField]
        public Sprite Icon;

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected override void RefreshLocalizableNames()
        {
            base.RefreshLocalizableNames();
            LocalizableStatusStartKey = LocalizableName + "/Start";
            LocalizableStatusTickKey = LocalizableName + "/Tick";
            LocalizableStatusEndKey = LocalizableName + "/End";
        }

        /// <summary>
        /// Check if the status can be added to a monster.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster.</param>
        /// <param name="takeCurrentStatusIntoAccount">ake the current status into account?</param>
        /// <param name="user">Monster that added the effect. Only used in battle.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>True if it can be added.</returns>
        public virtual bool CanAddStatus(MonsterInstance monsterInstance,
                                         YAPUSettings settings,
                                         MonsterInstance user = null,
                                         bool takeCurrentStatusIntoAccount = true)
        {
            (MonsterType type1, MonsterType type2) = monsterInstance.GetTypes(settings);

            return monsterInstance.CanBattle
                && (monsterInstance.GetStatus() == null || !takeCurrentStatusIntoAccount)
                && !ImmuneTypes.Contains(type1)
                && !ImmuneTypes.Contains(type2)
                && (!monsterInstance.CanUseHeldItem() || !ImmuneHeldItems.Contains(monsterInstance.HeldItem));
        }

        /// <summary>
        /// Check if the status can be added to a monster.
        /// </summary>
        /// <param name="battler">Reference to that monster.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">Monster that added the effect.</param>
        /// <param name="ignoresAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="takeCurrentStatusIntoAccount">Take the current status into account?</param>
        /// <returns>True if it can be added.</returns>
        public virtual bool
            CanAddStatus(Battler battler,
                         BattleManager battleManager,
                         Battler user,
                         bool ignoresAbilities,
                         bool takeCurrentStatusIntoAccount = true) =>
            (!battleManager.Scenario.GetWeather(out Weather weather) || !ImmuneWeathers.Contains(weather))
         && !ImmuneTerrains.Contains(battleManager.Scenario.Terrain)
         && CanAddStatus(battler, battleManager.YAPUSettings, user, takeCurrentStatusIntoAccount)
         && (!battler.CanUseHeldItemInBattle(battleManager) || !ImmuneHeldItems.Contains(battler.HeldItem))
         && (!battler.CanUseAbility(battleManager, ignoresAbilities)
          || !ImmuneAbilities.Contains(battler.GetAbility())
          || !battler.GetAbility()
                     .AffectsUserOfEffect(user,
                                          battler,
                                          ignoresAbilities,
                                          battleManager));

        /// <summary>
        /// Called when the status is added out of battle.
        /// </summary>
        /// <param name="monsterInstance">Reference to the monster instance.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public virtual IEnumerator OnStatusAdded(MonsterInstance monsterInstance,
                                                 ILocalizer localizer,
                                                 bool showMessage = true)
        {
            if (showMessage)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                             localizableModifiers: false,
                                                             modifiers: monsterInstance.GetNameOrNickName(localizer));
        }

        /// <summary>
        /// Called when the status is added in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        /// <param name="extraData">Extra data when adding the status in battle.</param>
        public virtual IEnumerator OnStatusAddedInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool ignoresAbilities,
                                                         bool showMessage = true,
                                                         params object[] extraData)
        {
            if (showMessage)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Called when the status is ticked in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public virtual IEnumerator OnStatusTickInBattle(Battler battler,
                                                        BattleManager battleManager,
                                                        bool showMessage = true)
        {
            yield return CheckIfImmuneAndRemove(battler, battleManager);

            if (battler.GetStatus() == null) yield break;

            if (showMessage)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Called when the status is removed out of battle.
        /// </summary>
        /// <param name="monsterInstance">Reference to the monster instance.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public virtual IEnumerator OnStatusRemoved(MonsterInstance monsterInstance,
                                                   ILocalizer localizer,
                                                   bool showMessage = true)
        {
            if (showMessage)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusEndKey,
                                                             localizableModifiers: false,
                                                             modifiers: monsterInstance.GetNameOrNickName(localizer));
        }

        /// <summary>
        /// Called when the status is removed out of battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public virtual IEnumerator OnStatusRemovedInBattle(Battler battler,
                                                           BattleManager battleManager,
                                                           bool showMessage = true)
        {
            if (showMessage)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusEndKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Called when the battle has started.
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
        /// <param name="battler">Battler the status is attached to.</param>
        public virtual IEnumerator OnBattleEnded(Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnMonsterEnteredBattle(Battler battler, BattleManager battleManager)
        {
            yield return CheckIfImmuneAndRemove(battler, battleManager);
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public virtual IEnumerator OnAboutToPerformMove(Battler battler,
                                                        Move move,
                                                        BattleManager battleManager,
                                                        Action<bool> finished)
        {
            finished.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Called when calculating a stat of the monster that has this status.
        /// </summary>
        /// <param name="monster">Monster that has the status.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager, if in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public virtual float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) => 1;

        /// <summary>
        /// Called when calculating the damage of a move.
        /// </summary>
        /// <param name="move">Move being used.</param>
        /// <param name="user">USer of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier to apply to the move.</returns>
        public virtual float OnCalculateMoveDamage(DamageMove move, Battler user, BattleManager battleManager) => 1;

        /// <summary>
        /// Called before being hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        public virtual IEnumerator AboutToBeHitByMove(Move move,
                                                      Battler battler,
                                                      Battler user,
                                                      BattleManager battleManager,
                                                      bool didShowUsedMessageNormally)
        {
            yield break;
        }

        /// <summary>
        /// Called after being hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator AfterHitByMove(DamageMove move,
                                                  float effectiveness,
                                                  Battler battler,
                                                  Battler user,
                                                  BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Check if the battler is immune and remove the status if it is.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator CheckIfImmuneAndRemove(Battler battler, BattleManager battleManager)
        {
            if (!battleManager.Battlers.GetBattlersFighting().Contains(battler)) yield break;
            if (CanAddStatus(battler, battleManager, null, false, false)) yield break;
            yield return battler.RemoveStatusInBattle(battleManager);
            battleManager.Animation.UpdatePanels();
        }

        /// <summary>
        /// Play the status animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator PlayAnimation(Battler battler, BattleManager battleManager)
        {
            yield break;
        }
    }
}