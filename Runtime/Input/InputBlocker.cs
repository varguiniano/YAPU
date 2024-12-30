using UnityEngine.InputSystem;

namespace Varguiniano.YAPU.Runtime.Input
{
    /// <summary>
    /// Empty class with no logic that can be added to the input state stack to block the input from any other state.
    /// </summary>
    public class InputBlocker : IInputReceiver
    {
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

        public void OnNavigation(InputAction.CallbackContext context)
        {
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
        }

        public void OnBack(InputAction.CallbackContext context)
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

        public InputType GetInputType() => InputType.Main;

        public void OnStateEnter()
        {
        }

        public void OnStateExit()
        {
        }

        public string GetDebugName() => "Blocker";
    }
}