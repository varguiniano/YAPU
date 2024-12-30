using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Rattled.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Rattled", fileName = "Rattled")]
    public class Rattled : Ability
    {
        /// <summary>
        /// Stat to change on trigger.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Stat StatToChange;

        /// <summary>
        /// Stages to change on trigger.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private short Stages;

        /// <summary>
        /// Moves that trigger this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [SerializeField]
        private List<MonsterType> TriggeringTypes;

        /// <summary>
        /// Abilities that rigger this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [SerializeField]
        private List<Ability> TriggeringAbilities;

        /// <summary>
        /// Trigger the effect.
        /// </summary>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler owner,
                                                   Battler user,
                                                   int damageDealt,
                                                   uint previousHP,
                                                   bool wasCritical,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   int hitNumber,
                                                   int expectedMoveHits,
                                                   BattleManager battleManager)
        {
            yield return base.AfterHitByMove(move,
                                             effectiveness,
                                             owner,
                                             user,
                                             damageDealt,
                                             previousHP,
                                             wasCritical,
                                             substituteTookHit,
                                             ignoresAbilities,
                                             hitNumber,
                                             expectedMoveHits,
                                             battleManager);

            if (TriggeringTypes.Contains(move.GetMoveTypeInBattle(user, battleManager)))
                yield return TriggerEffect(owner, battleManager);
        }

        /// <summary>
        /// Trigger the effect if it was chanced by a triggering ability.
        /// </summary>
        public override IEnumerator OnStatChange(Battler owner,
                                                 Stat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Ability changingAbility,
                                                 Action<short> callback)
        {
            yield return base.OnStatChange(owner,
                                           stat,
                                           modifier,
                                           userType,
                                           userIndex,
                                           battleManager,
                                           changingAbility,
                                           callback);

            if (TriggeringAbilities.Contains(changingAbility)) yield return TriggerEffect(owner, battleManager);
        }

        /// <summary>
        /// Trigger the ability.
        /// </summary>
        private IEnumerator TriggerEffect(Battler owner, BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToChange, Stages, owner, this);
        }
    }
}