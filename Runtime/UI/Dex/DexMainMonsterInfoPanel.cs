using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the info panel that shows the main information inside the single monster dex screen.
    /// </summary>
    public class DexMainMonsterInfoPanel : WhateverBehaviour<DexMainMonsterInfoPanel>
    {
        /// <summary>
        /// Reference to the number text.
        /// </summary>
        [SerializeField]
        private TMP_Text Number;

        /// <summary>
        /// Reference to the monster icon.
        /// </summary>
        [SerializeField]
        private Image MonsterIcon;

        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Name;

        /// <summary>
        /// Reference to the form text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Form;

        /// <summary>
        /// Reference to the gender indicator.
        /// </summary>
        [SerializeField]
        private GenderIndicator GenderIndicator;

        /// <summary>
        /// Reference to the caught indicator.
        /// </summary>
        [SerializeField]
        private Image CaughtIndicator;

        /// <summary>
        /// Sprite to use when only seen.
        /// </summary>
        [SerializeField]
        private Sprite SeenSprite;

        /// <summary>
        /// Sprite to use when partially caught.
        /// </summary>
        [SerializeField]
        private Sprite CaughtSprite;

        /// <summary>
        /// Sprite to use when all forms caught.
        /// </summary>
        [SerializeField]
        private Sprite FullyCaughtSprite;

        /// <summary>
        /// Update the information displayed in the panel.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="formEntry"></param>
        /// <param name="gender"></param>
        public void UpdateInfo(MonsterDexEntry entry, FormDexEntry formEntry, MonsterGender gender)
        {
            Number.SetText(entry.Species.DexNumber.ToString("0000"));

            DataByFormEntry data = entry.Species[formEntry.Form];

            if (gender == MonsterGender.Male && data.HasMaleMaterialOverride)
                MonsterIcon.sprite = formEntry.Form.IsShiny ? data.IconShinyMale : data.IconMale;
            else
                MonsterIcon.sprite = formEntry.Form.IsShiny ? data.IconShiny : data.Icon;

            Name.SetValue(entry.Species.LocalizableName);

            Form.SetValue(formEntry.Form.LocalizableName);

            GenderIndicator.SetGender(gender);
            GenderIndicator.Show(data.HasMaleMaterialOverride);

            CaughtIndicator.sprite = entry.HasMonsterBeenCaught
                                         ? entry.AllFormsAndVariationsCaught
                                               ? FullyCaughtSprite
                                               : CaughtSprite
                                         : SeenSprite;
        }
    }
}