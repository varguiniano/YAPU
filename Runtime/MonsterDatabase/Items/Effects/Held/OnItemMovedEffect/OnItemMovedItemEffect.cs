using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnItemMovedEffect
{
    /// <summary>
    /// Data class to add effects to items when the item is negated, stolen, consumed, etc.
    /// </summary>
    public abstract class OnItemMovedItemEffect : MonsterDatabaseScriptable<OnItemMovedItemEffect>
    {
        /// <summary>
        /// Called when the item effect is negated.
        /// </summary>
        /// <param name="item">Item negated.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnEffectNegated(Item item, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the item effect is negated.
        /// </summary>
        /// <param name="item">Item negated.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnEffectReEnabled(Item item, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the item gets stolen.
        /// </summary>
        /// <param name="item">Item stolen.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemStolen(Item item, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the holder received the item in battle.
        /// </summary>
        /// <param name="item">Item received.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemReceivedInBattle(Item item, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the item is consumed in battle.
        /// </summary>
        /// <param name="item">Item consumed.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemConsumedInBattle(Item item, Battler battler, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Called when the item is consumed.
        /// </summary>
        /// <param name="item">Item consumed.</param>
        public virtual IEnumerator OnItemConsumed(Item item)
        {
            yield break;
        }
    }
}