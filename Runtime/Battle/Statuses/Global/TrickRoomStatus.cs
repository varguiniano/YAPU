using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Global
{
    /// <summary>
    /// Data class for the global status of TrickRoomStatus.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Global/TrickRoomStatus", fileName = "TrickRoomStatus")]
    public class TrickRoomStatus : GlobalStatus
    {
        /// <summary>
        /// Invert priority brackets.
        /// </summary>
        public override bool DoesInvertPriorityBracketOrder(BattleManager battleManager) => true;
    }
}