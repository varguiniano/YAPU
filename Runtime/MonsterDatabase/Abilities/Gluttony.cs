using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Gluttony.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Gluttony", fileName = "Gluttony")]
    public class Gluttony : Ability
    {
        /// <summary>
        /// Eat the 25% berries at 50%.
        /// </summary>
        /// <param name="berry">Berry to eat.</param>
        /// <param name="threshold">Current threshold.</param>
        /// <param name="battler">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The new threshold.</returns>
        public override float ModifyBerryHPThreshold(Berry berry,
                                                     float threshold,
                                                     Battler battler,
                                                     BattleManager battleManager)
        {
            if (!(Math.Abs(threshold - .25f) < .01f)) return threshold;

            ShowAbilityNotification(battler);
            return .5f;
        }
    }
}