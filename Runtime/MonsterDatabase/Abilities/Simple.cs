using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Simple.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Simple", fileName = "Simple")]
    public class Simple : Ability
    {
        /// <summary>
        /// Duplicate all stat changes.
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
            ShowAbilityNotification(owner);

            callback.Invoke((short) (modifier * 2));

            yield break;
        }

        /// <summary>
        /// Duplicate all stat changes.
        /// </summary>
        public override IEnumerator OnStatChange(Battler owner,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Action<short> callback)
        {
            ShowAbilityNotification(owner);

            callback.Invoke((short) (modifier * 2));

            yield break;
        }
    }
}