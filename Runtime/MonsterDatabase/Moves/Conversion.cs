using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Conversion.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Conversion", fileName = "Conversion")]
    public class Conversion : SetVolatileStatusMove
    {
        /// <summary>
        /// Get the types of the first move.
        /// </summary>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            return new object[] {target.CurrentMoves[0].Move.GetMoveTypeInBattle(target, battleManager), null};
        }
    }
}