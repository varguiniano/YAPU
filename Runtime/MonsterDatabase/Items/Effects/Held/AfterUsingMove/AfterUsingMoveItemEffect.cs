using System;
using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterUsingMove
{
    /// <summary>
    /// Data class for held item effects that are called after the holder is uses a move.
    /// </summary>
    public abstract class AfterUsingMoveItemEffect : MonsterDatabaseScriptable<AfterUsingMoveItemEffect>
    {
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
        public virtual IEnumerator AfterHittingWithMove(Item item,
                                                        Move move,
                                                        Battler user,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        Action<bool> finished)
        {
            finished.Invoke(false);
            yield break;
        }

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
        public virtual IEnumerator AfterUsingMove(Item item,
                                                  Move move,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  Action<bool> finished)
        {
            finished.Invoke(false);
            yield break;
        }
    }
}