using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the tab that shows the basic dex information of the monster.
    /// </summary>
    public class BasicDexInfoTab : MonsterDexTab
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
        /// Reference to the species text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro SpeciesText;

        /// <summary>
        /// Text to display the height.
        /// </summary>
        [SerializeField]
        private TMP_Text HeightText;

        /// <summary>
        /// Text to display the weight.
        /// </summary>
        [SerializeField]
        private TMP_Text WeightText;

        /// <summary>
        /// Description text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Reference to the monster cry.
        /// </summary>
        private AudioReference cry;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Set the monster data in this tab.
        /// </summary>
        /// <param name="entry">Monster to display.</param>
        /// <param name="formEntry">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            FirstType.SetType(data.FirstType);
            SecondType.SetType(data.SecondType);

            SpeciesText.SetValue(data.Species.LocalizableName);

            HeightText.SetText(data.Height.ToString("0.00") + " m");
            WeightText.SetText(data.Weight.ToString("0.00") + " kg");

            cry = data.Cry;

            Description.SetValue(data.DexDescriptionKey);
        }

        /// <summary>
        /// Play the monster cry.
        /// </summary>
        public override void OnSelectPressedOnParentScreen() =>
            StartCoroutine(audioManager.IsAudioPlaying(cry,
                                                       isPlaying =>
                                                       {
                                                           if (!isPlaying) audioManager.PlayAudio(cry);
                                                       }));
    }
}