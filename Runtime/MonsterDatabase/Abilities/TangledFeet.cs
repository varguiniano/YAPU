using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability TangledFeet.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/TangledFeet", fileName = "TangledFeet")]
    public class TangledFeet : Ability
    {
        /// <summary>
        /// Reference to the confusion status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private VolatileStatus Confusion;

        /// <summary>
        /// Double evasion when confused.
        /// </summary>
        public override float OnCalculateEvasionStage(Battler monster, BattleManager battleManager) =>
            battleManager != null && battleManager.Statuses.HasStatus(Confusion, monster) ? 2 : 1;
    }
}