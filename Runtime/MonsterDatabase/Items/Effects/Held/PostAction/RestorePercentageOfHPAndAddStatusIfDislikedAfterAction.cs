using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will heal a percentage of HP after an action has been performed and a threshold is met.
    /// It will also add a volatile status if the holder dislikes the flavour.
    /// </summary>
    [CreateAssetMenu(menuName =
                         "YAPU/Items/Effects/Held/PostAction/RestorePercentageOfHPAndAddStatusIfDislikedAfterAction",
                     fileName = "RestorePercentageOfHPAndAddStatusIfDislikedAfterAction")]
    public class RestorePercentageOfHPAndAddStatusIfDislikedAfterAction : RestorePercentageOfHPAfterAction
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Add status if the holder dislikes the flavour.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        protected override IEnumerator ExtraEffect(Item item,
                                                   Battler battler,
                                                   BattleAction action,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer)
        {
            Berry berry = item as Berry;

            if (berry == null)
            {
                Logger.Error(item.GetName(localizer) + " is not a berry!");
                yield break;
            }

            if (berry.FlavourData[battler.StatData.Nature.GetDislikedFlavour()] <= 0) yield break;

            (BattlerType userType, int userIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.Statuses.AddStatus(Status,
                                                          Status.CalculateRandomCountdown(battleManager,
                                                                   userType,
                                                                   userIndex,
                                                                   userType,
                                                                   userIndex),
                                                          battler,
                                                          userType,
                                                          userIndex,
                                                          false);
        }
    }
}