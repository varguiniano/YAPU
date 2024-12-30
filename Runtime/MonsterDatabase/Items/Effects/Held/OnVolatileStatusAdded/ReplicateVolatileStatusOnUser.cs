using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnVolatileStatusAdded
{
    /// <summary>
    /// Data class for a held item effect that replicated the volatile status just added on the one that added it.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnVolatileStatusAdded/ReplicateVolatileStatusOnUser",
                     fileName = "ReplicateVolatileStatusOnUser")]
    public class ReplicateVolatileStatusOnUser : OnVolatileStatusAddedItemEffect
    {
        /// <summary>
        /// Status to replicate.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private VolatileStatus Status;

        /// <summary>
        /// Called when a volatile status is added to the holder.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="item">Item that has this effect.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="userType">Type of the one that added the status.</param>
        /// <param name="userIndex">Index of the one that added the status.</param>
        /// <param name="status">Status added.</param>
        /// <param name="countdown">Countdown established.</param>
        /// <param name="finished">Callback establishing if the item should be consumed.</param>
        public override IEnumerator OnVolatileStatusAdded(BattleManager battleManager,
                                                          Item item,
                                                          Battler holder,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          VolatileStatus status,
                                                          int countdown,
                                                          Action<bool> finished)
        {
            if (status != Status || battleManager.Statuses.HasStatus(Status, userType, userIndex)) yield break;

            item.ShowItemNotification(holder, battleManager.Localizer);

            yield return DialogManager.ShowDialogAndWait("Items/Dialogs/ReplicateVolatileStatus",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        item.GetName(battleManager.Localizer),
                                                                        battleManager.Battlers
                                                                           .GetBattlerFromBattleIndex(userType,
                                                                                userIndex)
                                                                           .GetNameOrNickName(battleManager.Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            (BattlerType holderType, int holderIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(holder);

            yield return battleManager.Statuses.AddStatus(Status,
                                                          countdown,
                                                          userType,
                                                          userIndex,
                                                          holderType,
                                                          holderIndex,
                                                          false,
                                                          holder);
        }
    }
}