using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Contrary.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Contrary", fileName = "Contrary")]
    public class Contrary : Ability
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
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(Battler owner,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Action<short> callback)
        {
            ShowAbilityNotification(owner);

            callback.Invoke((short)(modifier * -1));

            yield break;
        }
    }
}