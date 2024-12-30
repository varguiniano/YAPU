using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the HyperCutter ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/HyperCutter", fileName = "HyperCutter")]
    public class HyperCutter : Ability
    {
        /// <summary>
        /// Stat which stages can't be lowered.
        /// </summary>
        [SerializeField]
        private Stat ProtectedStat;

        /// <summary>
        /// Trigger the effect if it was chanced by a triggering ability.
        /// </summary>
        public override IEnumerator OnStatChange(Battler owner,
                                                 Stat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Ability changingAbility,
                                                 Action<short> callback)
        {
            if (stat != ProtectedStat)
            {
                callback.Invoke(modifier);
                yield break;
            }

            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, IgnoresOtherAbilities(battleManager, owner, null))
             && !user.GetAbility()
                     .AffectsUserOfEffect(owner,
                                          user,
                                          IgnoresOtherAbilities(battleManager, owner, null),
                                          battleManager))
            {
                user.GetAbility().ShowAbilityNotification(user);

                callback.Invoke(modifier);
                yield break;
            }

            ShowAbilityNotification(owner);

            callback.Invoke(0);
        }
    }
}