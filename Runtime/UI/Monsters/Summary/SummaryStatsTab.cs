using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Controller for the summary stats tab.
    /// </summary>
    public class SummaryStatsTab : SummaryTab
    {
        /// <summary>
        /// Reference to the HP text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text HP;

        /// <summary>
        /// Reference to the Attack text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Attack;

        /// <summary>
        /// Reference to the Defense text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Defense;

        /// <summary>
        /// Reference to the SpecialAttack text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpecialAttack;

        /// <summary>
        /// Reference to the SpecialDefence text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpecialDefense;

        /// <summary>
        /// Reference to the Speed text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Speed;

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
        /// Set the monster stats and ability.
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            Dictionary<Stat, uint> stats = monster.GetStats(battleManager);

            HP.SetText(stats[Stat.Hp].ToString());
            Attack.SetText(stats[Stat.Attack].ToString());
            Defense.SetText(stats[Stat.Defense].ToString());
            SpecialAttack.SetText(stats[Stat.SpecialAttack].ToString());
            SpecialDefense.SetText(stats[Stat.SpecialDefense].ToString());
            Speed.SetText(stats[Stat.Speed].ToString());

            AbilityName.SetValue(monster.GetAbility().LocalizableName);
            if (AbilityDescription != null) AbilityDescription.SetValue(monster.GetAbility().LocalizableDescription);
        }
    }
}