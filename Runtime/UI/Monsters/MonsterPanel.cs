using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.UI.Items;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Generic monster panel implementation.
    /// </summary>
    public class MonsterPanel : MonsterPanelBase
    {
        /// <summary>
        /// Reference to the monster icon.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Image MonsterIcon;

        /// <summary>
        /// Reference to the item icon.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private ItemIcon ItemIcon;

        /// <summary>
        /// Reference to the icon of the ball this monster was captured with.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private ItemIcon CapturedBallIcon;

        /// <summary>
        /// Update the icon.
        /// </summary>
        public override void UpdatePanel(float speed,
                                         bool playLowHealthSound = true,
                                         bool tween = false,
                                         Action finished = null)
        {
            if (MonsterIcon != null) MonsterIcon.sprite = GetMonster().GetIcon();

            if (ItemIcon != null) ItemIcon.SetIcon(GetMonster().EggData.IsEgg ? null : GetMonster().HeldItem);

            if (CapturedBallIcon != null)
                CapturedBallIcon.SetIcon(GetMonster().EggData.IsEgg ? null : GetMonster().OriginData.Ball);

            base.UpdatePanel(speed, playLowHealthSound, tween, finished);
        }
    }
}