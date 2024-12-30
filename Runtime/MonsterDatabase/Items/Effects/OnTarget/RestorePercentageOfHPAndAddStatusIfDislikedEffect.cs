using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an item effect that restores a percentage of the HP of a monster.
    /// It will also add a volatile status if the holder dislikes the flavour.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RestorePercentageOfHPAndAddStatusIfDislikedEffect",
                     fileName = "RestorePercentageOfHPAndAddStatusIfDislikedEffect")]
    public class RestorePercentageOfHPAndAddStatusIfDislikedEffect : RestorePercentageOfHPEffect
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Add status if the holder dislikes the flavour.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="localizer">Localizer reference.</param>
        protected override IEnumerator ExtraEffect(Item item,
                                                   Battler battler,
                                                   BattleManager battleManager,
                                                   YAPUSettings settings,
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