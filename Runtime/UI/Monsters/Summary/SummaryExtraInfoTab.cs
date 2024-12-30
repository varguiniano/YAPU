using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Summary tab to show some extra info.
    /// </summary>
    public class SummaryExtraInfoTab : SummaryTab
    {
        /// <summary>
        /// Reference to the max text.
        /// </summary>
        [SerializeField]
        private TMP_Text MaxText;

        /// <summary>
        /// Reference to the max bar.
        /// </summary>
        [SerializeField]
        private Slider MaxBar;

        /// <summary>
        /// Reference to the text showing the form.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro FormText;

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
            MaxText.SetText(monster.MaxFormLevel.ToString());
            MaxBar.value = monster.MaxFormLevel;

            FormText.SetValue(monster.Form.LocalizableName);

            StringBuilder extra = new();

            if (monster.OriginData.IsAlpha)
            {
                extra.AppendLine(localizer["Monsters/Summary/Alpha"]
                                    .Replace("{0}", monster.GetNameOrNickName(localizer)));

                extra.AppendLine();
            }

            if (monster.VirusData.HasVirus)
                extra.AppendLine(localizer["Monsters/Summary/Virus"]);
            else if (monster.VirusData.IsImmune)
                extra.AppendLine(localizer["Monsters/Summary/VirusSurvived"]);
            else
                extra.AppendLine(localizer["Monsters/Summary/Healthy"]);

            extra.AppendLine();

            if (monster.StatData.Nature.GetLikedFlavour() != Flavour.Neutral)
            {
                extra.AppendLine(monster.StatData.Nature.GetLikedFlavour()
                                        .GetLikedFlavourLocalizedDescription(localizer));

                extra.AppendLine(monster.StatData.Nature.GetDislikedFlavour()
                                        .GetDislikedFlavourLocalizedDescription(localizer));
            }
            else
                extra.AppendLine(localizer["Flavour/NeutralSummary"]);

            ExtraText.SetText(extra);
        }
    }
}