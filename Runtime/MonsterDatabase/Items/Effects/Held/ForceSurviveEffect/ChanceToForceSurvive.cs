using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.ForceSurviveEffect
{
    /// <summary>
    /// Data class for an item effect that has an unconditional chance to make the battler force survive.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/ForceSurvive/Chance", fileName = "ChanceToForceSurvive")]
    public class ChanceToForceSurvive : ForceSurviveItemEffect
    {
        /// <summary>
        /// Chance to force survive.
        /// </summary>
        [SerializeField]
        private float Chance;

        /// <summary>
        /// Consume the item after surviving?
        /// </summary>
        [SerializeField]
        private bool ConsumeItem;

        /// <summary>
        /// Check if this item should trigger force survive.
        /// </summary>
        /// <param name="item">Item having this effect.</param>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">Amount the HP is going to change. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        /// <returns>True if force survive should be triggered.</returns>
        public override bool ShouldForceSurvive(Item item,
                                                Battler owner,
                                                int amount,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                bool isSecondaryDamage,
                                                Move userMove = null) =>
            battleManager.RandomProvider.Value01() <= Chance
         && battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex) != owner;

        /// <summary>
        /// Called after the monster has survived, if it was this item the one that made it survive.
        /// </summary>
        /// <param name="item">Item having this effect.</param>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">The actual amount of Hp that was changed. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="finished">Callback stating true if the item should be consumed.</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        public override IEnumerator OnForceSurvive(Item item,
                                                   Battler owner,
                                                   int amount,
                                                   BattleManager battleManager,
                                                   BattlerType userType,
                                                   int userIndex,
                                                   bool isSecondaryDamage,
                                                   Action<bool> finished,
                                                   Move userMove = null)
        {
            item.ShowItemNotification(owner, battleManager.Localizer);

            finished.Invoke(ConsumeItem);

            yield break;
        }
    }
}