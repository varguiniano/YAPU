using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for damage moves that change stages of the user when the target faints.
    /// </summary>
    public abstract class StageChangeWhenTargetFaintsDamageMove : StageChanceDamageMove
    {
        /// <summary>
        /// Only apply if the target fainted.
        /// </summary>
        protected override IEnumerator ApplyStageChanges(BattleManager battleManager,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         BattlerType targetType,
                                                         int targetIndex,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         bool ignoresAbilities)
        {
            if (battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).CanBattle) yield break;

            yield return base.ApplyStageChanges(battleManager, userType, userIndex, targetType, targetIndex, targets, ignoresAbilities);
        }
    }
}