using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability WindRider.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/WindRider", fileName = "WindRider")]
    public class WindRider : Ability
    {
        /// <summary>
        /// Stats to raise when hit.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Stat, short> StatsToRaise;

        /// <summary>
        /// Moves that don't hit but trigger the ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> NonDamagingTriggeringMoves;

        /// <summary>
        /// Status that trigger the ability on etb.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<SideStatus> TriggeringSideStatuses;

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            foreach (SideStatus _ in TriggeringSideStatuses.Where(status => battleManager.Statuses.HasStatus(status,
                                                                      battleManager.Battlers
                                                                         .GetTypeAndIndexOfBattler(battler)
                                                                         .Type)))
            {
                ShowAbilityNotification(battler);

                foreach (KeyValuePair<Stat, short> statSlot in StatsToRaise)
                    yield return battleManager.BattlerStats.ChangeStatStage(battler,
                                                                            statSlot.Key,
                                                                            statSlot.Value,
                                                                            battler,
                                                                            this);
            }
        }

        /// <summary>
        /// Replace the move's effect when for raising the stat.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public override IEnumerator ShouldReplaceMoveEffectWhenHit(Battler owner,
                                                                   Move move,
                                                                   Battler user,
                                                                   BattleManager battleManager,
                                                                   Action<bool> callback)
        {
            if (!move.IsWindMove || owner == user)
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(owner);

            foreach (KeyValuePair<Stat, short> statSlot in StatsToRaise)
                yield return battleManager.BattlerStats.ChangeStatStage(owner,
                                                                        statSlot.Key,
                                                                        statSlot.Value,
                                                                        owner,
                                                                        this);

            callback.Invoke(false);
        }

        /// <summary>
        /// Trigger as if hit.
        /// </summary>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, List<(BattlerType Type, int Index)>>
                                                                      finished)
        {
            if (move.IsWindMove && NonDamagingTriggeringMoves.Contains(move))
            {
                ShowAbilityNotification(owner);

                foreach (KeyValuePair<Stat, short> statSlot in StatsToRaise)
                    yield return battleManager.BattlerStats.ChangeStatStage(owner,
                                                                            statSlot.Key,
                                                                            statSlot.Value,
                                                                            owner,
                                                                            this);
            }

            yield return base.OnOtherBattlerAboutToUseAMove(owner,
                                                            user,
                                                            move,
                                                            battleManager,
                                                            targets,
                                                            hasBeenReflected,
                                                            finished);
        }
    }
}