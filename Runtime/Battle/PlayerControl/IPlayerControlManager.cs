using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle.AI;

namespace Varguiniano.YAPU.Runtime.Battle.PlayerControl
{
    /// <summary>
    /// Interface that defines how a player control manager should work.
    /// </summary>
    public interface IPlayerControlManager
    {
        /// <summary>
        /// Request the player to choose their next action.
        /// </summary>
        /// <param name="battleManagerReference">Reference to the battle manager.</param>
        /// <param name="index">Index of the monster that is requesting the action.</param>
        /// <param name="finished">Event with the chosen battle action.</param>
        /// <param name="allowGoBack">Allow to go back to the previous monster. Useful for double battles.</param>
        /// <param name="goBackCallback">Callback when going back.</param>
        IEnumerator RequestAction(BattleManager battleManagerReference,
                                  int index,
                                  Action<BattleAction> finished,
                                  bool allowGoBack,
                                  Action goBackCallback);

        /// <summary>
        /// Request the player to choose a new monster to send.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback when finished.</param>
        /// <param name="menuCanBeClosed">Whether the change is obligatory, useful for changing after fainting.</param>
        void RequestNewMonster(BattleManager battleManager, Action<int> finished, bool menuCanBeClosed = false);

        /// <summary>
        /// Release the input of the menus.
        /// Maybe not all implementations will need to do something on this method.
        /// </summary>
        void ReleaseInput();
    }
}