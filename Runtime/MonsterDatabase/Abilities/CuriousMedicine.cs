using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability CuriousMedicine.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/CuriousMedicine", fileName = "CuriousMedicine")]
    public class CuriousMedicine : Ability
    {
        /// <summary>
        /// Reset all stats.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            ShowAbilityNotification(battler);

            foreach (Battler target in battleManager.Battlers.GetBattlersFighting(battleManager.Battlers
                        .GetTypeAndIndexOfBattler(battler)
                        .Type))
                target.ResetStages();
        }
    }
}