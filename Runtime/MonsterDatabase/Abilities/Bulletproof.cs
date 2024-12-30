using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Bulletproof.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Bulletproof", fileName = "Bulletproof")]
    public class Bulletproof : Ability
    {
        /// <summary>
        /// Immune to ball moves.
        /// </summary>
        public override bool IsImmuneToMove(Battler owner,
                                            Move move,
                                            BattlerType userType,
                                            int userIndex,
                                            BattleManager battleManager)
        {
            if (!move.IsBallMove) return false;
            ShowAbilityNotification(owner);
            return true;
        }
    }
}