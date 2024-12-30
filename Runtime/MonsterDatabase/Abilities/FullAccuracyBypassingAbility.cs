using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that completely bypass all accuracy checks.
    /// </summary>
    public abstract class FullAccuracyBypassingAbility : Ability
    {
        /// <summary>
        /// Bypass all checks.
        /// </summary>
        public override bool DoesBypassAllAccuracyChecksWhenUsing(Battler owner,
                                                                  Move move,
                                                                  Battler target,
                                                                  BattleManager battleManager)
        {
            ShowAbilityNotification(owner);
            return true;
        }

        /// <summary>
        /// Bypass all checks.
        /// </summary>
        public override bool DoesBypassAllAccuracyChecksWhenTargeted(Battler owner,
                                                                     Move move,
                                                                     Battler user,
                                                                     BattleManager battleManager)
        {
            ShowAbilityNotification(owner);
            return true;
        }
    }
}