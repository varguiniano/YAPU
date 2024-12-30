using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Input
{
    /// <summary>
    /// Manager in charge of handling and distributing input.
    /// It works as a state stack, other scripts can subscribe as an state to retrieve input values.
    /// Only the top state in the stack will receive input.
    /// </summary>
    public class InputManager : Singleton<InputManager>, IInputManager
    {
        /// <summary>
        /// Stack of input blockers that are being used to block the input.
        /// </summary>
        private Stack<InputBlocker> inputBlockers;

        /// <summary>
        /// Init the input and subscribe to the callbacks.
        /// </summary>
        private void Awake()
        {
            inputBlockers = new Stack<InputBlocker>();

            inputActions = new YAPUInputActions();

            inputActions.Main.SetCallbacks(this);
            inputActions.UI.SetCallbacks(this);
            inputActions.TextInput.SetCallbacks(this);
        }

        /// <summary>
        /// Register an input receiver that is requesting input.
        /// </summary>
        /// <param name="receiver">The receiver to register.</param>
        public void RequestInput(IInputReceiver receiver)
        {
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnStateExit();

            stateStack.Push(receiver);

            receiver.OnStateEnter();

            SwitchInputType(receiver.GetInputType());

            Logger.Info("Registered " + receiver.GetDebugName() + " for input.");
            LogCurrentState();
        }

        /// <summary>
        /// Unregister a receiver that was requesting input.
        /// </summary>
        /// <param name="receiver">The receiver to unregister.</param>
        public void ReleaseInput(IInputReceiver receiver)
        {
            if (stateStack.Contains(receiver))
            {
                if (stateStack.Peek() == receiver)
                {
                    stateStack.Pop().OnStateExit();

                    if (stateStack.TryPeek(out IInputReceiver state)) state.OnStateEnter();
                }
                else
                {
                    // In an ideal world, this part will almost never be called, we need to rebuild the stack.

                    Logger.Warn("State stack needs to be rebuilt.");

                    Stack<IInputReceiver> tempStack = new();

                    while (stateStack.TryPop(out IInputReceiver oldReceiver))
                        if (oldReceiver != null && oldReceiver != receiver)
                            tempStack.Push(oldReceiver);

                    // The temp stack is inverted, so we re invert it when setting the new stack.
                    stateStack.Clear();

                    while (tempStack.TryPop(out IInputReceiver newReceiver)) stateStack.Push(newReceiver);
                }

                Logger.Info("Unregistered " + receiver.GetDebugName() + " from input.");
            }
            else
                Logger.Warn("The input receiver " + receiver.GetDebugName() + " was not found on the state stack!");

            if (stateStack.TryPeek(out IInputReceiver currentState)) SwitchInputType(currentState.GetInputType());

            LogCurrentState();
        }

        /// <summary>
        /// Block or unblock the input.
        /// </summary>
        /// <param name="block">Block or unblock?</param>
        public void BlockInput(bool block = true)
        {
            if (block)
            {
                InputBlocker blocker = new();
                RequestInput(blocker);
                inputBlockers.Push(blocker);
            }
            else
            {
                if (inputBlockers.Count == 0) return;
                InputBlocker blocker = inputBlockers.Pop();
                ReleaseInput(blocker);
            }
        }

        /// <summary>
        /// Subscribe to the input device changed event.
        /// </summary>
        /// <param name="callback">Event callback.</param>
        public void SubscribeToInputDeviceChanged(Action<string> callback) => onInputDeviceChanged += callback;

        /// <summary>
        /// Unsubscribe from the input device changed event.
        /// </summary>
        /// <param name="callback">Event callback.</param>
        public void UnsubscribeToInputDeviceChanged(Action<string> callback) => onInputDeviceChanged -= callback;

        /// <summary>
        /// Get the input device currently being used.
        /// </summary>
        /// <returns>The name of that device.</returns>
        public string GetCurrentInputDevice() => CurrentInputDevice;

        /// <summary>
        /// Is the player holding the movement input?
        /// </summary>
        /// <returns>True if they are.</returns>
        public bool IsHoldingPlayerMovement() => holdingMovement;

        /// <summary>
        /// State stack with all the states requesting input.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private Stack<IInputReceiver> stateStack = new();

        /// <summary>
        /// Input actions object that has the input events.
        /// </summary>
        private YAPUInputActions inputActions;

        /// <summary>
        /// Current input device being used.
        /// </summary>
        private string CurrentInputDevice
        {
            get => currentInputDevice;
            set
            {
                if (value == currentInputDevice) return;

                currentInputDevice = value;
                onInputDeviceChanged?.Invoke(CurrentInputDevice);
                Logger.Info("New current input device: " + CurrentInputDevice);
            }
        }

        /// <summary>
        /// Backfield for CurrentInputDevice.
        /// </summary>
        private string currentInputDevice;

        /// <summary>
        /// Event called whenever the input device changes.
        /// </summary>
        private Action<string> onInputDeviceChanged;

        /// <summary>
        /// Flag to know if the player is holding the movement input.
        /// This works independently of who is receiving the input,
        /// so it can be tracked accross scene changes.
        /// </summary>
        private bool holdingMovement;

        #region InputCallbacks

        /// <summary>
        /// Called when the Main Movement action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnMovement(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);

            holdingMovement = context.phase switch
            {
                InputActionPhase.Performed => true,
                InputActionPhase.Canceled => false,
                _ => holdingMovement
            };

            if (stateStack.TryPeek(out IInputReceiver state)) state.OnMovement(context);
        }

        /// <summary>
        /// Called when the Main Interact action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnInteract(context);
        }

        /// <summary>
        /// Called when the Main Menu action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnMenu(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);

            holdingMovement = false;
            
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnMenu(context);
        }

        /// <summary>
        /// Called when the run action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnRun(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnRun(context);
        }

        /// <summary>
        /// Called when the Extra action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnExtra(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnExtra(context);
        }

        /// <summary>
        /// Called when the UI Navigation action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            
            holdingMovement = context.phase switch
            {
                InputActionPhase.Performed => true,
                InputActionPhase.Canceled => false,
                _ => holdingMovement
            };
            
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnNavigation(context);
        }

        /// <summary>
        /// Called when the UI Select action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnSelect(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnSelect(context);
        }

        /// <summary>
        /// Called when the UI Back action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnBack(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnBack(context);
        }

        /// <summary>
        /// Called when the UI Extra1 action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnExtra1(context);
        }

        /// <summary>
        /// Called when the UI Extra2 action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnExtra2(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnExtra2(context);
        }

        /// <summary>
        /// Called when the UI Submit action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnSubmit(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnSubmit(context);
        }

        /// <summary>
        /// Called when the UI Cancel action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnCancel(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnCancel(context);
        }

        /// <summary>
        /// Called when the UI Point action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnPoint(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnPoint(context);
        }

        /// <summary>
        /// Called when the UI ScrollWheel action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnScrollWheel(context);
        }

        /// <summary>
        /// Called when the text input backspace action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnTextBackspace(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnTextBackspace(context);
        }

        /// <summary>
        /// Called when the text input submit action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnTextSubmit(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnTextSubmit(context);
        }

        /// <summary>
        /// Called when the text input cancel action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnTextCancel(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnTextCancel(context);
        }

        /// <summary>
        /// Called when the text input any key action is inputted.
        /// </summary>
        /// <param name="context">Action context.</param>
        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
            UpdateLastDevice(context);
            if (stateStack.TryPeek(out IInputReceiver state)) state.OnAnyTextKey(context);
        }

        #endregion

        /// <summary>
        /// Get the current movement vector given by the user.
        /// </summary>
        /// <returns>A vector 2.</returns>
        public Vector2 GetCurrentMovementVector() => inputActions.Main.Movement.ReadValue<Vector2>();

        /// <summary>
        /// Get the current navigation vector given by the user.
        /// </summary>
        /// <returns>A vector 2.</returns>
        public Vector2 GetCurrentNavigationVector() => inputActions.UI.Navigation.ReadValue<Vector2>();

        /// <summary>
        /// Update the last device used.
        /// </summary>
        /// <param name="context">Context of the last action used.</param>
        private void UpdateLastDevice(InputAction.CallbackContext context)
        {
            InputDevice lastDevice = context.control?.device;

            if (lastDevice != null)
                CurrentInputDevice = lastDevice.displayName;
            else
                Logger.Warn("Registered an action from a null input device.");
        }

        /// <summary>
        /// Log the current input receiver.
        /// </summary>
        private void LogCurrentState()
        {
            try
            {
                Logger.Info(stateStack.TryPeek(out IInputReceiver receiver) && receiver != null
                                ? "Current input receiver: " + receiver.GetDebugName() + "."
                                : "No current input receivers.");
            }
            catch
            {
                Logger.Warn("The top receiver of the stack is null.");
            }
        }

        /// <summary>
        /// Switch to the given input type.
        /// </summary>
        /// <param name="inputType">Input type to switch to.</param>
        private void SwitchInputType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Main:
                    inputActions.UI.Disable();
                    inputActions.Main.Enable();
                    inputActions.TextInput.Disable();
                    break;
                case InputType.UI:
                    inputActions.Main.Disable();
                    inputActions.UI.Enable();
                    inputActions.TextInput.Disable();
                    break;
                case InputType.TextInput:
                    inputActions.Main.Disable();
                    inputActions.UI.Disable();
                    inputActions.TextInput.Enable();
                    break;
                default:
                    Logger.Error("Input type " + inputType + " not supported.");
                    break;
            }
        }
    }
}