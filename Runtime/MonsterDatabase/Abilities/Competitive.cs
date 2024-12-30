using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Competitive.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Competitive", fileName = "Competitive")]
    public class Competitive : Ability
    {
        /// <summary>
        /// Stat to raise.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Stat StatToRaise;

        /// <summary>
        /// Stages to raise.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private short Stages;

        /// <summary>
        /// Raise the stat if a stat has been lowered by an opponent.
        /// </summary>
        public override IEnumerator AfterStatChanged(Battler owner,
                                                     Stat stat,
                                                     short modifier,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattleManager battleManager)
        {
            yield return AfterStatChanged(owner, modifier, userType, battleManager);
        }

        /// <summary>
        /// Raise the stat if a stat has been lowered by an opponent.
        /// </summary>
        public override IEnumerator AfterStatChanged(Battler owner,
                                                     BattleStat stat,
                                                     short modifier,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattleManager battleManager)
        {
            yield return AfterStatChanged(owner, modifier, userType, battleManager);
        }

        /// <summary>
        /// Raise the stat if a stat has been lowered by an opponent.
        /// </summary>
        private IEnumerator AfterStatChanged(Battler owner,
                                             short modifier,
                                             BattlerType userType,
                                             BattleManager battleManager)
        {
            if (battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type == userType || modifier >= 0) yield break;

            ShowAbilityNotification(owner);
            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToRaise, Stages, owner, this);
        }
    }
}