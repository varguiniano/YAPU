using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that heal the monster when it's hit by moves of the specified types.
    /// </summary>
    public abstract class HealWhenHitWithSpecificTypesAbility : Ability
    {
        /// <summary>
        /// Types that heal instead of doing their effect when hitting this monster.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> HPHealingTypes;

        /// <summary>
        /// Replace the move's effect when for healing.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public override IEnumerator ShouldReplaceMoveEffectWhenHit(Battler owner,
                                                                   Move move,
                                                                   Battler user,
                                                                   BattleManager battleManager,
                                                                   Action<bool> callback)
        {
            MonsterType moveType = move.GetMoveTypeInBattle(user, battleManager);

            if (HPHealingTypes.SerializedList.All(slot => slot.Key != moveType) || owner == user)
            {
                callback.Invoke(true);
                yield break;
            }

            if (!owner.CanHeal(battleManager))
            {
                callback.Invoke(false);
                yield break;
            }

            ShowAbilityNotification(owner);

            battleManager.GetMonsterSprite(owner).FXAnimator.PlayBoost(battleManager.BattleSpeed);

            int hpToHeal = (int) (owner.GetStats(battleManager)[Stat.Hp] * HPHealingTypes[moveType]);

            yield return battleManager.BattlerHealth.ChangeLife(owner, owner, hpToHeal);

            battleManager.Animation.UpdatePanels();

            callback.Invoke(false);
        }
    }
}