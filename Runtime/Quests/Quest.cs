using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Quests
{
    /// <summary>
    /// Class to define a quest the player has to achieve.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Quests/Quest", fileName = "Quest")]
    public class Quest : LocalizableMonsterDatabaseScriptable<Quest>
    {
        /// <summary>
        /// Base root for localization.
        /// </summary>
        protected override string BaseLocalizationRoot => "Quests/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// Is this a main or secondary quest?
        /// </summary>
        public bool IsMainQuest;

        /// <summary>
        /// NPC that gave the player the quest.
        /// </summary>
        public CharacterData QuestGiver;

        /// <summary>
        /// List of the quest objectives, in order.
        /// </summary>
        [ListDrawerSettings(ShowIndexLabels = true, CustomAddFunction = nameof(AddObjective))]
        public List<Objective> Objectives;

        /// <summary>
        /// Add an objective with the correct localization.
        /// </summary>
        private Objective AddObjective() =>
            new() { LocalizationKey = LocalizableName + "/Objectives/" + Objectives.Count };
    }
}