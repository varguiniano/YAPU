using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability SpeedBoost.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SpeedBoost", fileName = "SpeedBoost")]
    public class SpeedBoost : Ability
    {
        /// <summary>
        /// Stats to be boosted at the eot.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, short> StatsToBoost;

        /// <summary>
        /// Trigger the boosts.
        /// </summary>
        public override IEnumerator AfterTurnPostStatus(Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer)
        {
            ShowAbilityNotification(battler);

            foreach (KeyValuePair<Stat, short> pair in StatsToBoost)
                yield return battleManager.BattlerStats.ChangeStatStage(battler, pair.Key, pair.Value, battler, this);
        }
    }
}