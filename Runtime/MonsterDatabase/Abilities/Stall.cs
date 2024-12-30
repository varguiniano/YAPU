using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Stall.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Stall", fileName = "Stall")]
    public class Stall : Ability
    {
        /// <summary>
        /// Called to determine the priority of the battler inside its priority bracket.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Finished callback stating if it should go first (1), last (-1) or normal (0).</param>
        public override IEnumerator OnDeterminePriority(Battler battler,
                                                        BattleManager battleManager,
                                                        Action<int> finished)
        {
            ShowAbilityNotification(battler);
            finished.Invoke(-1);
            yield break;
        }
    }
}