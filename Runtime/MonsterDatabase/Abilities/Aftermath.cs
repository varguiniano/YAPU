using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Aftermath.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Aftermath", fileName = "Aftermath")]
    public class Aftermath : Ability
    {
        /// <summary>
        /// Called when this battler is knocked out by another battler
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="userType">User of the effect.</param>
        /// <param name="userIndex">User of the effect.</param>
        /// <param name="userMove">Move used for the knock out, if any.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnKnockedOutByBattler(Battler owner,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Move userMove,
                                                          BattleManager battleManager)
        {
            if (userMove == null) yield break;

            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (!userMove.DoesMoveMakeContact(user,
                                              owner,
                                              battleManager,
                                              false))
                yield break;

            ShowAbilityNotification(owner);

            if (user.CanUseAbility(battleManager, IgnoresOtherAbilities(battleManager, owner, null))
             && !AffectsUserOfEffect(owner, user, IgnoresOtherAbilities(battleManager, owner, null), battleManager))
            {
                user.GetAbility().ShowAbilityNotification(user);
                yield break;
            }

            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                owner,
                                                                (short) (user.GetStats(battleManager)[Stat.Hp]
                                                                       * -.25f),
                                                                isSecondaryDamage: true);
        }
    }
}