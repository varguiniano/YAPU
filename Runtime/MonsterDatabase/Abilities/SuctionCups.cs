using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability SuctionCups.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SuctionCups", fileName = "SuctionCups")]
    public class SuctionCups : Ability
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
            bool canSwitch = userType == battleManager.Battlers.GetTypeAndIndexOfBattler(battler).Type;

            if (!canSwitch) ShowAbilityNotification(battler);

            return canSwitch;
        }
    }
}