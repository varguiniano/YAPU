using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability SkillLink.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SkillLink", fileName = "SkillLink")]
    public class SkillLink : Ability
    {
        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>Tuple stating if the number has been modified and the new value.</returns>
        public override (bool, int) GetNumberOfHitsOfMultihitMove(BattleManager battleManager,
                                                                  Battler owner,
                                                                  Move move,
                                                                  List<(BattlerType Type, int Index)> targets)
        {
            if (!move.IsMultiHit) return (false, 1);

            ShowAbilityNotification(owner);

            return (true, move.HitChances.Max(pair => pair.Value));
        }
    }
}