using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Moxie.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Moxie", fileName = "Moxie")]
    public class Moxie : Ability
    {
        /// <summary>
        /// Stat raises when knocking out a monster.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, short> StatRaisesOnKnockOut;

        /// <summary>
        /// Raise the stat.
        /// </summary>
        public override IEnumerator OnKnockedOutBattler(Battler owner,
                                                        BattlerType otherType,
                                                        int otherIndex,
                                                        BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            foreach (KeyValuePair<Stat, short> pair in StatRaisesOnKnockOut)
                yield return battleManager.BattlerStats.ChangeStatStage(owner, pair.Key, pair.Value, owner, this);
        }
    }
}