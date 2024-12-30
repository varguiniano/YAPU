using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex tab that displays other varied information.
    /// </summary>
    public class OtherInfoTab : MonsterDexTab
    {
        /// <summary>
        /// Reference to the base friendship text.
        /// </summary>
        [SerializeField]
        private TMP_Text BaseFriendship;

        /// <summary>
        /// Reference to the legendary text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro IsLegendary;

        /// <summary>
        /// Reference to the Mythical text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro IsMythical;

        /// <summary>
        /// Reference to the UltraBeast text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro IsUltraBeast;

        /// <summary>
        /// Reference to the TimesSeen text.
        /// </summary>
        [SerializeField]
        private TMP_Text TimesSeen;

        /// <summary>
        /// Reference to the TimesCaught text.
        /// </summary>
        [SerializeField]
        private TMP_Text TimesCaught;

        /// <summary>
        /// Reference to the TimesHatched text.
        /// </summary>
        [SerializeField]
        private TMP_Text TimesHatched;

        /// <summary>
        /// Display the info.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            BaseFriendship.SetText(data.BaseFriendship.ToString());
            IsLegendary.SetValue(data.IsLegendary ? "Common/True" : "Common/False");
            IsMythical.SetValue(data.IsMythical ? "Common/True" : "Common/False");
            IsUltraBeast.SetValue(data.IsUltraBeast ? "Common/True" : "Common/False");

            TimesSeen.SetText(formEntry.TotalSeen.ToString());
            TimesCaught.SetText(formEntry.TotalCaught.ToString());
            TimesHatched.SetText(formEntry.TotalHatched.ToString());
        }
    }
}