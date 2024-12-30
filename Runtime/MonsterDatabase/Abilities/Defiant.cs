using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Defiant.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Defiant", fileName = "Defiant")]
    public class Defiant : Ability
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

            if (userType == battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type) yield break;

            ShowAbilityNotification(owner);

            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToChange, Stages, owner, this);
        }
    }
}