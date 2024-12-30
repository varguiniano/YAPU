using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move ReflectType.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/ReflectType", fileName = "ReflectType")]
    public class ReflectType : SetVolatileStatusMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the types of the target.
        /// </summary>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex)
        {
            (MonsterType firstType, MonsterType secondType) =
                battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex)
                             .GetTypes(battleManager.YAPUSettings);

            return new object[] {firstType, secondType};
        }
    }
}