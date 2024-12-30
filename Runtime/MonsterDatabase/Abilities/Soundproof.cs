using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Soundproof.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Soundproof", fileName = "Soundproof")]
    public class Soundproof : Ability
    {
        /// <summary>
        /// Immune to sound moves.
        /// </summary>
        public override bool IsImmuneToMove(Battler owner,
                                            Move move,
                                            BattlerType userType,
                                            int userIndex,
                                            BattleManager battleManager)
        {
            if (!move.SoundBased) return false;
            ShowAbilityNotification(owner);
            return true;
        }
    }
}