using System;
using Varguiniano.YAPU.Runtime.World;

namespace Varguiniano.YAPU.Runtime.Quests
{
    /// <summary>
    /// Class that represents an objective inside a quest.
    /// </summary>
    [Serializable]
    public class Objective
    {
        /// <summary>
        /// Localization key for this objective.
        /// </summary>
        public string LocalizationKey = "Quests/QuestName/Objectives/Number";

        /// <summary>
        /// Map location of the objective.
        /// </summary>
        public SceneInfoAsset MapLocation;
    }
}