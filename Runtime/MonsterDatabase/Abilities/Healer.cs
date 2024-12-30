using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Healer.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Healer", fileName = "Healer")]
    public class Healer : Ability
    {
        /// <summary>
        /// Chances to heal the status condition.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HealChance = .3f;

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            (BattlerType ownType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            foreach (Battler ally in battleManager.Battlers.GetBattlersFighting(ownType)
                                                  .Where(ally => battler != ally && ally.GetStatus() != null)
                                                  .Where(_ => !(battleManager.RandomProvider.Value01()
                                                              > HealChance)))
            {
                ShowAbilityNotification(battler);

                yield return battleManager.Statuses.RemoveStatus(ally);
            }
        }
    }
}