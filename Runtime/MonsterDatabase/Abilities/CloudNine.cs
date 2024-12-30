using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// CloudNine ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/CloudNine", fileName = "CloudNine")]
    public class CloudNine : Ability
    {
        /// <summary>
        /// Does the weather have effect?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override bool DoesWeatherHaveEffect(Battler owner, BattleManager battleManager) => false;
    }
}