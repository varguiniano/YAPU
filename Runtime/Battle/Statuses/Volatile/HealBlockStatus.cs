using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the HealBlock status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/HealBlockStatus", fileName = "HealBlockStatus")]
    public class HealBlockStatus : VolatileStatus
    {
        /// <summary>
        /// Prevent healing.
        /// </summary>
        public override bool CanHeal(Battler owner, BattleManager battleManager) => false;
    }
}