using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for PayDay.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/PayDay", fileName = "PayDay")]
    public class PayDay : DamageMoveAndSetLayeredSideStatus
    {
        /// <summary>
        /// Get the number of layers to set.
        /// </summary>
        protected override uint GetNumberOfLayers(Battler user, BattleManager battleManager) =>
            user.StatData.Level * base.GetNumberOfLayers(user, battleManager);
    }
}