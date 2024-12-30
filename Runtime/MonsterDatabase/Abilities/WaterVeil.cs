using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability WaterVeil.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/WaterVeil", fileName = "WaterVeil")]
    public class WaterVeil : Ability
    {
        /// <summary>
        /// Status that will be cured after each action or on entering battle.
        /// </summary>
        [SerializeField]
        private Status StatusToCure;

        /// <summary>
        /// Trigger the ability when the monster enters battle.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return TriggerAbility(battler, battleManager);
        }

        /// <summary>
        /// Trigger the ability after each action.
        /// </summary>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            yield return TriggerAbility(owner, battleManager);
        }

        /// <summary>
        /// Trigger the ability when the monster leaves battle.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            yield return TriggerAbility(battler, battleManager);
        }

        /// <summary>
        /// Cure the status.
        /// </summary>
        private IEnumerator TriggerAbility(Battler battler, BattleManager battleManager)
        {
            if (battler.GetStatus() != StatusToCure) yield break;

            ShowAbilityNotification(battler);

            yield return battleManager.Statuses.RemoveStatus(battler);
        }
    }
}