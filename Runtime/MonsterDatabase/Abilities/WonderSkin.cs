using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability WonderSkin.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/WonderSkin", fileName = "WonderSkin")]
    public class WonderSkin : Ability
    {
        /// <summary>
        /// Called when calculating the move's accuracy and targeting the holder.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override (float Multiplier, bool ForceClamp, (int Min, int Max) Clamp) OnCalculateAccuracyWhenTargeted(
            Move move,
            Battler user,
            Battler target,
            BattleManager battleManager)
        {
            if (move.GetMoveCategory(user, target, false, battleManager) != Move.Category.Status
             || move.GetOutOfBattleAccuracy() < 50)
                return base.OnCalculateAccuracyWhenTargeted(move, user, target, battleManager);

            ShowAbilityNotification(target, true);

            return (1, true, (0, 50));
        }
    }
}