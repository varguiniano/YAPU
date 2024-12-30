using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Controller of the monster summary info tab.
    /// </summary>
    public class SummaryInfoTab : SummaryTab
    {
        /// <summary>
        /// Reference to the origin region text.
        /// </summary>
        [SerializeField]
        private TMP_Text OriginRegion;

        /// <summary>
        /// Reference to the origin location text.
        /// </summary>
        [SerializeField]
        private TMP_Text OriginLocation;

        /// <summary>
        /// Reference to the origin trainer text.
        /// </summary>
        [SerializeField]
        private TMP_Text OriginTrainer;

        /// <summary>
        /// Reference to the origin level text.
        /// </summary>
        [SerializeField]
        private TMP_Text OriginLevel;

        /// <summary>
        /// Reference to the origin text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Origin;

        /// <summary>
        /// Reference to the height text.
        /// </summary>
        [SerializeField]
        private TMP_Text Height;

        /// <summary>
        /// Reference to the weight text.
        /// </summary>
        [SerializeField]
        private TMP_Text Weight;

        /// <summary>
        /// Set the info of the monster.
        /// </summary>
        /// <param name="monster">Monster reference.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            OriginRegion.SetText(monster.OriginData.Region);
            OriginLocation.SetText(monster.OriginData.Location);
            OriginTrainer.SetText(monster.OriginData.Trainer);
            OriginLevel.SetText(monster.OriginData.OriginalLevel.ToString());
            Origin.SetValue(monster.OriginData.OriginTypeLocalizationKey);

            if (battleManager != null)
                Height.SetText(((Battler) monster).GetHeight(battleManager, false).ToString("n2") + " m");
            else
                Height.SetText(monster.Height.ToString("n2") + " m");

            if (battleManager != null)
                Weight.SetText(((Battler) monster).GetWeight(battleManager, false).ToString("n2") + " Kg");
            else
                Weight.SetText(monster.Weight.ToString("n2") + " Kg");
        }
    }
}