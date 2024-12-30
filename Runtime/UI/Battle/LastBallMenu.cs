using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Items;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Controller for the menu that can be open with the last used ball.
    /// </summary>
    public class LastBallMenu : MenuSelector, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the panel open position.
        /// </summary>
        [FoldoutGroup("Positions")]
        [SerializeField]
        private Transform OpenPosition;

        /// <summary>
        /// Reference to the panel closed position.
        /// </summary>
        [FoldoutGroup("Positions")]
        [SerializeField]
        private Transform ClosedPosition;

        /// <summary>
        /// Reference to the Ball name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text BallName;

        /// <summary>
        /// Reference to the Ball icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ItemIcon BallIcon;

        /// <summary>
        /// Reference to the number of balls.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text NumberOfBalls;

        /// <summary>
        /// Reference to the Ball name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text BallDescription;

        /// <summary>
        /// Reference to the left arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement LeftArrow;

        /// <summary>
        /// Reference to the main battle menu.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MainBattleMenu MainBattleMenu;

        /// <summary>
        /// Reference to the right arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement RightArrow;

        /// <summary>
        /// Reference to the ball category.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ItemCategory BallCategory;

        /// <summary>
        /// Called when the ball is selected.
        /// </summary>
        public Action<Ball> BallSelected;

        /// <summary>
        /// Reference to the attached transform.
        /// </summary>
        private Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [Inject]
        private PlayerSettings playerSettings;

        /// <summary>
        /// List of available balls.
        /// </summary>
        private List<Ball> availableBalls;

        /// <summary>
        /// Index of the current selected ball.
        /// </summary>
        private int currentBallIndex;

        /// <summary>
        /// Show or hide the panel.
        /// </summary>
        public override void Show(bool show = true) => StartCoroutine(show ? ShowRoutine() : HideRoutine());

        /// <summary>
        /// Routine to show the menu.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            transform.position = ClosedPosition.position;

            InputManager.BlockInput();

            yield return WaitAFrame;

            availableBalls = playerBag.GetItemsForCategory(BallCategory).Keys.Cast<Ball>().ToList();

            base.Show();

            currentBallIndex = availableBalls.IndexOf(globalGameData.LastUsedBall);

            yield return WaitAFrame;

            UpdateBall();

            if (availableBalls.Count == 1)
            {
                LeftArrow.Show(false);
                RightArrow.Show(false);
            }
            else
            {
                LeftArrow.Show();
                RightArrow.Show();
            }

            yield return Transform.DOMove(OpenPosition.position, .25f).SetEase(Ease.OutBack).WaitForCompletion();

            InputManager.BlockInput(false);

            yield return WaitAFrame;

            RequestInput();
        }

        /// <summary>
        /// Routine to hide the menu.
        /// </summary>
        private IEnumerator HideRoutine()
        {
            InputManager.BlockInput();

            yield return Transform.DOMove(ClosedPosition.position, .25f).SetEase(Ease.InBack).WaitForCompletion();

            InputManager.BlockInput(false);

            base.Show(false);
            
            ReleaseInput();

            yield return WaitAFrame;
        }

        /// <summary>
        /// Update the ball data.
        /// </summary>
        private void UpdateBall()
        {
            Ball ball = availableBalls[currentBallIndex];

            BallName.SetText(ball.GetName(localizer));
            BallIcon.SetIcon(ball);
            BallDescription.SetText(ball.GetDescription(localizer, playerSettings));
            NumberOfBalls.SetText("x" + playerBag.GetItemAmount(ball));

            // Update the ball if navigated.
            globalGameData.LastUsedBall = ball;
        }

        /// <summary>
        /// Close the panel.
        /// </summary>
        public override void OnExtra2(InputAction.CallbackContext context) => OnBack(context);

        /// <summary>
        /// Close the panel.
        /// </summary>
        public override void OnBack(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;
            StartCoroutine(GoBackRoutine());
        }

        /// <summary>
        /// Routine to go back.
        /// </summary>
        private IEnumerator GoBackRoutine()
        {
            AudioManager.PlayAudio(SelectAudio);

            yield return HideRoutine();

            MainBattleMenu.Show();
        }

        /// <summary>
        /// Switch balls.
        /// </summary>
        public override void OnNavigation(InputAction.CallbackContext context)
        {
            base.OnNavigation(context);

            switch (context.ReadValue<Vector2>().x)
            {
                case < 0:
                {
                    currentBallIndex--;

                    if (currentBallIndex == -1) currentBallIndex = availableBalls.Count - 1;

                    AudioManager.PlayAudio(NavigationAudio);

                    UpdateBall();
                    break;
                }
                case > 0:
                {
                    currentBallIndex++;

                    if (currentBallIndex == availableBalls.Count) currentBallIndex = 0;

                    AudioManager.PlayAudio(NavigationAudio);

                    UpdateBall();
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnSelect(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            AudioManager.PlayAudio(SelectAudio);

            Show(false);

            BallSelected.Invoke(availableBalls[currentBallIndex]);
        }
    }
}