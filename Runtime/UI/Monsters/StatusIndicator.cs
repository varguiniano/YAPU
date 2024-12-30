using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Controller to display the status of a monster on a UI icon.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class StatusIndicator : HidableUiElement<StatusIndicator>
    {
        /// <summary>
        /// Icon to use when fainted.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite FaintedIcon;

        /// <summary>
        /// Icon to use when there is no status.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite NoStatusIcon;

        /// <summary>
        /// Update the status with the given monster.
        /// </summary>
        /// <param name="monster">Reference to the monster to get the status from.</param>
        public void UpdateStatus(MonsterInstance monster)
        {
            if (monster.CurrentHP == 0)
            {
                GetCachedComponent<Image>().sprite = FaintedIcon;
                return;
            }

            if (monster.GetStatus() == null)
            {
                GetCachedComponent<Image>().sprite = NoStatusIcon;
                return;
            }

            GetCachedComponent<Image>().sprite = monster.GetStatus().Icon;
        }
    }
}