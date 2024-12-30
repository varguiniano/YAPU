using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Block move status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Blocked", fileName = "Blocked")]
    public class Blocked : VolatileStatus
    {
        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanSwitch(Battler battler,
                                       BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       Move userMove,
                                       Item item,
                                       bool itemBelongsToUser,
                                       bool showMessages)
        {
            (BattlerType ownType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            // Can switch if the effect comes from a move.
            if (userMove != null)
                return base.CanSwitch(battler,
                                      battleManager,
                                      userType,
                                      userIndex,
                                      userMove,
                                      item,
                                      itemBelongsToUser,
                                      showMessages);

            if (showMessages)
                DialogManager.ShowDialog("Status/Volatile/Blocked/CantSwitch",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }

        /// <summary>
        /// Can never run away.
        /// </summary>
        public override bool CanRunAway(Battler battler, BattleManager battleManager, bool showMessages) => false;
    }
}