using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding
{
    /// <summary>
    /// Data class for an item effect that modifies the number of IVs to pass on breeding.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/Breeding/OnCalculateSpecificIVsToPassOnBreeding",
                     fileName = "OnCalculateSpecificIVsToPassOnBreeding")]
    public class OnCalculateSpecificIVsToPassOnBreedingItemEffect : MonsterDatabaseScriptable<
        OnCalculateSpecificIVsToPassOnBreedingItemEffect>
    {
        /// <summary>
        /// IVs to pass.
        /// </summary>
        [SerializeField]
        private List<Stat> IVsToPass;

        /// <summary>
        /// Calculate the specific IVs to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The specific IVs that should be passed.</returns>
        public SerializableDictionary<Stat, byte> OnCalculateSpecificIVsToPassOnBreeding(MonsterInstance holder,
            MonsterInstance otherParent,
            MonsterEntry species,
            Form form,
            bool isMother)
        {
            SerializableDictionary<Stat, byte> toPass = new();

            foreach (Stat stat in IVsToPass) toPass[stat] = holder.StatData.IndividualValues[stat];

            return toPass;
        }
    }
}