using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Behaviour to set the roster indicator in the battle UI.
    /// </summary>
    public class BattleRosterIndicator : HidableUiElement<BattleRosterIndicator>
    {
        /// <summary>
        /// Reference to the position when hidden.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform HiddenPosition;

        /// <summary>
        /// Reference to the position when shown.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ShownPosition;

        /// <summary>
        /// Reference to the actual panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform Panel;

        /// <summary>
        /// List of the 6 ball renderers.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<Image> BallRenderers;

        /// <summary>
        /// Reference to the ball icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite BallIcon;

        /// <summary>
        /// Reference to the gary ball icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite GrayBallIcon;

        /// <summary>
        /// Reference to the no ball icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Sprite NoBallIcon;

        /// <summary>
        /// Move the panel before showing or hiding.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        public override void Show(bool show = true)
        {
            if (show)
            {
                // ReSharper disable once BaseMethodCallWithDefaultParameter
                base.Show();
                Panel.DOLocalMove(ShownPosition.localPosition, .5f).SetEase(Ease.OutBack);
            }
            else
                Panel.DOLocalMove(HiddenPosition.localPosition, .5f)
                     .SetEase(Ease.InBack)
                     .OnComplete(() => base.Show(false));
        }

        /// <summary>
        /// Update the indicator with the roster.
        /// </summary>
        /// <param name="roster">Roster to use to update.</param>
        [Button]
        public void UpdateRoster(List<Battler> roster)
        {
            for (int i = 0; i < 6; ++i)
                BallRenderers[i].sprite =
                    roster.Count > i ? roster[i].CanBattle ? BallIcon : GrayBallIcon : NoBallIcon;
        }
    }
}