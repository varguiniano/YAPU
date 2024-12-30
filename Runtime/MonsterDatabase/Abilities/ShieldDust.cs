using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Shield Dust, that protects from damage move's secondary effects.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ShieldDust", fileName = "ShieldDust")]
    public class ShieldDust : Ability
    {
        /// <summary>
        /// It's never affected.
        /// </summary>
        public override bool IsAffectedBySecondaryEffectsOfDamageMove(Battler owner,
                                                                      Battler attacker,
                                                                      DamageMove move,
                                                                      int damageDealt,
                                                                      BattleManager battleManager) =>
            !AffectsUserOfEffect(attacker, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager);
    }
}