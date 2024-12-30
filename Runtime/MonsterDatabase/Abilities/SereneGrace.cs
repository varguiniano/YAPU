using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability SereneGrace.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SereneGrace", fileName = "SereneGrace")]
    public class SereneGrace : Ability
    {
        /// <summary>
        /// Chance of performing secondary effects of moves.
        /// </summary>
        [SerializeField]
        private float SecondaryChanceMultiplier = 2;

        /// <summary>
        /// Multiply the chance.
        /// </summary>
        public override float GetMultiplierForChanceOfSecondaryEffectOfMove(Battler owner,
                                                                            List<(BattlerType Type, int Index)> targets,
                                                                            Move move,
                                                                            BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            return SecondaryChanceMultiplier;
        }
    }
}