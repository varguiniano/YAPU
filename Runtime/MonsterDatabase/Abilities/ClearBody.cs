using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// ClearBody ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ClearBody", fileName = "ClearBody")]
    public class ClearBody : Ability
    {
        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(Battler owner,
                                                 Stat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Ability changingAbility,
                                                 Action<short> callback)
        {
            if (modifier >= 0 || battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex) == owner)
                callback.Invoke(modifier);
            else
            {
                ShowAbilityNotification(owner);

                callback.Invoke(0);
            }

            yield break;
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(Battler owner,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Action<short> callback)
        {
            if (modifier >= 0 || battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex) == owner)
                callback.Invoke(modifier);
            else
            {
                ShowAbilityNotification(owner);

                callback.Invoke(0);
            }

            yield break;
        }
    }
}