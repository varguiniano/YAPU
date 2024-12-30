using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Quests;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.UI.Quests
{
    /// <summary>
    /// Controller for the panel that displays the description of a quest.
    /// </summary>
    public class QuestDescription : HidableUiElement<QuestDescription>
    {
        /// <summary>
        /// Name of the quest.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro QuestName;

        /// <summary>
        /// Type of the quest.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro QuestType;

        /// <summary>
        /// Color to use when it's a main quest.
        /// </summary>
        [SerializeField]
        private Color MainQuestColor;

        /// <summary>
        /// Color to use when it's a side quest.
        /// </summary>
        [SerializeField]
        private Color SideQuestColor;

        /// <summary>
        /// Hider for the requester field.
        /// </summary>
        [SerializeField]
        private HidableUiElement RequesterHider;

        /// <summary>
        /// Icon of the requester.
        /// </summary>
        [SerializeField]
        private Image RequesterIcon;

        /// <summary>
        /// Name of the requester.
        /// </summary>
        [SerializeField]
        private TMP_Text RequesterName;

        /// <summary>
        /// Description of the quest.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Hider for the current objective.
        /// </summary>
        [SerializeField]
        private HidableUiElement ObjectiveHider;

        /// <summary>
        /// Field for the current objective.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Objective;

        /// <summary>
        /// Tip that prompts to open the map.
        /// </summary>
        [SerializeField]
        private HidableUiElement MapTip;

        /// <summary>
        /// Reference to the hider for the completed status.
        /// </summary>
        [SerializeField]
        private HidableUiElement CompletedHider;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the data for the given quest.
        /// </summary>
        public void SetData((Quest, QuestStatus) data)
        {
            (Quest quest, QuestStatus status) = data;

            QuestName.SetValue(quest.LocalizableName);
            QuestType.SetValue(quest.IsMainQuest ? "Quests/MainQuest" : "Quests/SideQuest");
            QuestType.Text.color = quest.IsMainQuest ? MainQuestColor : SideQuestColor;

            RequesterHider.Show(quest.QuestGiver != null);

            if (quest.QuestGiver != null)
            {
                RequesterIcon.sprite =
                    quest.QuestGiver.GetLooking(CharacterController.Direction.Down, false, false, false, false);

                RequesterName.SetText(quest.QuestGiver.GetLocalizedFullName(localizer));
            }

            Description.SetValue(quest.LocalizableDescription);

            ObjectiveHider.Show(!status.IsCompleted && status.CurrentObjective >= 0);

            if (status.CurrentObjective >= 0)
            {
                Objective.SetValue(quest.Objectives[status.CurrentObjective].LocalizationKey);
                MapTip.Show(quest.Objectives[status.CurrentObjective].MapLocation != null);
            }
            else
                MapTip.Show(false);

            CompletedHider.Show(status.IsCompleted);
        }
    }
}