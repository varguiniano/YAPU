using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Download.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Download", fileName = "Download")]
    public class Download : Ability
    {
        /// <summary>
        /// Iterate the opponents and lower their stats.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="battler"></param>
        /// <returns></returns>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            ShowAbilityNotification(battler);

            (BattlerType ownType, int ownIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            List<Battler> opponents =
                battleManager.Battlers.GetBattlersFighting(ownType == BattlerType.Ally
                                                               ? BattlerType.Enemy
                                                               : BattlerType.Ally);

            uint defense = 0;
            uint specialDefense = 0;

            foreach (Dictionary<Stat, uint> stats in opponents.Select(opponent => opponent.GetStats(battleManager)))
            {
                defense += stats[Stat.Defense];
                specialDefense += stats[Stat.SpecialDefense];
            }

            if (defense > specialDefense)
                yield return battleManager.BattlerStats.ChangeStatStage(battler, Stat.Attack, 1, battler, this);
            else
                yield return battleManager.BattlerStats.ChangeStatStage(battler, Stat.SpecialAttack, 1, battler, this);
        }
    }
}