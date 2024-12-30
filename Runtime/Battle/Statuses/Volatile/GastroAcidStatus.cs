using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move GastroAcid.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/GastroAcid", fileName = "GastroAcidStatus")]
    public class GastroAcidStatus : VolatileStatus
    {
        /// <summary>
        /// Prevent from using the ability.
        /// </summary>
        public override bool CanUseAbility(Battler owner, BattleManager battleManager) => false;
    }
}