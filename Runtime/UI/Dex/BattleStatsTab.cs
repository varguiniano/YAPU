using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex tab that shows the battle stats of the monster.
    /// </summary>
    public class BattleStatsTab : MonsterDexTab
    {
        /// <summary>
        /// Reference to the EV yield text.
        /// </summary>
        [SerializeField]
        private TMP_Text EVYield;

        /// <summary>
        /// Reference to the XP yield text.
        /// </summary>
        [SerializeField]
        private TMP_Text XPYield;

        /// <summary>
        /// Reference to the catch rate text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro CatchRateText;

        /// <summary>
        /// Reference to the chance to run.
        /// </summary>
        [SerializeField]
        private TMP_Text WildRunChance;

        /// <summary>
        /// Reference to the items a wild one can hold.
        /// </summary>
        [SerializeField]
        private TMP_Text WildItemsText;

        /// <summary>
        /// Reference to the Poke Ball.
        /// </summary>
        [SerializeField]
        private Ball PokeBall;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the data from the monster into the tab.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            StringBuilder evYieldText = new();

            for (int i = 0; i < data.EVYield.Count; i++)
            {
                StatByteValuePair yield = data.EVYield[i];

                if (i != 0) evYieldText.Append(", ");
                evYieldText.Append(yield.Value);
                evYieldText.Append(" ");
                evYieldText.Append(localizer[yield.Stat.GetLocalizationString()]);
            }

            EVYield.SetText(evYieldText);

            XPYield.SetText(data.BaseExperience.ToString());

            CatchRateText.SetValue("Dex/CatchRate/Value",
                                   false,
                                   data.CatchRate.ToString(),
                                   (BattleUtils.CalculateCatchProbabilityOutOfBattle(data,
                                        PokeBall,
                                        playerCharacter.GlobalGameData)
                                  * 100)
                                  .ToString("#0.##"));

            WildRunChance.SetText((data.WildRunChance * 100).ToString("#0.##") + "%");

            StringBuilder wildItemsText = new();

            foreach (KeyValuePair<Item, float> chance in data.WildHeldItems)
            {
                wildItemsText.Append((chance.Value * 100).ToString("#0.##"));
                wildItemsText.Append("% ");
                wildItemsText.AppendLine(chance.Key.GetName(localizer));
            }

            WildItemsText.SetText(wildItemsText);
        }
    }
}