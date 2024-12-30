using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the MeanLooked status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/MeanLooked", fileName = "MeanLooked")]
    public class MeanLooked : VolatileStatus
    {
        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanRunAway(Battler battler, BattleManager battleManager, bool showMessages)
        {
            if (showMessages)
                DialogManager.ShowDialog("Status/Volatile/MeanLooked/CantRun",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }
    }
}