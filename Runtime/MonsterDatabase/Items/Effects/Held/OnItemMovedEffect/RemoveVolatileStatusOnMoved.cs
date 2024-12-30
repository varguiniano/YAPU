using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnItemMovedEffect
{
    /// <summary>
    /// Data class for an effect that removes a volatile status when the item is moved away.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnItemMoved/RemoveVolatileStatusOnMoved",
                     fileName = "RemoveVolatileStatusOnMoved")]
    public class RemoveVolatileStatusOnMoved : OnItemMovedItemEffect
    {
        /// <summary>
        /// Status to remove.
        /// </summary>
        [SerializeField]
        private VolatileStatus StatusToRemove;

        /// <summary>
        /// Called when the item effect is negated.
        /// </summary>
        /// <param name="item">Item negated.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnEffectNegated(Item item, Battler battler, BattleManager battleManager)
        {
            yield return RemoveStatus(battler, battleManager);

            yield return base.OnEffectNegated(item, battler, battleManager);
        }

        /// <summary>
        /// Called when the item gets stolen.
        /// </summary>
        /// <param name="item">Item stolen.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnItemStolen(Item item, Battler battler, BattleManager battleManager)
        {
            yield return RemoveStatus(battler, battleManager);

            yield return base.OnItemStolen(item, battler, battleManager);
        }

        /// <summary>
        /// Called when the holder received the item in battle.
        /// </summary>
        /// <param name="item">Item received.</param>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnItemConsumedInBattle(Item item, Battler battler, BattleManager battleManager)
        {
            yield return RemoveStatus(battler, battleManager);

            yield return base.OnItemConsumedInBattle(item, battler, battleManager);
        }

        /// <summary>
        /// Remove the status from the holder.
        /// </summary>
        /// <param name="battler">Holder.</param>
        /// <param name="battleManager">Reference tot he battle manager.</param>
        private IEnumerator RemoveStatus(Battler battler, BattleManager battleManager)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.Statuses.RemoveStatus(StatusToRemove, type, index);
        }
    }
}