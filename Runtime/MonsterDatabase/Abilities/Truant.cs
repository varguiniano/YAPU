using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Truant.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Truant", fileName = "Truant")]
    public class Truant : Ability
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected VolatileStatus Status;

        /// <summary>
        /// At the end of the turn, if it doesn't have the loafing around status, set it.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            yield return base.AfterTurnPreStatus(battler, battleManager, localizer);

            if (battleManager.Statuses.HasStatus(Status, battler)) yield break;

            (BattlerType battlerType, int battlerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            ShowAbilityNotification(battler);

            yield return battleManager.Statuses.AddStatus(Status, 2, battler, battlerType, battlerIndex, false);
        }
    }
}