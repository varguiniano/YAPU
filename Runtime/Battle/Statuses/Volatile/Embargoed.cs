using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Embargoed status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Embargoed", fileName = "Embargoed")]
    public class Embargoed : VolatileStatus
    {
        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Check if the battler is allowed to use their held item.
        /// </summary>
        /// <param name="battler">Battler owner of the status.</param>
        public override bool CanUseHeldItem(Battler battler) => false;

        /// <summary>
        /// Check if the monster can use bag items.
        /// Only be able to use balls.
        /// </summary>
        /// <returns>True if it can.</returns>
        public override bool CanUseBagItem(Battler battler, Item item, BattleManager battleManager) => item is Ball;
    }
}