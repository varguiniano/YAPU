using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.World.Encounters
{
    /// <summary>
    /// Class that defines a wild encounter.
    /// </summary>
    [Serializable]
    public class WildEncounter : MonsterDatabaseData
    {
        /// <summary>
        /// Monster to spawn.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        public MonsterEntry Monster;

        /// <summary>
        /// Calculator to generate the form.
        /// </summary>
        public EncounterFormCalculator FormCalculator;

        /// <summary>
        /// Minimum level of the encounter.
        /// </summary>
        [PropertyRange(1, 100)]
        [OnValueChanged(nameof(OnLevelChanged))]
        public byte MinLevel = 1;

        /// <summary>
        /// Maximum level of the encounter.
        /// </summary>
        [PropertyRange(1, 100)]
        [OnValueChanged(nameof(OnLevelChanged))]
        public byte MaxLevel = 100;

        /// <summary>
        /// The higher the weight, the more chances to appear.
        /// </summary>
        [Tooltip("The higher the weight, the more chances to appear.")]
        public float Weight;

        /// <summary>
        /// Called when the min level is changed to adjust the max one.
        /// </summary>
        private void OnLevelChanged() => MaxLevel = (byte)Mathf.Clamp(MaxLevel, MinLevel, 100);

        /// <summary>
        /// Get all monsters and forms possible for dex displaying.
        /// </summary>
        public List<(MonsterEntry, Form)> GetPossibleDexEncounters() =>
            FormCalculator.GetAllPossibleForms().Select(form => (Monster, form)).ToList();
    }
}