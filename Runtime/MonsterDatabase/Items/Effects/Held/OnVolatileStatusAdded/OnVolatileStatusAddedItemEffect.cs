using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnVolatileStatusAdded
{
    /// <summary>
    /// Base data class for held item effects that are called when the holder is given a volatile status.
    /// </summary>
    public abstract class OnVolatileStatusAddedItemEffect : MonsterDatabaseScriptable<OnVolatileStatusAddedItemEffect>
    {
        /// <summary>
        /// Called when a volatile status is added to the holder.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="item">Item that has this effect.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="userType">Type of the one that added the status.</param>
        /// <param name="userIndex">Index of the one that added the status.</param>
        /// <param name="status">Status added.</param>
        /// <param name="countdown">Countdown established.</param>
        /// <param name="finished">Callback establishing if the item should be consumed.</param>
        public abstract IEnumerator OnVolatileStatusAdded(BattleManager battleManager,
                                                          Item item,
                                                          Battler holder,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          VolatileStatus status,
                                                          int countdown,
                                                          Action<bool> finished);
    }
}