using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Steadfast.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Steadfast", fileName = "Steadfast")]
    public class Steadfast : Ability
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
        /// Raise the stat when flinched.
        /// </summary>
        public override IEnumerator OnFlinched(Battler owner, BattleManager battleManager)
        {
            yield return base.OnFlinched(owner, battleManager);

            ShowAbilityNotification(owner);

            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToChange, Stages, owner, this);
        }
    }
}