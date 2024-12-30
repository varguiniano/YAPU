using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for an item effect that forces the user to always use the first move it used.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/ForceUseFirstMoveChosen",
                     fileName = "ForceUseFirstMoveChosen")]
    public class ForceUseFirstMoveChosen : PostActionCallbackItemEffect
    {
        /// <summary>
        /// Reference to the volatile status it needs to add.
        /// </summary>
        [SerializeField]
        private ForceUseMove VolatileStatus;

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterAction(Item item,
                                                Battler battler,
                                                BattleAction action,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            if (battler.HasVolatileStatus(VolatileStatus)) yield break;

            Battler actionPerformer =
                battleManager.Battlers.GetBattlerFromBattleIndex(action.BattlerType, action.Index);

            if (battler != actionPerformer) yield break;

            if (action.ActionType != BattleAction.Type.Move) yield break;

            int moveIndex = action.Parameters[0];

            if (moveIndex is < 0 or > 3) yield break;

            item.ShowItemNotification(battler, battleManager.Localizer);

            yield return battleManager.Statuses.AddStatus(VolatileStatus,
                                                          -1,
                                                          action.BattlerType,
                                                          action.Index,
                                                          action.BattlerType,
                                                          action.Index,
                                                          false,
                                                          battler.CurrentMoves[moveIndex].Move);
        }
    }
}