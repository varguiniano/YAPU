using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Behaviours;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Controller for a simple monster sprite renderer.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MonsterSprite : WhateverBehaviour<MonsterSprite>
    {
        /// <summary>
        /// Reference to the monster property block.
        /// </summary>
        private MaterialPropertyBlock monsterPropertyBlock;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings yapuSettings;

        /// <summary>
        /// Instantiate the property blocks.
        /// </summary>
        private void OnEnable() => monsterPropertyBlock = new MaterialPropertyBlock();

        /// <summary>
        /// Set the sprite corresponding to that monster.
        /// </summary>
        /// <param name="monster">Monster to set.</param>
        /// <param name="front">Front or back sprite?</param>
        public void SetMonster(MonsterInstance monster, bool front = true) =>
            SetMonster(monster.Species, monster.Form, monster.PhysicalData.Gender, monster.EggData.IsEgg, front, monster.ExtraData.PersonalityValue);

        /// <summary>
        /// Set the sprite from the species, form and gender.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="form">Monster form.</param>
        /// <param name="gender">Monster gender.</param>
        /// <param name="isEgg">Is the monster an egg?</param>
        /// <param name="front">Front or back sprite?</param>
        /// <param name="personalityValue">Personality value of the monster.</param>
        public void SetMonster(MonsterEntry species, Form form, MonsterGender gender, bool isEgg, bool front, int personalityValue)
        {
            DataByFormEntry formData = species[form];

            if (isEgg)
                GetCachedComponent<SpriteRenderer>().sharedMaterial = formData.EggMaterial;
            else if (front)
                if (form.IsShiny)
                    GetCachedComponent<SpriteRenderer>().sharedMaterial =
                        gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                            ? formData.FrontShinyMale
                            : formData.FrontShiny;
                else
                    GetCachedComponent<SpriteRenderer>().sharedMaterial =
                        gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                            ? formData.FrontMale
                            : formData.Front;
            else if (form.IsShiny)
                GetCachedComponent<SpriteRenderer>().sharedMaterial =
                    gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                        ? formData.BackShinyMale
                        : formData.BackShiny;
            else
                GetCachedComponent<SpriteRenderer>().sharedMaterial =
                    gender == MonsterGender.Male && formData.HasMaleMaterialOverride
                        ? formData.BackMale
                        : formData.Back;

            GetCachedComponent<SpriteRenderer>().GetPropertyBlock(monsterPropertyBlock);

            MonsterMathHelper.AddAdditionalMaterialProperties(species,
                                                              form,
                                                              gender,
                                                              isEgg,
                                                              front,
                                                              personalityValue,
                                                              ref monsterPropertyBlock,
                                                              yapuSettings);

            GetCachedComponent<SpriteRenderer>().SetPropertyBlock(monsterPropertyBlock);
        }
    }
}