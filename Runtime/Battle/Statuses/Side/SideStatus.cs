using System;
using System.Collections;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Class representing a battle status that affects a whole side of the field.
    /// </summary>
    public abstract class SideStatus : MonsterDatabaseScriptable<SideStatus>
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
        /// Get the localization key for this status' dialog when it ends.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LocalizableStatusEndKey;

        /// <summary>
        /// Auto generate the localization based on the object name.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected virtual void GenerateLocalization()
        {
            LocalizableNameKey = "Status/Side/" + name;
            LocalizableStatusStartKey = LocalizableNameKey + "/Start";
            LocalizableStatusEndKey = LocalizableNameKey + "/End";
        }

        /// <summary>
        /// Play an animation when this status starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        /// <param name="extraData">Extra data provided when adding the status.</param>
        public virtual IEnumerator StartAnimation(BattleManager battleManager,
                                                  BattlerType side,
                                                  string sideOwner,
                                                  params object[] extraData)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         localizableModifiers: false,
                                                         modifiers: sideOwner,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        public virtual IEnumerator OnTickStatus(BattleManager battleManager, BattlerType side)
        {
            yield break;
        }

        /// <summary>
        /// Play an animation when this status ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        public virtual IEnumerator EndAnimation(BattleManager battleManager, BattlerType side, string sideOwner)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusEndKey,
                                                         localizableModifiers: false,
                                                         modifiers: sideOwner,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when a battler enters the battle on the side this status is in.
        /// </summary>
        /// <param name="side">Side of this status.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnBattlerEnteredSide(BattlerType side, int battlerIndex, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public virtual float OnCalculateStat(MonsterInstance monster, Stat stat) => 1;

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference tot he localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public virtual IEnumerator OnStatChange(BattlerType targetType,
                                                int targetIndex,
                                                Stat stat,
                                                short modifier,
                                                BattlerType userType,
                                                int userIndex,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<short> callback)
        {
            callback.Invoke(modifier);

            yield break;
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference tot he localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public virtual IEnumerator OnStatChange(BattlerType targetType,
                                                int targetIndex,
                                                BattleStat stat,
                                                short modifier,
                                                BattlerType userType,
                                                int userIndex,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<short> callback)
        {
            callback.Invoke(modifier);

            yield break;
        }

        /// <summary>
        /// Callback for when a critical stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference tot he localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public virtual IEnumerator OnCriticalStageChange(BattlerType targetType,
                                                         int targetIndex,
                                                         short modifier,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         BattleManager battleManager,
                                                         ILocalizer localizer,
                                                         Action<short> callback)
        {
            callback.Invoke(modifier);

            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier"></param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public virtual IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                     float multiplier,
                                                                     Battler user,
                                                                     Battler target,
                                                                     bool effectIgnoresAbilities,
                                                                     BattleManager battleManager,
                                                                     ILocalizer localizer,
                                                                     Action<float> finished)
        {
            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowMoveMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public virtual IEnumerator OnAboutToBeHitByMove(Battler target,
                                                        Move move,
                                                        BattleManager battleManager,
                                                        Battler user,
                                                        bool didShowMoveMessageNormally,
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
        /// Called just before checking if a battler is fainted.
        /// </summary>
        /// <param name="side">Side this status is on.</param>
        /// <param name="battlerIndex">Index of the battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns></returns>
        public virtual IEnumerator OnCheckFaintedBattler(BattlerType side,
                                                         int battlerIndex,
                                                         BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called to provide the player with additional price money at the end of the battle.
        /// </summary>
        public virtual uint GetPriceMoney() => 0;

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="side">Side the status is setup in.</param>
        public virtual IEnumerator OnBattleEnded(BattlerType side)
        {
            yield break;
        }
    }
}