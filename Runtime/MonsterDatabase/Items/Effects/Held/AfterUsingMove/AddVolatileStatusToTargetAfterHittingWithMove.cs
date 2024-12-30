using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterUsingMove
{
    /// <summary>
    /// Data class for a held item effect that adds a volatile status to the target after hitting it with a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterUsingMove/AddVolatileStatusToTargetAfterHittingWithMove",
                     fileName = "AddVolatileStatusToTargetAfterHittingWithMove")]
    public class AddVolatileStatusToTargetAfterHittingWithMove : AfterUsingMoveItemEffect
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Chance to add the status.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float Chance;

        /// <summary>
        /// Countdown for the status.
        /// -1 = infinite.
        /// </summary>
        [SerializeField]
        private int Countdown = 1;

        /// <summary>
        /// Check if the move is affected by King's Rock.
        /// </summary>
        [SerializeField]
        private bool CheckAffectedByKingsRock;

        /// <summary>
        /// Called after the holder uses a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHittingWithMove(Item item,
                                                         Move move,
                                                         Battler user,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         BattleManager battleManager,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            if (CheckAffectedByKingsRock && !move.AffectedByKingsRock) yield break;

            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                if (battleManager.Statuses.HasStatus(Status, targetType, targetIndex)) yield break;

                float roll = battleManager.RandomProvider.Value01()
                           / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, move, battleManager);

                if (!(roll < Chance)) continue;
                item.ShowItemNotification(user, localizer);

                yield return battleManager.Statuses.AddStatus(Status,
                                                              Countdown,
                                                              targetType,
                                                              targetIndex,
                                                              userType,
                                                              userIndex,
                                                              false);
            }
        }
    }
}