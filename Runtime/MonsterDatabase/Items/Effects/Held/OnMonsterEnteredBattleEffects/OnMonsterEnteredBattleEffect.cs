using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnMonsterEnteredBattleEffects
{
    /// <summary>
    /// Base class for item effects that trigger when the monster enters battle.
    /// </summary>
    public abstract class OnMonsterEnteredBattleEffect : MonsterDatabaseScriptable<OnMonsterEnteredBattleEffect>
    {
        /// <summary>
        /// Called after the holder enters battle.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public abstract IEnumerator OnMonsterEnteredBattle(Item item,
                                                           Battler battler,
                                                           BattleManager battleManager,
                                                           Action<bool> finished);
    }
}