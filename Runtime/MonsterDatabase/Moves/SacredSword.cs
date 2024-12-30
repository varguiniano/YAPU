using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move SacredSword.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/SacredSword", fileName = "SacredSword")]
    public class SacredSword : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Ignore defense stage.
        /// </summary>
        protected override bool DoesIgnoreDefenseStageWhenUsingMove(BattleManager battleManager,
                                                                    Battler user,
                                                                    Battler target,
                                                                    bool isCritical,
                                                                    Stat attackStat,
                                                                    bool ignoresAbilities) =>
            true;

        /// <summary>
        /// Ignore evasion.
        /// </summary>
        protected override bool DoesIgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing(Battler user,
            Battler target,
            bool ignoresAbilities,
            BattleManager battleManager) =>
            true;
    }
}