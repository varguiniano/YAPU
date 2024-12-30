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
    /// Data class for the ability Berserk.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Berserk", fileName = "Berserk")]
    public class Berserk : Ability
    {
        /// <summary>
        /// Threshold to activate the ability.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float Threshold = 0.5f;

        /// <summary>
        /// Changes to apply to stats when triggered.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, short> StatChanges;

        /// <summary>
        /// Raise stat if the user drops below the threshold.
        /// </summary>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler owner,
                                                   Battler user,
                                                   int damageDealt,
                                                   uint previousHP,
                                                   bool wasCritical,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   int hitNumber,
                                                   int expectedMoveHits,
                                                   BattleManager battleManager)
        {
            float hpThreshold = owner.GetStats(battleManager)[Stat.Hp] * Threshold;

            if (previousHP < hpThreshold || owner.CurrentHP >= hpThreshold) yield break;

            ShowAbilityNotification(owner);

            foreach (KeyValuePair<Stat, short> statChange in StatChanges)
                yield return battleManager.BattlerStats.ChangeStatStage(owner,
                                                                        statChange.Key,
                                                                        statChange.Value,
                                                                        owner,
                                                                        this);
        }
    }
}