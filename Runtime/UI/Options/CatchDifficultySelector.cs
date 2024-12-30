using Varguiniano.YAPU.Runtime.Player;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Selector for the catch difficulty.
    /// </summary>
    public class CatchDifficultySelector : OptionsMenuItem, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        private GlobalGameData globalGameData;

        /// <summary>
        /// Initialize.
        /// </summary>
        [Inject]
        public void Construct(GlobalGameData globalGameDataReference)
        {
            globalGameData = globalGameDataReference;

            SetOption((int) globalGameData.CatchDifficulty, true);
        }

        /// <summary>
        /// Save the difficulty.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup) =>
            globalGameData.CatchDifficulty = (CatchDifficulty) CurrentIndex;
    }
}