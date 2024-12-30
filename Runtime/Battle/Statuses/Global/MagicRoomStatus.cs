using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Global
{
    /// <summary>
    /// Data class for the global status of Magic Room.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Global/MagicRoom", fileName = "MagicRoomStatus")]
    public class MagicRoomStatus : GlobalStatus
    {
        /// <summary>
        /// Check if the battler is allowed to use their held item.
        /// </summary>
        /// <param name="battler">Battler owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override bool CanUseHeldItem(Battler battler, BattleManager battleManager) => false;
    }
}