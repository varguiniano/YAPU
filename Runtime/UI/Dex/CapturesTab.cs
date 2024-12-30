using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex tab that shows information related to the forms and genders of this monster that have been caught.
    /// </summary>
    public class CapturesTab : MonsterDexTab
    {
        /// <summary>
        /// First type of the monster.
        /// </summary>
        [SerializeField]
        private TypeBadge FirstType;

        /// <summary>
        /// First type of the monster.
        /// </summary>
        [SerializeField]
        private TypeBadge SecondType;

        /// <summary>
        /// Reference to the text with the forms seen.
        /// </summary>
        [SerializeField]
        private TMP_Text FormsSeen;

        /// <summary>
        /// Reference to the text with the forms caught.
        /// </summary>
        [SerializeField]
        private TMP_Text FormsCaught;

        /// <summary>
        /// Reference to the text with the forms caught.
        /// </summary>
        [SerializeField]
        private TMP_Text FormsTotal;

        /// <summary>
        /// Reference to the text with the Shiny forms seen.
        /// </summary>
        [SerializeField]
        private TMP_Text ShinyFormsSeen;

        /// <summary>
        /// Reference to the text with the Shiny forms caught.
        /// </summary>
        [SerializeField]
        private TMP_Text ShinyFormsCaught;

        /// <summary>
        /// Reference to the text with the Shiny forms caught.
        /// </summary>
        [SerializeField]
        private TMP_Text ShinyFormsTotal;

        /// <summary>
        /// Reference to the text with the Genders seen.
        /// </summary>
        [SerializeField]
        private TMP_Text GendersSeen;

        /// <summary>
        /// Reference to the text with the Genders caught.
        /// </summary>
        [SerializeField]
        private TMP_Text GendersCaught;

        /// <summary>
        /// Reference to the text with the Genders caught.
        /// </summary>
        [SerializeField]
        private TMP_Text GendersTotal;

        /// <summary>
        /// Reference to the text that tells if the current form has a shiny version.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro ShinyVersionInfo;

        /// <summary>
        /// Reference to the text that tells if the current form has a shiny version.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro GenderVariationInfo;

        /// <summary>
        /// Set the monster data in this tab.
        /// </summary>
        /// <param name="entry">Monster to display.</param>
        /// <param name="formEntry">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            FirstType.SetType(data.FirstType);
            SecondType.SetType(data.SecondType);

            FormsSeen.SetText(entry.NumberOfFormsWithoutShiniesAndGendersSeen.ToString("##00"));
            FormsCaught.SetText(entry.NumberOfFormsWithoutShiniesAndGendersCaught.ToString("##00"));
            FormsTotal.SetText(entry.NumberOfFormsWithoutShiniesAndGenders.ToString("##00"));

            ShinyFormsSeen.SetText(entry.NumberOfShinyFormsWithoutGendersSeen.ToString("##00"));
            ShinyFormsCaught.SetText(entry.NumberOfShinyFormsWithoutGendersCaught.ToString("##00"));
            ShinyFormsTotal.SetText(entry.NumberOfShinyFormsWithoutGenders.ToString("##00"));

            GendersSeen.SetText(entry.NumberOfGenderVariationsSeen.ToString("##00"));
            GendersCaught.SetText(entry.NumberOfGenderVariationsCaught.ToString("##00"));
            GendersTotal.SetText(entry.NumberOfGenderVariations.ToString("##00"));

            ShinyVersionInfo.SetValue(formEntry.Form.HasShinyVersion ? "Dex/HasShinyVersion" :
                                      formEntry.Form.IsShiny ? "Dex/IsShinyVersion" : "Dex/DoesntHaveShinyVersion");

            GenderVariationInfo.SetValue(data.HasGenderVariations
                                             ? "Dex/HasGenderVariations"
                                             : "Dex/DoesntHaveGenderVariations");
        }
    }
}