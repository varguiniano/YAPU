using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Sturdy.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Sturdy", fileName = "Sturdy")]
    public class Sturdy : Ability
    {
        /// <summary>
        /// Check if this ability should trigger force survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">Amount the HP is going to change. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        /// <returns>True if force survive should be triggered.</returns>
        public override bool ShouldForceSurvive(Battler owner,
                                                int amount,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                bool isSecondaryDamage,
                                                Move userMove = null)
        {
            uint maxHP = owner.GetStats(battleManager)[Stat.Hp];

            return amount < 0
                && owner.CurrentHP == maxHP
                && -amount >= maxHP
                && battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex) != owner;
        }

        /// <summary>
        /// Called after the monster has survived, if it was this ability the one that made it survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">The actual amount of Hp that was changed. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        public override IEnumerator OnForceSurvive(Battler owner,
                                                   int amount,
                                                   BattleManager battleManager,
                                                   BattlerType userType,
                                                   int userIndex,
                                                   bool isSecondaryDamage,
                                                   Move userMove = null)
        {
            ShowAbilityNotification(owner);

            yield break;
        }
    }
}