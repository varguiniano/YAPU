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
    /// Data class for the ability Justified.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Justified", fileName = "Justified")]
    public class Justified : Ability
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

            if (!TriggeringTypes.Contains(move.GetMoveTypeInBattle(user, battleManager))) yield break;

            ShowAbilityNotification(owner);

            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToChange, Stages, owner, this);
        }
    }
}