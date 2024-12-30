using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability NaturalCure.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/NaturalCure", fileName = "NaturalCure")]
    public class NaturalCure : Ability
    {
        /// <summary>
        /// Trigger the healing effect.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            yield return TriggerAbility(battleManager, battler);
        }

        /// <summary>
        /// Trigger the healing effect.
        /// </summary>
        public override IEnumerator OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            yield return TriggerAbility(battleManager, battler);
        }

        /// <summary>
        /// Trigger the healing effect.
        /// </summary>
        private IEnumerator TriggerAbility(BattleManager battleManager, Battler battler)
        {
            if (battler.GetStatus() == null) yield break;

            ShowAbilityNotification(battler);

            yield return battleManager.Statuses.RemoveStatus(battler);
        }
    }
}