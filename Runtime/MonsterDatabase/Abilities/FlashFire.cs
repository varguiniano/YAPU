using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability Flash Fire.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/FlashFire", fileName = "FlashFire")]
    public class FlashFire : Ability
    {
        /// <summary>
        /// Reference to the type this ability affects.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType AffectedType;

        /// <summary>
        /// Battles for which the ability has been activated.
        /// </summary>
        private List<Battler> activatedBattlers;

        /// <summary>
        /// Recreate the list, just in case.
        /// </summary>
        public override IEnumerator OnBattleStarted(BattleManager battleManager, Battler battler)
        {
            activatedBattlers = new List<Battler>();

            yield return base.OnBattleStarted(battleManager, battler);
        }

        /// <summary>
        /// Replace the move's effect when for raising the stat.
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
            if (move.GetMoveTypeInBattle(user, battleManager) != AffectedType || owner == user)
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(owner);

            activatedBattlers.AddIfNew(owner);

            callback.Invoke(false);
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        public override IEnumerator OnCalculateMoveDamageWhenUsing(DamageMove move,
                                                                   float multiplier,
                                                                   Battler user,
                                                                   Battler target,
                                                                   float effectiveness,
                                                                   bool isCritical,
                                                                   int hitNumber,
                                                                   int expectedHitNumber,
                                                                   bool ignoresAbilities,
                                                                   List<(BattlerType Type, int Index)> allTargets,
                                                                   BattleManager battleManager,
                                                                   ILocalizer localizer,
                                                                   Action<float> finished)
        {
            yield return base.OnCalculateMoveDamageWhenUsing(move,
                                                             multiplier,
                                                             user,
                                                             target,
                                                             effectiveness,
                                                             isCritical,
                                                             hitNumber,
                                                             expectedHitNumber,
                                                             ignoresAbilities,
                                                             allTargets,
                                                             battleManager,
                                                             localizer,
                                                             newMultiplier => multiplier = newMultiplier);

            if (move.GetMoveTypeInBattle(user, battleManager) != AffectedType || !activatedBattlers.Contains(user))
            {
                finished.Invoke(multiplier);
                yield break;
            }

            multiplier *= 1.5f;
            ShowAbilityNotification(user);

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Called when the monster is withdrawn from battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that left the battle.</param>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            if (activatedBattlers.Contains(battler)) activatedBattlers.Remove(battler);

            yield return base.OnMonsterLeavingBattle(battleManager, battler);
        }

        /// <summary>
        /// Clear the list on battle ended.
        /// </summary>
        public override IEnumerator OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            activatedBattlers.Clear();

            yield return base.OnBattleEnded(battler, battleManager);
        }
    }
}