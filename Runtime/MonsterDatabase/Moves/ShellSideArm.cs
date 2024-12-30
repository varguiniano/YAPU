using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move ShellSideArm.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/ShellSideArm", fileName = "ShellSideArm")]
    public class ShellSideArm : StatusChanceDamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Physical or special depending on which is more effective.
        /// </summary>
        public override Category GetMoveCategory(Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 BattleManager battleManager)
        {
            if (user == null || target == null || battleManager == null)
                return base.GetMoveCategory(user, target, ignoresAbilities, battleManager);

            float physicalRatio =
                GetAttackDefenseDamageMultiplier(battleManager,
                                                 user,
                                                 target,
                                                 false,
                                                 ignoresAbilities,
                                                 Stat.Attack,
                                                 Stat.Defense);

            float specialRatio =
                GetAttackDefenseDamageMultiplier(battleManager,
                                                 user,
                                                 target,
                                                 false,
                                                 ignoresAbilities,
                                                 Stat.SpecialAttack,
                                                 Stat.SpecialDefense);

            return physicalRatio > specialRatio ? Category.Physical : Category.Special;
        }

        /// <summary>
        /// Contact only if physical.
        /// </summary>
        public override bool DoesMoveMakeContact(Battler user,
                                                 Battler target,
                                                 BattleManager battleManager,
                                                 bool ignoresAbilities) =>
            GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical;
    }
}