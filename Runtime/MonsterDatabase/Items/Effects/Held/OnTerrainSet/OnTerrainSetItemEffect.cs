using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnTerrainSet
{
    /// <summary>
    /// Base class for item effects that are called when the terrain is set.
    /// </summary>
    public abstract class OnTerrainSetItemEffect : MonsterDatabaseScriptable<OnTerrainSetItemEffect>
    {
        /// <summary>
        /// Called when a terrain is set on the battlefield.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="terrain">Terrain that has been set.</param>
        /// <param name="holder">Holder of this item.</param>
        /// <param name="item">Item containing this effect.</param>
        /// <param name="finished">Should the item be consumed?</param>
        public abstract IEnumerator OnTerrainSet(BattleManager battleManager,
                                                 Terrain terrain,
                                                 Battler holder,
                                                 Item item,
                                                 Action<bool> finished);
    }
}