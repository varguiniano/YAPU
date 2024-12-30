using System.Collections.Generic;
using System.Linq;
using AssetIcons;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.DataStructures;

#if UNITY_EDITOR
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Object that holds all the info about a monster type.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Type", fileName = "MonsterType")]
    public class MonsterType : MonsterDatabaseScriptable<MonsterType>
    {
        /// <summary>
        /// Localizable name of the type.
        /// </summary>
        public string LocalizableName = "Types/";

        /// <summary>
        /// Index in the database?
        /// </summary>
        public bool Index = true;

        /// <summary>
        /// Icon to display over the type color.
        /// </summary>
        [FoldoutGroup("UI")]
        [PreviewField(100)]
        [AssetIcon] // TODO: Optional.
        public Sprite IconOverColor;

        /// <summary>
        /// Main color of this type.
        /// </summary>
        [FoldoutGroup("UI")]
        public Color Color;

        /// <summary>
        /// Background color of this type.
        /// </summary>
        [FoldoutGroup("UI")]
        public Color BackgroundColor;

        /// <summary>
        /// Multipliers applied when attacking other types.
        /// </summary>
        [FoldoutGroup("Multipliers")]
        public SerializableDictionary<MonsterType, float> AttackMultipliers;

        #if UNITY_EDITOR
        /// <summary>
        /// Refresh the dictionary with all the other types in the assets folder.
        /// </summary>
        [FoldoutGroup("Multipliers")]
        [Button]
        public void RefreshMultipliersDictionary()
        {
            List<MonsterType> types = AssetManagementUtils.FindAssetsByType<MonsterType>();

            foreach (MonsterType type in types.Where(type => !AttackMultipliers.ContainsKey(type)))
                AttackMultipliers[type] = 1f;
        }
        #endif

        /// <summary>
        /// Does a monster having this move fly?
        /// </summary>
        [FoldoutGroup("Effects")]
        public bool PreventsGrounding;
    }
}