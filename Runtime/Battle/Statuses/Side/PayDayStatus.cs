using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Class representing the PayDay battle side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/PayDay", fileName = "PayDayStatus")]
    public class PayDayStatus : LayeredSideStatus
    {
        /// <summary>
        /// Called to provide the player with additional price money at the end of the battle.
        /// </summary>
        public override uint GetPriceMoney() =>
            !LayerCount.ContainsKey(BattlerType.Ally) ? 0 : LayerCount[BattlerType.Ally];
    }
}