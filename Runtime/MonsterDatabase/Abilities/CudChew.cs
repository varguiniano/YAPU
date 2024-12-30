using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// CudChew ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/CudChew", fileName = "CudChew")]
    public class CudChew : Ability
    {
        /// <summary>
        /// Berries currently being chewed and the amount of turns they have been eaten.
        /// </summary>
        private readonly Dictionary<Battler, List<(int, Berry)>> chewedBerries = new();

        /// <summary>
        /// Add the berry to the list of chewed berries.
        /// </summary>
        public override IEnumerator OnBerryEaten(Berry berry, Battler battler, BattleManager battleManager)
        {
            if (!chewedBerries.ContainsKey(battler)) chewedBerries[battler] = new List<(int, Berry)>();

            chewedBerries[battler].Add((0, berry));

            yield return base.OnBerryEaten(berry, battler, battleManager);
        }

        /// <summary>
        /// Remove the chewed list.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            if (chewedBerries.ContainsKey(battler)) chewedBerries.Remove(battler);

            yield return base.OnMonsterLeavingBattle(battleManager, battler);
        }

        /// <summary>
        /// Cud chew on the second turn.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            if (chewedBerries.TryGetValue(battler, out List<(int, Berry)> berries))
            {
                List<(int, Berry)> newBerryList = new();

                foreach ((int turn, Berry berry) in berries)
                    if (turn > 0)
                    {
                        ShowAbilityNotification(battler);

                        yield return battleManager
                                    .GetMonsterSprite(battleManager.Battlers.GetTypeAndIndexOfBattler(battler))
                                    .EatBerry(battleManager.BattleSpeed);

                        yield return berry.CudChewBerry(battler, battleManager);
                    }
                    else
                        newBerryList.Add((turn + 1, berry));

                chewedBerries[battler] = newBerryList;
            }

            yield return base.AfterTurnPreStatus(battler, battleManager, localizer);
        }
    }
}