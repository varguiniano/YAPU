using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move FoulPlay.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/FoulPlay", fileName = "FoulPlay")]
    public class FoulPlay : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the value of the attack stat to use for damage calculation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="attackStat">Stat to use.</param>
        /// <param name="ignoresAbilities"></param>
        /// <returns>The value of that stat.</returns>
        protected override float GetAttackValueForDamageCalculation(BattleManager battleManager,
                                                                    Battler user,
                                                                    Battler target,
                                                                    bool isCritical,
                                                                    Stat attackStat,
                                                                    bool ignoresAbilities)
        {
            float attackStage =
                target.CanUseAbility(battleManager, ignoresAbilities)
             && target.GetAbility().IgnoreAttackStageWhenWhenTargeted()
                    ? 1
                    : MonsterMathHelper.GetStageMultiplier(target, attackStat, isCritical);

            return MonsterMathHelper.CalculateStat(target, attackStat, battleManager) * attackStage;
        }
    }
}