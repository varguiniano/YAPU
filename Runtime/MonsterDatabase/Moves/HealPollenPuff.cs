using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the heal part of the move PollenPuff.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Bug/HealPollenPuff", fileName = "HealPollenPuff")]
    public class HealPollenPuff : HPPercentageRegenMove
    {
        // TODO: Animation.
        
        /// <summary>
        /// Always 50%.
        /// </summary>
        protected override float GetRegenPercentage(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex) => .5f;
    }
}