using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for protecting the user.
    /// It also changes the stages of the attacker.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Obstruction", fileName = "Obstruction")]
    public class Obstruction : Protection
    {
        /// <summary>
        /// Stat changes it produces.
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        private SerializableDictionary<Stat, short> StatChanges;

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowUsedMessageNormally,
                                                         Action<bool> callback)
        {
            yield return base.OnAboutToBeHitByMove(target,
                                                   move,
                                                   battleManager,
                                                   user,
                                                   didShowUsedMessageNormally,
                                                   callback);

            if (!move.AffectedByProtect || !move.DoesMoveMakeContact(user, target, battleManager, false)) yield break;

            (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            foreach ((Stat stat, short value) in StatChanges)
                yield return battleManager.BattlerStats.ChangeStatStage(userType,
                                                                        userIndex,
                                                                        stat,
                                                                        value,
                                                                        targetType,
                                                                        targetIndex);
        }
    }
}