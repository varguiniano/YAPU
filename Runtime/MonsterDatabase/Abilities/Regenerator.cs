using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Regenerator.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Regenerator", fileName = "Regenerator")]
    public class Regenerator : Ability
    {
        /// <summary>
        /// Percentage of health to regenerate.
        /// </summary>
        [SerializeField]
        private float RegenPercentage = 1 / 3f;

        /// <summary>
        /// Regen HP when the monster leaves the battle.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            if (!battler.CanBattle) yield break;

            int hpToRegen = (int) (battler.GetStats(battleManager)[Stat.Hp] * RegenPercentage);

            ShowAbilityNotification(battler);

            yield return battleManager.BattlerHealth.ChangeLife(battler, battler, hpToRegen);
        }
    }
}