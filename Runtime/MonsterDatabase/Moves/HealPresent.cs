using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the Healing part of Present.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/HealPresent", fileName = "HealPresent")]
    public class HealPresent : HPPercentageRegenMove
    {
        /// <summary>
        /// Percentage to regen.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float RegenPercentage;

        /// <summary>
        /// Get the percentage of HP to regen.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="userType"></param>
        /// <param name="userIndex"></param>
        /// <param name="targetType"></param>
        /// <param name="targetIndex"></param>
        /// <returns>A number between 0 and 1.</returns>
        protected override float GetRegenPercentage(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex) =>
            RegenPercentage;
    }
}