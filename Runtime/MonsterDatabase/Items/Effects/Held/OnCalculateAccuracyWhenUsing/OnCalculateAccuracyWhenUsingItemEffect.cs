using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAccuracyWhenUsing
{
    /// <summary>
    /// Data class for an item effect that modifies the accuracy of a move when the holder is the user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateAccuracyWhenUsing/SimpleModifier",
                     fileName = "OnCalculateAccuracyWhenUsing")]
    public class
        OnCalculateAccuracyWhenUsingItemEffect : MonsterDatabaseScriptable<OnCalculateAccuracyWhenUsingItemEffect>
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1f;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public float GetMoveAccuracyMultiplierWhenUsed(Item item,
                                                       BattleManager battleManager,
                                                       Battler target,
                                                       Battler user,
                                                       Move move) =>
            Multiplier;
    }
}