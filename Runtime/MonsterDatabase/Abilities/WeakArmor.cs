using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability WeakArmor.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/WeakArmor", fileName = "WeakArmor")]
    public class WeakArmor : Ability
    {
        /// <summary>
        /// Stats to change when hit by a physical move.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, short> StageChanges;

        /// <summary>
        /// Change stages when hit by a physical move.
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
            if (move.GetMoveCategory(user, owner, ignoresAbilities, battleManager) != Move.Category.Physical)
                yield break;

            ShowAbilityNotification(owner);

            foreach (KeyValuePair<Stat, short> pair in StageChanges)
                yield return battleManager.BattlerStats.ChangeStatStage(owner, pair.Key, pair.Value, owner, this);
        }
    }
}