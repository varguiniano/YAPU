using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that lower stats stages when entering the battlefield.
    /// </summary>
    public abstract class ETBStatLoweringAbility : Ability
    {
        /// <summary>
        /// Stats to change and the number of stages to change them.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Stat, short> StatChanges;

        // TODO: Battle stats and critical stage?

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

            (BattlerType battlerType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            BattlerType opponentType = battlerType == BattlerType.Ally ? BattlerType.Enemy : BattlerType.Ally;

            // TODO: If we implement triple battles this should only affect adjacent opponents.
            foreach (Battler opponent in battleManager.Battlers.GetBattlersFighting(opponentType))
            {
                if (opponent.CanUseAbility(battleManager, IgnoresOtherAbilities(battleManager, battler, null))
                 && !opponent.GetAbility().AffectsUserOfEffect(battler, opponent, IgnoresOtherAbilities(battleManager, battler, null), battleManager))
                {
                    opponent.GetAbility().ShowAbilityNotification(opponent);

                    continue;
                }

                foreach (KeyValuePair<Stat, short> statChange in StatChanges)
                    yield return battleManager.BattlerStats.ChangeStatStage(opponent,
                                                                            statChange.Key,
                                                                            statChange.Value,
                                                                            battler,
                                                                            this);
            }
        }
    }
}