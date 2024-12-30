using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for a volatile status a monster can have.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Inert", fileName = "InertVolatileStatus")]
    public class VolatileStatus : MonsterDatabaseScriptable<VolatileStatus>
    {
        /// <summary>
        /// Get the localization key for this status' name.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LocalizableNameKey;

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
        /// Minimum random countdown.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private int MinCountdown = 2;

        /// <summary>
        /// Maximum random countdown.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private int MaxCountdown = 5;

        /// <summary>
        /// Does this status persist even when switching?
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        public bool PersistsWhenSwitching;

        /// <summary>
        /// Is this status removed when the form changes?
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        public bool RemovedOnFormChange;

        /// <summary>
        /// Auto generate the localization based on the object name.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected virtual void GenerateLocalization()
        {
            LocalizableNameKey = "Status/Volatile/" + name;
            LocalizableStatusStartKey = LocalizableNameKey + "/Start";
            LocalizableStatusTickKey = LocalizableNameKey + "/Tick";
            LocalizableStatusEndKey = LocalizableNameKey + "/End";
        }

        #region State Machine

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public virtual IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        public virtual IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public virtual IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                  Battler battler,
                                                  bool playAnimation = true)
        {
            if (playAnimation)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusEndKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Calculate the random countdown of the status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        public int CalculateRandomCountdown(BattleManager battleManager,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex)
        {
            int modifier = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex)
                                        .CalculateRandomCountdownOfVolatileStatus(battleManager,
                                             this,
                                             targetType,
                                             targetIndex);

            return modifier != -2 ? modifier : battleManager.RandomProvider.Range(MinCountdown, MaxCountdown + 1);
        }

        /// <summary>
        /// Called when another monster is withdrawn from battle.
        /// </summary>
        /// <param name="owner">Owner of the volatile status.</param>
        /// <param name="other">Monster leaving.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnOtherMonsterLeavingBattle(Battler owner,
                                                               Battler other,
                                                               BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when this status is passed from one battler to another.
        /// For example with Baton Pass.
        /// </summary>
        /// <param name="oldOwner">Old owner of the status.</param>
        /// <param name="newOwner">New owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual void OnMonsterChanged(Battler oldOwner, Battler newOwner, BattleManager battleManager)
        {
        }

        /// <summary>
        /// Force the battler to use a specific action.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battler manager.</param>
        /// <param name="battleAction">Generated battle action.</param>
        public virtual bool RequestForcedAction(Battler battler,
                                                BattleManager battleManager,
                                                out BattleAction battleAction)
        {
            battleAction = default;
            return false;
        }

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
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
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public virtual IEnumerator OnBattleEnded(Battler battler)
        {
            yield break;
        }

        #endregion

        #region General Data

        /// <summary>
        /// Get the name of the monster.
        /// </summary>
        /// <param name="battler">Battler to get the name from.</param>
        /// <returns>A non empty string if it has been overriden.</returns>
        public virtual string GetMonsterName(Battler battler) => "";

        /// <summary>
        /// Get the monster weight in battle.
        /// </summary>
        /// <param name="battler">The owner of the status.</param>
        /// <returns>The modifier to apply to the weight and a full override of the weight.</returns>
        public virtual (float, float) GetMonsterWeightInBattle(Battler battler) => (0, 0);
        
        /// <summary>
        /// Get the monster height in battle.
        /// </summary>
        /// <param name="battler">The owner of the status.</param>
        /// <returns>The modifier to apply to the weight and a full override of the height.</returns>
        public virtual (float, float) GetMonsterHeightInBattle(Battler battler) => (0, 0);

        /// <summary>
        /// Used to override the ability of the mon when retrieved.
        /// </summary>
        /// <param name="battler">Battler to get the ability from.</param>
        /// <returns>Overriding ability or null if it doesn't override.</returns>
        public virtual Ability OnGetAbility(Battler battler) => null;

        /// <summary>
        /// Can this monster use its ability in battle?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can.</returns>
        public virtual bool CanUseAbility(Battler owner, BattleManager battleManager) => true;

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        /// <param name="overrideBaseValue">If > 0 override the base value with this value.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public virtual float OnCalculateStat(Battler monster, Stat stat, out uint overrideBaseValue)
        {
            overrideBaseValue = 0;
            return 1;
        }

        /// <summary>
        /// Called when the monster types are being calculated.
        /// </summary>
        /// <param name="battler">Reference to the monster.</param>
        /// <param name="currentCalculatedFirst">First type already calculated.</param>
        /// <param name="currentCalculatedSecond">Second type already calculated.</param>
        /// <returns>The new calculated types.</returns>
        public virtual (MonsterType, MonsterType) OnCalculateTypes(Battler battler,
                                                                   MonsterType currentCalculatedFirst,
                                                                   MonsterType currentCalculatedSecond) =>
            (currentCalculatedFirst, currentCalculatedSecond);

        /// <summary>
        /// Does this status ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>True if this status forces the monster to be grounded and true if this status prevents grounding.</returns>
        public virtual (bool, bool) IsGrounded(Battler battler) => (false, false);

        /// <summary>
        /// Get the monster's catch rate.
        /// </summary>
        /// <param name="battler">Owner of the status.</param>
        /// <param name="current">Its current calculated catch rate.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>New catch rate.</returns>
        public virtual byte GetCatchRate(Battler battler, byte current, BattleManager battleManager) => current;

        /// <summary>
        /// Can this monster be caught by a ball?
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can be caught.</returns>
        public virtual bool CanBeCaught(Battler battler, BattleManager battleManager) => true;
        
        /// <summary>
        /// Can the battler change form?
        /// </summary>
        public virtual bool CanChangeForm(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Can the battler transform?
        /// </summary>
        public virtual bool CanTransform(Battler battler, BattleManager battleManager) => true;

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
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public virtual bool CanRunAway(Battler battler, BattleManager battleManager, bool showMessages) => true;

        #endregion

        #region Health

        /// <summary>
        /// Called when this battler is knocked out by another battler
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="owner">Owner of the status</param>
        /// <param name="userType">User of the effect.</param>
        /// <param name="userIndex">User of the effect.</param>
        /// <param name="userMove">Move that knocked out, if any.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnKnockedOutByBattler(Battler owner,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         Move userMove,
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
        /// Can this monster heal?
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can.</returns>
        public virtual bool CanHeal(Battler owner, BattleManager battleManager) => true;

        #endregion

        #region Moves

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public virtual List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) => usableMoves;

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="other">Reference to the other battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The new list of usable moves.</returns>
        public virtual List<MoveSlot> OnRetrieveUsableMovesForOtherBattler(Battler owner,
                                                                           Battler other,
                                                                           List<MoveSlot> usableMoves,
                                                                           BattleManager battleManager) =>
            usableMoves;

        /// <summary>
        /// Does this status bypass all accuracy checks when using a move?
        /// </summary>
        public virtual bool DoesBypassAllAccuracyChecksWhenUsing(Battler owner,
                                                                 Move move,
                                                                 Battler target,
                                                                 BattleManager battleManager) =>
            false;

        /// <summary>
        /// Does this status bypass all accuracy checks when being targeted by a move?
        /// </summary>
        public virtual bool DoesBypassAllAccuracyChecksWhenTargeted(Battler owner,
                                                                    Move move,
                                                                    Battler user,
                                                                    BattleManager battleManager) =>
            false;

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public virtual IEnumerator OnAboutToUseAMove(Battler battler,
                                                     Move move,
                                                     BattleManager battleManager,
                                                     List<(BattlerType Type, int Index)> targets,
                                                     Action<bool> finished)
        {
            finished.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has this move been reflected?</param>
        /// <param name="finished">Callback stating if the move will still be used, if the targets are modified and the new targets for the move.</param>
        public virtual IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                 Battler user,
                                                                 Move move,
                                                                 BattleManager battleManager,
                                                                 List<(BattlerType Type, int Index)> targets,
                                                                 bool hasBeenReflected,
                                                                 Action<bool, bool, List<(BattlerType Type, int Index)>>
                                                                     finished)
        {
            finished.Invoke(true, false, targets);
            yield break;
        }

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public virtual IEnumerator OnAboutToBeHitByMove(Battler target,
                                                        Move move,
                                                        BattleManager battleManager,
                                                        Battler user,
                                                        bool didShowUsedMessageNormally,
                                                        Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Called when the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finished">Callback stating the multiplier for the effectiveness and if it will force survive.</param>
        public virtual IEnumerator OnHitByMove(DamageMove move,
                                               float effectiveness,
                                               Battler battler,
                                               BattleManager battleManager,
                                               Battler moveUser,
                                               bool ignoresAbilities,
                                               Action<float, bool> finished)
        {
            finished.Invoke(1f, false);
            yield break;
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="owner">Owner of the status and move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public virtual float GetMovePowerMultiplierWhenUsingMove(Battler owner,
                                                                 Move move,
                                                                 Battler target,
                                                                 BattleManager battleManager) =>
            1;

        /// <summary>
        /// Get the power of a move that is going to hit this battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Battler being hit, owner of this status.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public virtual float GetMovePowerMultiplierWhenHit(BattleManager battleManager,
                                                           Move move,
                                                           Battler user,
                                                           Battler target) =>
            1;

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="battler">Owner of the ability.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Has the chance been changed?</returns>
        public virtual bool OnCalculateCriticalChance(Battler battler,
                                                      Battler target,
                                                      BattleManager battleManager,
                                                      Move move,
                                                      ref float multiplier,
                                                      ref bool alwaysHit) =>
            false;

        #endregion

        #region Items

        /// <summary>
        /// Check if the battler is allowed to use their held item.
        /// </summary>
        /// <param name="battler">Battler owner of the status.</param>
        public virtual bool CanUseHeldItem(Battler battler) => true;

        /// <summary>
        /// Check if the monster can use bag items.
        /// </summary>
        /// <returns>True if it can.</returns>
        public virtual bool CanUseBagItem(Battler battler, Item item, BattleManager battleManager) => true;

        #endregion

        #region Statuses

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="owner">Owner of the status.</param>
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
        /// <param name="owner">Owner of the status.</param>
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

        #endregion

        #region Scenario

        /// <summary>
        /// Does this status allow the monster to be affected by terrain?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="terrain">Terrain in place.</param>
        /// <returns>True if is affected by the terrain.</returns>
        public virtual bool IsAffectedByTerrain(Battler battler, Terrain.Terrain terrain) => true;

        #endregion
    }
}