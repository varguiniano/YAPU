﻿using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Intimidate.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Intimidate", fileName = "Intimidate")]
    public class Intimidate : ETBStatLoweringAbility
    {
        /// <summary>
        /// Called after a wild level has been chosen on an encounter, last chance to prevent it.
        /// Ex: Keen eye.
        /// </summary>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="level">Level of the wild monster.</param>
        public override bool ShouldPreventEncounter(EncounterType encounterType, MonsterInstance owner, byte level) =>
            // https://bulbapedia.bulbagarden.net/wiki/Intimidate_(Ability)#Outside_of_battle
            owner.StatData.Level >= level + 5 && Random.value <= .5f;
    }
}