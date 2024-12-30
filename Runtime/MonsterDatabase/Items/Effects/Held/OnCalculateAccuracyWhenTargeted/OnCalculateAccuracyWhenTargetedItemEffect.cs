using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAccuracyWhenTargeted
{
    /// <summary>
    /// Data class for an item effect that modifies the accuracy of a move when the holder is targeted.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateAccuracyWhenTargeted/SimpleModifier",
                     fileName = "SimpleModifierOnCalculateAccuracyWhenTargeted")]
    public class
        OnCalculateAccuracyWhenTargetedItemEffect : MonsterDatabaseScriptable<OnCalculateAccuracyWhenTargetedItemEffect>
    {
        /// <summary>
        /// Modifier to apply.
        /// </summary>
        [SerializeField]
        private float Modifier = 1f;

        /// <summary>
        /// Called when calculating the move's accuracy.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="item">Item held.</param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public float OnCalculateMoveAccuracyWhenTargeted(Move move,
                                                         Item item,
                                                         Battler user,
                                                         Battler target,
                                                         BattleManager battleManager) =>
            Modifier;
    }
}