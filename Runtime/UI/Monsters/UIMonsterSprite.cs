using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Behaviours;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Behaviour to display a monster sprite.
    /// </summary>
    public class UIMonsterSprite : WhateverBehaviour<UIMonsterSprite>
    {
        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings yapuSettings;

        /// <summary>
        /// Set the sprite corresponding to that monster.
        /// </summary>
        /// <param name="monster">Monster to set.</param>
        /// <param name="front">Front or back sprite?</param>
        public void SetMonster(MonsterInstance monster, bool front = true) =>
            SetMonster(monster.Species,
                       monster.Form,
                       monster.PhysicalData.Gender,
                       monster.EggData.IsEgg,
                       monster.ExtraData.PersonalityValue,
                       front);

        /// <summary>
        /// Set the sprite from the species, form and gender.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="form">Monster form.</param>
        /// <param name="gender">Monster gender.</param>
        /// <param name="isEgg">Is this an egg?</param>
        /// <param name="personalityValue">Personality value of the monster.</param>
        /// <param name="front">Front or back sprite?</param>
        public void SetMonster(MonsterEntry species,
                               Form form,
                               MonsterGender gender,
                               bool isEgg,
                               int personalityValue,
                               bool front = true)
        {
            DataByFormEntry formData = species[form];

            GetCachedComponent<Image>().sprite = null;

            if (isEgg)
                GetCachedComponent<Image>().material = formData.EggMaterial;
            else if (front)
                if (form.IsShiny)
                    GetCachedComponent<Image>().material =
                        gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                            ? formData.FrontShinyMale
                            : formData.FrontShiny;
                else
                    GetCachedComponent<Image>().material =
                        gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                            ? formData.FrontMale
                            : formData.Front;
            else if (form.IsShiny)
                GetCachedComponent<Image>().material =
                    gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                        ? formData.BackShinyMale
                        : formData.BackShiny;
            else
                GetCachedComponent<Image>().material =
                    gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                        ? formData.BackMale
                        : formData.Back;

            MonsterMathHelper.AddAdditionalMaterialProperties(species,
                                                              form,
                                                              gender,
                                                              isEgg,
                                                              front,
                                                              personalityValue,
                                                              GetCachedComponent<Image>().materialForRendering,
                                                              yapuSettings);
        }
    }
}