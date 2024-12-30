using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterUsingMove
{
    /// <summary>
    /// Data class for a held item effect that changes a stat stage after using a sound move
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterUsingMove/ChangeStatsAfterSoundMove",
                     fileName = "ChangeStatsAfterSoundMove")]
    public class ChangeStatAfterSoundMove : AfterUsingMoveItemEffect
    {
        /// <summary>
        /// Stat to be changed.
        /// </summary>
        [SerializeField]
        private Stat StatToChange;

        /// <summary>
        /// Amount to change the stat.
        /// </summary>
        [SerializeField]
        private short Amount = 1;

        /// <summary>
        /// Consume the item?
        /// </summary>
        [SerializeField]
        private bool ConsumeItem;

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
        public override IEnumerator AfterUsingMove(Item item,
                                                   Move move,
                                                   Battler user,
                                                   List<(BattlerType Type, int Index)> targets,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            if (move.SoundBased)
            {
                item.ShowItemNotification(user, localizer);
                
                (BattlerType userType, int userIndex) =
                    battleManager.Battlers.GetTypeAndIndexOfBattler(user);

                yield return battleManager.BattlerStats.ChangeStatStage(userType,
                                                                        userIndex,
                                                                        StatToChange,
                                                                        Amount,
                                                                        userType,
                                                                        userIndex);
            }

            finished?.Invoke(move.SoundBased && ConsumeItem);
        }
    }
}