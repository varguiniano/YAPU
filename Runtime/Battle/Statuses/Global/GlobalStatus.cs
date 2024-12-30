using System.Collections;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Global
{
    /// <summary>
    /// Class representing a battle status that affects the entire battlefield.
    /// </summary>
    public abstract class GlobalStatus : LocalizableMonsterDatabaseScriptable<GlobalStatus>
    {
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
        /// Base localization root for the asset.
        /// </summary>
        protected override string BaseLocalizationRoot => "Status/Global/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Auto generate the localization based on the object name.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected override void RefreshLocalizableNames()
        {
            LocalizableName = BaseLocalizationRoot + name;
            LocalizableDescription = "NotUsed";
            LocalizableStatusStartKey = LocalizableName + "/Start";
            LocalizableStatusTickKey = LocalizableName + "/Tick";
            LocalizableStatusEndKey = LocalizableName + "/End";
        }

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">The one who added the status.</param>
        public virtual IEnumerator OnAddStatus(BattleManager battleManager, Battler user)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnTickStatus(BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnRemoveStatus(BattleManager battleManager)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusEndKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnBattleEnded(BattleManager battleManager)
        {
            yield break;
        }
        
        /// <summary>
        /// Does this status affect the order of each priority bracket?
        /// </summary>
        public virtual bool DoesInvertPriorityBracketOrder(BattleManager battleManager) => false;

        /// <summary>
        /// Called when calculating stats.
        /// Allows the status to rearrange them.
        /// Example: Wonder Room.
        /// </summary>
        public virtual SerializableDictionary<Stat, Stat> OnCalculateStatReplacement(MonsterInstance battler,
            BattleManager battleManager) =>
            new();

        /// <summary>
        /// Check if the battler is allowed to use their held item.
        /// </summary>
        /// <param name="battler">Battler owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual bool CanUseHeldItem(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Does this status ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if this status forces the monster to be grounded.</returns>
        public virtual bool IsGrounded(Battler battler, BattleManager battleManager) => false;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public virtual float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                               Battler user,
                                                               Battler target,
                                                               Move move) =>
            1;
    }
}