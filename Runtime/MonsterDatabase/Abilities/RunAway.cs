using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Run Away, that allows running away in any situation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/RunAway", fileName = "RunAway")]
    public class RunAway : Ability
    {
        /// <summary>
        /// Force being able to run away.
        /// </summary>
        public override (bool, bool) CanRunAway(Battler battler, BattleManager battleManager, bool showMessages) =>
            (true, true);

        /// <summary>
        /// Called when the battler runs away.
        /// </summary>
        public override IEnumerator OnRunAway(Battler owner, BattleManager battleManager)
        {
            ShowAbilityNotification(owner);
            yield break;
        }
    }
}