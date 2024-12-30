using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability MotorDrive.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MotorDrive", fileName = "MotorDrive")]
    public class MotorDrive : Ability
    {
        /// <summary>
        /// Reference to the type this ability replaces.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType ReplacedType;

        /// <summary>
        /// Stats to raise when hit.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Stat, short> StatsToRaise;

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
            if (move.GetMoveTypeInBattle(user, battleManager) != ReplacedType || owner == user)
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
    }
}