using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Class representing a dex monster button.
    /// </summary>
    public class DexMonsterButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the caught icon.
        /// </summary>
        [SerializeField]
        private Image CaughtIcon;

        /// <summary>
        /// Reference to the monster icon.
        /// </summary>
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Reference to the dex number.
        /// </summary>
        [SerializeField]
        private TMP_Text Number;

        /// <summary>
        /// Reference to the monster name.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Name;

        /// <summary>
        /// Sprite to use when the monster is not caught.
        /// </summary>
        [SerializeField]
        private Sprite EmptySprite;

        /// <summary>
        /// Sprite to use when the monster is caught.
        /// </summary>
        [SerializeField]
        private Sprite CaughtSprite;

        /// <summary>
        /// Sprite to use when the monster is unknown.
        /// </summary>
        [SerializeField]
        private Sprite UnknownSprite;

        /// <summary>
        /// Set the monster in the button based on the forms the player knows.
        /// </summary>
        /// <param name="monster">Monster to set.</param>
        /// <param name="formEntry">Form to set.</param>
        /// <param name="gender">Gender to set.</param>
        public void SetMonster(MonsterDexEntry monster, FormDexEntry formEntry, MonsterGender gender)
        {
            Number.SetText(monster.Species.DexNumber.ToString("0000"));

            CaughtIcon.sprite = monster.HasMonsterBeenCaught ? CaughtSprite : EmptySprite;

            if (monster.HasMonsterBeenSeen)
            {
                DataByFormEntry data = monster.Species[formEntry.Form];

                if (formEntry.Form.IsShiny)
                    Icon.sprite = gender == MonsterGender.Male && data.HasMaleMaterialOverride
                                      ? data.IconShinyMale
                                      : data.IconShiny;
                else
                    Icon.sprite = gender == MonsterGender.Male && data.HasMaleMaterialOverride
                                      ? data.IconMale
                                      : data.Icon;

                Name.SetValue(monster.Species.LocalizableName);
            }
            else
            {
                Icon.sprite = UnknownSprite;
                Name.Text.SetText("????");
            }
        }

        /// <summary>
        /// Factory class used for instantiation.
        /// </summary>
        public class Factory : GameObjectFactory<DexMonsterButton>
        {
        }
    }
}