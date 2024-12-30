using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex tab that shows the breeding information of the monster.
    /// </summary>
    public class BreedingDexTab : MonsterDexTab
    {
        /// <summary>
        /// Reference to the egg groups field.
        /// </summary>
        [SerializeField]
        private TMP_Text EggGroups;

        /// <summary>
        /// Reference to the egg cycles field.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro EggCycles;

        /// <summary>
        /// Reference to the text displaying unknown gender.
        /// </summary>
        [SerializeField]
        private HidableUiElement UnknownGender;

        /// <summary>
        /// Reference to the first possible gender.
        /// </summary>
        [SerializeField]
        private GenderIndicator FirstPossibleGender;

        /// <summary>
        /// Reference to the second possible gender.
        /// </summary>
        [SerializeField]
        private GenderIndicator SecondPossibleGender;

        /// <summary>
        /// Reference to the gender ratio field.
        /// </summary>
        [SerializeField]
        private TMP_Text GenderRatio;

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
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Set the data from the monster into the tab.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            string eggGroups = string.Empty;

            for (int i = 0; i < data.EggGroups.Count; i++)
            {
                if (i > 0) eggGroups += ", ";
                eggGroups += localizer[data.EggGroups[i].LocalizableName];
            }

            EggGroups.SetText(eggGroups);

            EggCycles.SetValue("Dex/EggCycles/Value",
                               false,
                               data.EggCycles.ToString(),
                               (data.EggCycles * settings.StepsPerEggCycle).ToString());

            string genderRatio = "100% " + localizer["Common/Unknown"];
            SecondPossibleGender.Show(false);

            if (data.HasBinaryGender)
            {
                if (data.FemaleRatio > 0)
                {
                    FirstPossibleGender.SetGender(MonsterGender.Female);

                    genderRatio = (data.FemaleRatio * 100).ToString("00.##") + "% " + localizer["Genders/Females"];

                    if (data.MaleRatio > 0)
                    {
                        SecondPossibleGender.SetGender(MonsterGender.Male);
                        SecondPossibleGender.Show();

                        genderRatio +=
                            ", " + (data.MaleRatio * 100).ToString("00.##") + "% " + localizer["Genders/Males"];
                    }
                }
                else
                {
                    FirstPossibleGender.SetGender(MonsterGender.Male);

                    genderRatio = (data.MaleRatio * 100).ToString("00.##") + "% " + localizer["Genders/Males"];
                }
            }

            UnknownGender.Show(!data.HasBinaryGender);
            FirstPossibleGender.Show(data.HasBinaryGender);
            GenderRatio.SetText(genderRatio);

            List<DexMonsterRelationshipData> breedingRelationships = new();

            foreach (BreedingData breedingData in data.BreedingData)
                breedingRelationships.AddRange(breedingData.GetDexRelationships(entry, formEntry));

            RelationshipMenu.SetButtons(breedingRelationships);
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
            InfoPanel.SetTexts("Dex/Relationships/Breeding", RelationshipMenu.Data[index].LocalizableDescriptionKey);

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