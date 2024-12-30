using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the tab that displays the base stats.
    /// </summary>
    public class BaseStatsTab : MonsterDexTab
    {
        /// <summary>
        /// Reference to the text for the total stats.
        /// </summary>
        [SerializeField]
        private TMP_Text TotalText;

        /// <summary>
        /// Reference to the text for the HP.
        /// </summary>
        [SerializeField]
        private TMP_Text HPText;

        /// <summary>
        /// Reference to the slider for the HP.
        /// </summary>
        [SerializeField]
        private Slider HPSlider;

        /// <summary>
        /// Reference to the text for the Attack.
        /// </summary>
        [SerializeField]
        private TMP_Text AttackText;

        /// <summary>
        /// Reference to the slider for the Attack.
        /// </summary>
        [SerializeField]
        private Slider AttackSlider;

        /// <summary>
        /// Reference to the text for the Defense.
        /// </summary>
        [SerializeField]
        private TMP_Text DefenseText;

        /// <summary>
        /// Reference to the slider for the Defense.
        /// </summary>
        [SerializeField]
        private Slider DefenseSlider;

        /// <summary>
        /// Reference to the text for the SpAttack.
        /// </summary>
        [SerializeField]
        private TMP_Text SpAttackText;

        /// <summary>
        /// Reference to the slider for the SpAttack.
        /// </summary>
        [SerializeField]
        private Slider SpAttackSlider;

        /// <summary>
        /// Reference to the text for the SpDefense.
        /// </summary>
        [SerializeField]
        private TMP_Text SpDefenseText;

        /// <summary>
        /// Reference to the slider for the SpDefense.
        /// </summary>
        [SerializeField]
        private Slider SpDefenseSlider;

        /// <summary>
        /// Reference to the text for the Speed.
        /// </summary>
        [SerializeField]
        private TMP_Text SpeedText;

        /// <summary>
        /// Reference to the slider for the Speed.
        /// </summary>
        [SerializeField]
        private Slider SpeedSlider;

        /// <summary>
        /// Reference to the growth rate text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro GrowthRateText;

        /// <summary>
        /// Display the info.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            TotalText.SetText(data.TotalBaseStats.ToString());

            byte hp = data.BaseStats[Stat.Hp];
            HPText.SetText(hp.ToString());
            HPSlider.value = hp;

            byte attack = data.BaseStats[Stat.Attack];
            AttackText.SetText(attack.ToString());
            AttackSlider.value = attack;

            byte defense = data.BaseStats[Stat.Defense];
            DefenseText.SetText(defense.ToString());
            DefenseSlider.value = defense;

            byte spAttack = data.BaseStats[Stat.SpecialAttack];
            SpAttackText.SetText(spAttack.ToString());
            SpAttackSlider.value = spAttack;

            byte spDefense = data.BaseStats[Stat.SpecialDefense];
            SpDefenseText.SetText(spDefense.ToString());
            SpDefenseSlider.value = spDefense;

            byte speed = data.BaseStats[Stat.Speed];
            SpeedText.SetText(speed.ToString());
            SpeedSlider.value = speed;

            GrowthRateText.SetValue(data.GrowthRate.GetLocalizableKey());
        }
    }
}