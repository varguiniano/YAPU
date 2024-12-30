namespace Varguiniano.YAPU.Runtime.Input
{
    /// <summary>
    /// Interface that defines a class that can receive input from the input manager.
    /// </summary>
    public interface IInputReceiver : YAPUInputActions.IMainActions, YAPUInputActions.IUIActions, YAPUInputActions.ITextInputActions
    {
        /// <summary>
        /// Retrieve the input type of this receiver.
        /// </summary>
        /// <returns>The input type of this receiver.</returns>
        InputType GetInputType();

        /// <summary>
        /// Called when the state enters the top of the stack.
        /// </summary>
        void OnStateEnter();

        /// <summary>
        /// Called when the state exits the top of the stack.
        /// </summary>
        void OnStateExit();

        /// <summary>
        /// Name of this receiver for debugging purposes.
        /// </summary>
        string GetDebugName();
    }
}