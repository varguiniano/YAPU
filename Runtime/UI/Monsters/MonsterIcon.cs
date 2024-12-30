using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Controller for a monster icon on the UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MonsterIcon : WhateverBehaviour<MonsterIcon>
    {
        /// <summary>
        /// Reference to an empty sprite.
        /// </summary>
        [SerializeField]
        private Sprite EmptySprite;

        /// <summary>
        /// Set the icon based on the monster instance.
        /// </summary>
        /// <param name="monster">The monster to set the icon from.</param>
        public void SetIcon(MonsterInstance monster) =>
            GetCachedComponent<Image>().sprite =
                monster == null || monster.IsNullEntry ? EmptySprite : monster.GetIcon();

        /// <summary>
        /// Set this icon from the minimal possible data.
        /// </summary>
        public void SetIcon(MonsterEntry monster, Form form, MonsterGender gender, bool isEgg) =>
            GetCachedComponent<Image>().sprite = monster[form].GetIcon(isEgg, form.IsShiny, gender);
    }
}