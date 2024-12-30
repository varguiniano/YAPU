using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle
{
    /// <summary>
    /// Item effect that allows to run from wilds.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/InBattle/RunFromWildInBattle",
                     fileName = "RunFromWildInBattle")]
    public class RunFromWildInBattle : UseInBattleItemEffect
    {
        /// <summary>
        /// It can be used as long as the player can run.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the item.</param>
        /// <returns>True if it can be used.</returns>
        public override bool CanBeUsed(BattleManager battleManager, Battler user) => battleManager.CanPlayerRun;

        /// <summary>
        /// Use in battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        BattlerType userType,
                                        int userIndex,
                                        BattleManager battleManager,
                                        Action<bool> finished)
        {
            yield return battleManager.Battlers.RunAway(BattlerType.Ally, userIndex, true, true);

            finished.Invoke(true);
        }
    }
}