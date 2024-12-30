using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.UI.Bags;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Shops
{
    /// <summary>
    /// Controller for the selector for the amount of items to buy in the shop.
    /// </summary>
    public class ShopAmountSelector : HidableUiElement<ShopAmountSelector>, IInputReceiver
    {
        /// <summary>
        /// Reference to the amount text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text AmountText;

        /// <summary>
        /// Reference to the price text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoneyText PriceText;

        /// <summary>
        /// Reference to the open position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform OpenPosition;

        /// <summary>
        /// Reference to the closed position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ClosedPosition;

        /// <summary>
        /// Reference to the up arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform UpArrow;

        /// <summary>
        /// Reference to the down arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform DownArrow;

        /// <summary>
        /// Reference to the choose audio.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference ChooseAudio;

        /// <summary>
        /// Reference to the select audio.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Can the navigation button be held to scroll faster?
        /// </summary>
        [FoldoutGroup("Input")]
        [SerializeField]
        public bool CanHoldToScrollFaster;

        /// <summary>
        /// Interval to navigate when holding.
        /// </summary>
        [FoldoutGroup("Input")]
        [SerializeField]
        public float HoldNavigationInterval = 0.05f;

        /// <summary>
        /// Cached reference to the transform.
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
        /// Flag to mark when the player chose.
        /// </summary>
        private bool playerChose;

        /// <summary>
        /// Player accepted the amount.
        /// </summary>
        private bool playerAccepted;

        /// <summary>
        /// Accessor for the current amount to buy that updates the texts.
        /// </summary>
        private uint CurrentAmount
        {
            get => currentAmount;
            set
            {
                currentAmount = value;
                AmountText.SetText(currentAmount.ToString());
                PriceText.SetMoney(CurrentAmount * price);
            }
        }

        /// <summary>
        /// Current amount to buy.
        /// </summary>
        private uint currentAmount;

        /// <summary>
        /// Max amount to be bought.
        /// </summary>
        private uint maxAmount;

        /// <summary>
        /// Price to buy at.
        /// </summary>
        private uint price;

        /// <summary>
        /// Cached value of the last navigation input.
        /// </summary>
        private Vector2 lastNavInput;

        /// <summary>
        /// Flag to know if the navigation is currently being held down.
        /// </summary>
        private bool navHolding;

        /// <summary>
        /// Routine to handle hold navigation.
        /// </summary>
        private Coroutine holdNavigationRoutineReference;

        /// <summary>
        /// Reference to the original up arrow position.
        /// </summary>
        private Vector3 originalUpArrowPosition;

        /// <summary>
        /// Reference to the original down arrow position.
        /// </summary>
        private Vector3 originalDownArrowPosition;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Request the player to choose an amount to buy.
        /// </summary>
        /// <param name="buyPrice">Price to buy at.</param>
        /// <param name="maxAmountToBuy">Maximum amount to buy.</param>
        /// <param name="callback">Callback stating the amount selected and if the player decided to buy (true) o cancelled (false).</param>
        public IEnumerator RequestAmount(uint buyPrice, uint maxAmountToBuy, Action<uint, bool> callback)
        {
            inputManager.BlockInput();

            Show(false);

            SetClosedPosition();

            originalUpArrowPosition = UpArrow.localPosition;
            originalDownArrowPosition = DownArrow.localPosition;

            playerChose = false;
            playerAccepted = false;

            price = buyPrice;
            maxAmount = maxAmountToBuy;

            CurrentAmount = 1;

            Show();

            yield return Transform.DOLocalMove(OpenPosition.localPosition, .25f)
                                  .SetEase(Ease.OutBack)
                                  .WaitForCompletion();

            inputManager.BlockInput(false);
            inputManager.RequestInput(this);

            yield return new WaitUntil(() => playerChose);

            inputManager.ReleaseInput(this);
            inputManager.BlockInput();

            callback.Invoke(CurrentAmount, playerAccepted);

            yield return Transform.DOLocalMove(ClosedPosition.localPosition, .25f)
                                  .SetEase(Ease.InBack)
                                  .WaitForCompletion();

            inputManager.BlockInput(false);

            Show(false);
        }

        /// <summary>
        /// Set the closed position immediately.
        /// </summary>
        public void SetClosedPosition() => Transform.localPosition = ClosedPosition.localPosition;

        /// <summary>
        /// Called when it starts receiving input.
        /// </summary>
        public void OnStateEnter() => holdNavigationRoutineReference ??= StartCoroutine(HoldNavigation());

        /// <summary>
        /// Called when it stops receiving input.
        /// </summary>
        public void OnStateExit()
        {
            if (holdNavigationRoutineReference != null) StopCoroutine(holdNavigationRoutineReference);
            holdNavigationRoutineReference = null;
        }

        /// <summary>
        /// Called when the user inputs navigation.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    lastNavInput = context.ReadValue<Vector2>();
                    Navigate(lastNavInput);
                    break;
                case { phase: InputActionPhase.Performed } when CanHoldToScrollFaster:
                    navHolding = true;
                    break;
                case { phase: InputActionPhase.Canceled } when CanHoldToScrollFaster:
                    navHolding = false;
                    break;
            }
        }

        /// <summary>
        /// Navigation when holding.
        /// </summary>
        private IEnumerator HoldNavigation()
        {
            while (true)
            {
                if (navHolding) Navigate(lastNavInput);

                yield return new WaitForSeconds(HoldNavigationInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Perform the navigation.
        /// Up and down change by one, left and right change by 10.
        /// </summary>
        /// <param name="input">Input to use when navigating.</param>
        private void Navigate(Vector2 input)
        {
            // ReSharper disable twice ConvertIfStatementToSwitchStatement
            if (input.y < 0)
            {
                if (CurrentAmount == 1)
                {
                    if (!navHolding) CurrentAmount = maxAmount;
                }
                else
                    CurrentAmount--;

                audioManager.PlayAudio(ChooseAudio);
                DownArrowJump();
            }
            else if (input.y > 0)
            {
                if (CurrentAmount == maxAmount)
                {
                    if (!navHolding) CurrentAmount = 1;
                }
                else
                    CurrentAmount++;

                audioManager.PlayAudio(ChooseAudio);
                UpArrowJump();
            }
            else if (input.x < 0)
            {
                int newAmount = (int)CurrentAmount - 10;

                CurrentAmount = (uint)Mathf.Clamp(newAmount, 1, maxAmount);

                audioManager.PlayAudio(ChooseAudio);
                DownArrowJump();
            }
            else if (input.x > 0)
            {
                CurrentAmount = (uint)Mathf.Clamp(CurrentAmount + 10, 1, maxAmount);

                audioManager.PlayAudio(ChooseAudio);
                UpArrowJump();
            }
        }

        /// <summary>
        /// Player chose to accept the amount.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            playerAccepted = true;
            playerChose = true;
        }

        /// <summary>
        /// Player chose to cancel.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            playerChose = true;
        }

        /// <summary>
        /// Make the up arrow jump.
        /// </summary>
        private void UpArrowJump() => UpArrow.DOLocalJump(originalUpArrowPosition, 2, 1, .25f);

        /// <summary>
        /// Make the down arrow jump.
        /// </summary>
        private void DownArrowJump() => DownArrow.DOLocalJump(originalDownArrowPosition, -2, 1, .25f);

        /// <summary>
        /// This controller has the UI input type.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name for input debug.
        /// </summary>
        public string GetDebugName() => name;

        #region Unused input callbacks

        public void OnMovement(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
        {
        }

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public void OnExtra1(InputAction.CallbackContext context)
        {
        }

        public void OnExtra2(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTextBackspace(InputAction.CallbackContext context)
        {
        }

        public void OnTextSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnTextCancel(InputAction.CallbackContext context)
        {
        }

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}