using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Behaviour to control the gender indicator in a UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class GenderIndicator : HidableUiElement<GenderIndicator>
    {
        /// <summary>
        /// Reference to the female sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite FemaleSprite;

        /// <summary>
        /// Reference to the male sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite MaleSprite;

        /// <summary>
        /// Reference to the non binary sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite NonBinarySprite;

        /// <summary>
        /// Set the gender to display.
        /// </summary>
        /// <param name="gender">Gender to display.</param>
        [Button]
        public void SetGender(MonsterGender gender) =>
            GetCachedComponent<Image>().sprite = gender switch
            {
                MonsterGender.Female => FemaleSprite,
                MonsterGender.Male => MaleSprite,
                MonsterGender.NonBinary => NonBinarySprite,
                _ => GetCachedComponent<Image>().sprite
            };
    }
}