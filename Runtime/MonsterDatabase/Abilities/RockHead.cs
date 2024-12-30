using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// RockHead ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/RockHead", fileName = "RockHead")]
    public class RockHead : Ability
    {
        /// <summary>
        /// Not affected by recoil.
        /// </summary>
        public override bool IsAffectedByRecoil(Battler battler, BattleManager battleManager)
        {
            ShowAbilityNotification(battler);
            return false;
        }
    }
}