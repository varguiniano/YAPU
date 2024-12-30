using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move Uproar.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Uproar", fileName = "UproarStatus")]
    public class UproarStatus : Fixation
    {
        /// <summary>
        /// Statuses that can't be added when any battler has this status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private List<Status> PreventedStatuses;

        /// <summary>
        /// Statuses that can't be added when any battler has this status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private List<VolatileStatus> PreventedVolatileStatuses;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
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
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer));
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
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
        public override IEnumerator CanAnyMonsterAddStatus(Status status,
                                                           Battler owner,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           BattleManager battleManager,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Action<bool> callback)
        {
            bool prevent = PreventedStatuses.Contains(status);

            if (prevent) yield return OnTickStatus(battleManager, owner);

            callback.Invoke(!prevent);
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
        public override IEnumerator CanAnyMonsterAddStatus(VolatileStatus status,
                                                           Battler owner,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           BattleManager battleManager,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Action<bool> callback)
        {
            bool prevent = PreventedVolatileStatuses.Contains(status);

            if (prevent) yield return OnTickStatus(battleManager, owner);

            callback.Invoke(!prevent);
        }
    }
}