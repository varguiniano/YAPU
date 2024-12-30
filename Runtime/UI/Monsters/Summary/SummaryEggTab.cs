using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Summary tab to show egg info.
    /// </summary>
    public class SummaryEggTab : SummaryTab
    {
        /// <summary>
        /// Reference to the cycles text.
        /// </summary>
        [SerializeField]
        private TMP_Text CyclesText;

        /// <summary>
        /// Reference to the cycles bar.
        /// </summary>
        [SerializeField]
        private Slider CyclesBar;

        /// <summary>
        /// Reference to the text with extra info.
        /// </summary>
        [SerializeField]
        private TMP_Text ExtraText;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the info from the given monster.
        /// </summary>
        /// <param name="monster">Monster to set the info from.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            CyclesText.SetText(monster.EggData.EggCyclesLeft.ToString());
            CyclesBar.maxValue = monster.FormData.EggCycles;
            CyclesBar.value = monster.EggData.EggCyclesLeft;

            ExtraText.SetText(localizer[MonsterMathHelper.GetEggCyclesLocalizationString(monster)]);
        }
    }
}