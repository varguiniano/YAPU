using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Tab to display the EVs of a monster in the summary.
    /// </summary>
    public class SummaryEVTab : SummaryTab
    {
        /// <summary>
        /// Reference to the HP text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text HPText;

        /// <summary>
        /// Reference to the HP bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider HPBar;

        /// <summary>
        /// Reference to the Attack text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text AttackText;

        /// <summary>
        /// Reference to the Attack bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider AttackBar;

        /// <summary>
        /// Reference to the Defense text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text DefenseText;

        /// <summary>
        /// Reference to the Defense bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider DefenseBar;

        /// <summary>
        /// Reference to the SpecialAttack text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpecialAttackText;

        /// <summary>
        /// Reference to the SpecialAttack bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider SpecialAttackBar;

        /// <summary>
        /// Reference to the SpecialDefense text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpecialDefenseText;

        /// <summary>
        /// Reference to the SpecialDefense bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider SpecialDefenseBar;

        /// <summary>
        /// Reference to the Speed text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpeedText;

        /// <summary>
        /// Reference to the Speed bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider SpeedBar;

        /// <summary>
        /// Reference to the Ability name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro AbilityName;

        /// <summary>
        /// Reference to the Ability description.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro AbilityDescription;

        /// <summary>
        /// Set the IVs and the ability of the monster.
        /// </summary>
        /// <param name="monster">Monster reference.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            byte hpValue = monster.StatData.EffortValues[Stat.Hp];
            HPText.SetText(hpValue.ToString());
            HPBar.value = hpValue;

            byte attackValue = monster.StatData.EffortValues[Stat.Attack];
            AttackText.SetText(attackValue.ToString());
            AttackBar.value = attackValue;

            byte defenseValue = monster.StatData.EffortValues[Stat.Defense];
            DefenseText.SetText(defenseValue.ToString());
            DefenseBar.value = defenseValue;

            byte specialAttackValue = monster.StatData.EffortValues[Stat.SpecialAttack];
            SpecialAttackText.SetText(specialAttackValue.ToString());
            SpecialAttackBar.value = specialAttackValue;

            byte specialDefenseValue = monster.StatData.EffortValues[Stat.SpecialDefense];
            SpecialDefenseText.SetText(specialDefenseValue.ToString());
            SpecialDefenseBar.value = specialDefenseValue;

            byte speedValue = monster.StatData.EffortValues[Stat.Speed];
            SpeedText.SetText(speedValue.ToString());
            SpeedBar.value = speedValue;

            AbilityName.SetValue(monster.GetAbility().LocalizableName);
            AbilityDescription.SetValue(monster.GetAbility().LocalizableDescription);
        }
    }
}