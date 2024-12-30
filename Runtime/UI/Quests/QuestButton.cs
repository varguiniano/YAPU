using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Quests;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Quests
{
    /// <summary>
    /// Button that represents a quest.
    /// </summary>
    public class QuestButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the image icon.
        /// </summary>
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Reference to the name of the quest.
        /// </summary>
        [SerializeField]
        private TMP_Text QuestTitle;

        /// <summary>
        /// Icon when the quest is ongoing.
        /// </summary>
        [SerializeField]
        private Sprite OngoingIcon;

        /// <summary>
        /// Icon when the quest is completed.
        /// </summary>
        [SerializeField]
        private Sprite CompletedIcon;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the quest for this button.
        /// </summary>
        /// <param name="quest">Quest to represent.</param>
        /// <param name="status">Status of the quest.</param>
        public void SetQuest(Quest quest, QuestStatus status)
        {
            Icon.sprite = status.IsCompleted ? CompletedIcon : OngoingIcon;

            QuestTitle.SetText((quest.IsMainQuest
                                    ? "<color=#C8C72E>" + localizer["Quests/Main"] + "</color>"
                                    : localizer["Quests/Side"])
                             + ": "
                             + localizer[quest.LocalizableName]);
        }

        /// <summary>
        /// Factory class used for instantiation.
        /// </summary>
        public class Factory : GameObjectFactory<QuestButton>
        {
        }
    }
}