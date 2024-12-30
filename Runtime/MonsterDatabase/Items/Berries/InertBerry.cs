using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Data class for a berry that has no direct bag uses.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Berries/InertBerry", fileName = "InertBerry")]
    public class InertBerry : Berry
    {
        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// This berry has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}