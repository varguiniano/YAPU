using System;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Input
{
    /// <summary>
    /// Manager in charge of handling and distributing input.
    /// It works as a state stack, other scripts can subscribe as an state to retrieve input values.
    /// Only the top state in the stack will receive input.
    /// </summary>
    public interface IInputManager : YAPUInputActions.IMainActions, YAPUInputActions.IUIActions, YAPUInputActions.ITextInputActions
    {
        /// <summary>
        /// Register an input receiver that is requesting input.
        /// </summary>
        /// <param name="receiver">The receiver to register.</param>
        void RequestInput(IInputReceiver receiver);

        /// <summary>
        /// Unregister a receiver that was requesting input.
        /// </summary>
        /// <param name="receiver">The receiver to unregister.</param>
        void ReleaseInput(IInputReceiver receiver);

        /// <summary>
        /// Block or unblock the input.
        /// </summary>
        /// <param name="block">Block or unblock?</param>
        void BlockInput(bool block = true);

        /// <summary>
        /// Subscribe to the input device changed event.
        /// </summary>
        /// <param name="callback">Event callback.</param>
        void SubscribeToInputDeviceChanged(Action<string> callback);
        
        /// <summary>
        /// Unsubscribe from the input device changed event.
        /// </summary>
        /// <param name="callback">Event callback.</param>
        void UnsubscribeToInputDeviceChanged(Action<string> callback);

        /// <summary>
        /// Get the input device currently being used.
        /// </summary>
        /// <returns>The name of that device.</returns>
        string GetCurrentInputDevice();

        /// <summary>
        /// Get the current movement vector given by the user.
        /// </summary>
        /// <returns>A vector 2.</returns>
        Vector2 GetCurrentMovementVector();
        
        /// <summary>
        /// Get the current navigation vector given by the user.
        /// </summary>
        /// <returns>A vector 2.</returns>
        Vector2 GetCurrentNavigationVector();

        /// <summary>
        /// Is the player holding the movement input?
        /// </summary>
        /// <returns>True if they are.</returns>
        bool IsHoldingPlayerMovement();
    }
}