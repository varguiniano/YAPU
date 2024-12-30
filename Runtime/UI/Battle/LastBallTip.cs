using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.UI.Items;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Controller to show or hide the tip that displays the last used ball in the main battle menu.
    /// </summary>
    public class LastBallTip : HidableUiElement<LastBallTip>
    {
        /// <summary>
        /// Icon for the ball.
        /// </summary>
        [SerializeField]
        private ItemIcon BallIcon;

        /// <summary>
        /// Refresh the tip.
        /// </summary>
        public void Refresh(bool show, Ball ball)
        {
            BallIcon.SetIcon(ball);
            Show(show);
        }
    }
}