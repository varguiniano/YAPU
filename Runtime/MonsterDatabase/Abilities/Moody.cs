using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability Moody.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Moody", fileName = "Moody")]
    public class Moody : Ability
    {
        /// <summary>
        /// Trigger the boosts.
        /// </summary>
        public override IEnumerator AfterTurnPostStatus(Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer)
        {
            Dictionary<Stat, uint> stats = battler.GetStats(battleManager);

            Stat statToBoost;

            List<Stat> checkedStats = new();

            do
            {
                statToBoost = stats.Keys.ToList().Random();

                if (stats[statToBoost] != 6) continue;

                checkedStats.AddIfNew(statToBoost);
                statToBoost = Stat.Hp;
            }
            while (statToBoost == Stat.Hp && checkedStats.Count < stats.Count);

            if (statToBoost == Stat.Hp) yield break;

            ShowAbilityNotification(battler);

            Stat statToDecrease = stats.Keys.Where(stat => stat != Stat.Hp && stat != statToBoost).ToList().Random();

            yield return battleManager.BattlerStats.ChangeStatStage(battler, statToBoost, 2, battler, this);
            yield return battleManager.BattlerStats.ChangeStatStage(battler, statToDecrease, -1, battler, this);
        }
    }
}