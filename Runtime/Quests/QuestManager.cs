using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Quests
{
    /// <summary>
    /// Manager in charge of keeping track of the quests the player has been assigned, and their progress.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Quests/Manager", fileName = "QuestManager")]
    public class QuestManager : SavableObject
    {
        /// <summary>
        /// Game percentage will be calculated by completed quests/total quests in the game.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        [Tooltip("Game percentage will be calculated by completed quests/total quests in the game.")]
        private bool GamePercentageCalculatedAutomatically = true;

        /// <summary>
        /// Number of quests that will mark the 100%.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        [Tooltip("Number of quests that will mark the 100%.")]
        [HideIf(nameof(GamePercentageCalculatedAutomatically))]
        private int QuestsForHundo;

        /// <summary>
        /// Percentage for game completion.
        /// </summary>
        public float GameCompletionPercentage =>
            GamePercentageCalculatedAutomatically
                ? (float)questStatuses.Count(pair => pair.Value.IsCompleted) / worldDatabase.Quests.Count
                : (float)questStatuses.Count(pair => pair.Value.IsCompleted) / QuestsForHundo;

        /// <summary>
        /// Reference to the icon to use on quest notifications.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite NotificationIcon;

        /// <summary>
        /// Dictionary for status of all quests.
        /// If the quest isn't in the dictionary, it hasn't been started.
        /// </summary>
        [ShowInInspector]
        [HideInEditorMode]
        [PropertyOrder(-1)]
        private Dictionary<Quest, QuestStatus> questStatuses = new();

        /// <summary>
        /// Have the game percentage visible in the quests screen?
        /// </summary>
        [ShowInInspector]
        [HideInEditorMode]
        [PropertyOrder(-1)]
        public bool VisibleGamePercentage;

        /// <summary>
        /// Reference to the world database.
        /// </summary>
        [Inject]
        private WorldDatabase worldDatabase;

        /// <summary>
        /// Get all the quests.
        /// </summary>
        public Dictionary<Quest, QuestStatus> GetAllQuests() => questStatuses;

        /// <summary>
        /// Does the player have a quest ongoing or completed?
        /// </summary>
        public bool HasQuest(Quest quest) => questStatuses.ContainsKey(quest);

        /// <summary>
        /// Get the status of a quest.
        /// </summary>
        public QuestStatus GetStatus(Quest quest) => questStatuses[quest];

        /// <summary>
        /// Make the player start a quest.
        /// </summary>
        /// <param name="quest">Quest to start.</param>
        /// <param name="startingObjective">Objective for the quest to start with. Can be -1 for no current objective.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void StartQuest(Quest quest, int startingObjective = 0)
        {
            if (questStatuses.TryGetValue(quest, out QuestStatus status))
            {
                Logger.Warn(quest.name + " is already " + (status.IsCompleted ? "completed." : "in progress."));
                return;
            }

            questStatuses[quest] = new QuestStatus();

            DialogManager.Notifications.QueueIconTextNotification(NotificationIcon,
                                                                  "Quests/NewQuest",
                                                                  modifiers: quest.LocalizableName);

            if (startingObjective >= 0) SetQuestObjective(quest, startingObjective, false);
        }

        /// <summary>
        /// Set a new objective for a quest.
        /// </summary>
        /// <param name="quest">Quest to update the objective from.</param>
        /// <param name="objective">New objective to set.</param>
        /// <param name="showChangeNotification">Show the notification?</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void SetQuestObjective(Quest quest, int objective, bool showChangeNotification = true)
        {
            if (!questStatuses.ContainsKey(quest))
            {
                Logger.Warn(quest.name + " is not in progress.");
                return;
            }

            if (questStatuses[quest].IsCompleted)
            {
                Logger.Warn(quest.name + " has already been completed.");
                return;
            }

            if (questStatuses[quest].CurrentObjective == objective)
            {
                Logger.Warn(quest.name + " already has " + objective + " as the current objective.");
                return;
            }

            if (quest.Objectives.Count <= objective)
            {
                Logger.Warn(quest.name + " doesn't have an objective " + objective + ".");
                return;
            }

            questStatuses[quest].CurrentObjective = objective;

            if (objective >= 0 && showChangeNotification)
                DialogManager.Notifications.QueueIconTextNotification(NotificationIcon,
                                                                      "Quests/NewObjective",
                                                                      modifiers: quest.LocalizableName);
        }

        /// <summary>
        /// Complete the given quest.
        /// </summary>
        /// <param name="quest">Quest to complete.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void CompleteQuest(Quest quest)
        {
            if (!questStatuses.ContainsKey(quest))
            {
                Logger.Warn(quest.name + " is not in progress.");
                return;
            }

            if (questStatuses[quest].IsCompleted)
            {
                Logger.Warn(quest.name + " has already been completed.");
                return;
            }

            questStatuses[quest].IsCompleted = true;
            questStatuses[quest].CurrentObjective = -1;

            DialogManager.Notifications.QueueIconTextNotification(NotificationIcon,
                                                                  "Quests/CompletedQuest",
                                                                  modifiers: quest.LocalizableName);
        }

        /// <summary>
        /// Remove a quest from the dictionary, making it as it was never assigned to the player.
        /// </summary>
        /// <param name="quest">Quest to remove.</param>
        internal void ResetQuest(Quest quest)
        {
            if (questStatuses.ContainsKey(quest)) questStatuses.Remove(quest);
        }

        /// <summary>
        /// Save the quest progress.
        /// </summary>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableQuestManager(this));

        /// <summary>
        /// Load the quest progress.
        /// </summary>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableQuestManager saveData = serializer.From<SavableQuestManager>(data);

            saveData.LoadIntoManager(this, worldDatabase);

            yield break;
        }

        /// <summary>
        /// Reset the quest progress.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            questStatuses = new Dictionary<Quest, QuestStatus>();
            yield break;
        }

        /// <summary>
        /// Version of the quest manager that can be serialized to a file.
        /// </summary>
        [Serializable]
        public class SavableQuestManager
        {
            /// <summary>
            /// Dictionary for status of all quests.
            /// If the quest isn't in the dictionary, it hasn't been started.
            /// </summary>
            public SerializableDictionary<string, QuestStatus> QuestStatuses;

            /// <summary>
            /// Have the game percentage visible in the quests screen?
            /// </summary>
            public bool VisibleGamePercentage;

            /// <summary>
            /// Constructor from the runtime class.
            /// </summary>
            public SavableQuestManager(QuestManager manager)
            {
                VisibleGamePercentage = manager.VisibleGamePercentage;

                QuestStatuses = new SerializableDictionary<string, QuestStatus>();

                foreach (KeyValuePair<Quest, QuestStatus> questPair in manager.questStatuses)
                    QuestStatuses[questPair.Key.name] = questPair.Value;
            }

            /// <summary>
            /// Load the information back into the runtime class.
            /// </summary>
            public void LoadIntoManager(QuestManager manager, WorldDatabase worldDatabase)
            {
                manager.VisibleGamePercentage = VisibleGamePercentage;

                manager.questStatuses = new Dictionary<Quest, QuestStatus>();

                foreach (KeyValuePair<string, QuestStatus> questPair in QuestStatuses)
                    manager.questStatuses[worldDatabase.GetQuestByName(questPair.Key)] = questPair.Value;
            }
        }
    }
}