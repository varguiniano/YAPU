using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle
{
    /// <summary>
    /// Data class representing an effect of an item that can be used only in battle.
    /// </summary>
    public abstract class UseInBattleItemEffect : MonsterDatabaseScriptable<UseInBattleItemEffect>
    {
        /// <summary>
        /// Check if the item can be used in this moment.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user">User of the item.</param>
        /// <returns></returns>
        public virtual bool CanBeUsed(BattleManager battleManager, Battler user) => true;

        /// <summary>
        /// Use in battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public abstract IEnumerator Use(Item item,
                                        BattlerType userType,
                                        int userIndex,
                                        BattleManager battleManager,
                                        Action<bool> finished);
    }
}