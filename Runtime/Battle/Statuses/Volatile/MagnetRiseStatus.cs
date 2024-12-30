using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the MagnetRise status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/MagnetRiseStatus", fileName = "MagnetRiseStatus")]
    public class MagnetRiseStatus : VolatileStatus
    {
        /// <summary>
        /// Never grounded.
        /// </summary>
        public override (bool, bool) IsGrounded(Battler battler) => (false, true);
    }
}