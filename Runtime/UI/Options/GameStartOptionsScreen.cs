using System.Collections;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.GameFlow;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Controller for the options screen shown at the start of each game.
    /// </summary>
    public class GameStartOptionsScreen : OptionsScreen
    {
        /// <summary>
        /// Reference to the new game initializer.
        /// </summary>
        [Inject]
        private NewGameInitializer newGameInitializer;

        /// <summary>
        /// Show on enable.
        /// </summary>
        private void OnEnable() => ShowOptions();

        /// <summary>
        /// Debug name for input.
        /// </summary>
        public override string GetDebugName() => "GameStartOptionsScreen";

        /// <summary>
        /// Continue with starting the game.
        /// </summary>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            StartCoroutine(NextScreen());
        }

        /// <summary>
        /// Continue with starting the game.
        /// </summary>
        private IEnumerator NextScreen()
        {
            InputManager.ReleaseInput(this);

            AudioManager.PlayAudio(SelectAudio);

            yield return HideRoutine();

            newGameInitializer.OnGameOptionsFinished();
        }
    }
}