using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for a dex tab that displays a monster's evolutions.
    /// </summary>
    public class EvolutionsTab : MonsterDexTab
    {
        /// <summary>
        /// Audio to play when entering the menu.
        /// </summary>
        [SerializeField]
        private AudioReference EnterMenuAudio;

        /// <summary>
        /// Menu controller that allows viewing the info.
        /// </summary>
        [SerializeField]
        protected MonsterRelationshipMenu RelationshipMenu;

        /// <summary>
        /// Reference to the info panel.
        /// </summary>
        [SerializeField]
        private DexGenericInfoPanel InfoPanel;

        /// <summary>
        /// Reference to the tips shower.
        /// </summary>
        [SerializeField]
        private SingleDexTipsShower TipsShower;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the data from the monster into the tab.
        /// </summary>
        public override void SetData(MonsterDexEntry entry, FormDexEntry formEntry, MonsterGender gender, PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            List<DexMonsterRelationshipData> evolutionRelationships = new();

            foreach (EvolutionData evolution in data.Evolutions)
                evolutionRelationships.AddRange(evolution.GetDexRelationships(entry, formEntry, gender, localizer));

            evolutionRelationships.AddRange(data.GetMegaEvolutionDexRelationshipData(entry, formEntry, gender));

            RelationshipMenu.SetButtons(evolutionRelationships);
        }

        /// <summary>
        /// Subscribe to menu events.
        /// </summary>
        private void OnEnable()
        {
            RelationshipMenu.OnHovered += OnHovered;
            RelationshipMenu.OnBackSelected += OnBackSelected;
        }

        /// <summary>
        /// Unsubscribe from menu events.
        /// </summary>
        private void OnDisable()
        {
            RelationshipMenu.OnHovered -= OnHovered;
            RelationshipMenu.OnBackSelected -= OnBackSelected;
        }

        /// <summary>
        /// Enter the moves menu when the select button is pressed.
        /// </summary>
        public override void OnSelectPressedOnParentScreen()
        {
            if (RelationshipMenu.Data.Count == 0) return;

            audioManager.PlayAudio(EnterMenuAudio);
            RelationshipMenu.RequestInput();
            InfoPanel.Open();
            TipsShower.SwitchToSubmenu();
        }

        /// <summary>
        /// Called when an ability is hovered.
        /// </summary>
        /// <param name="index">Index hovered.</param>
        private void OnHovered(int index) =>
            InfoPanel.SetTexts("Dex/Evolution", RelationshipMenu.Data[index].LocalizableDescriptionKey);

        /// <summary>
        /// Called when back is selected in the menu.
        /// </summary>
        private void OnBackSelected()
        {
            InfoPanel.Close();
            RelationshipMenu.ReleaseInput();
            TipsShower.SwitchToGeneral();
        }
    }
}