using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability Telepathy.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Telepathy", fileName = "Telepathy")]
    public class Telepathy : Ability
    {
        /// <summary>
        /// Accuracy 0 if hit by ally.
        /// </summary>
        public override (float Multiplier, bool ForceClamp, (int Min, int Max) Clamp) OnCalculateAccuracyWhenTargeted(
            Move move,
            Battler user,
            Battler target,
            BattleManager battleManager)
        {
            if (move.GetMoveCategory(user, target, false, battleManager) == Move.Category.Status
             || battleManager.Battlers.GetTypeAndIndexOfBattler(user).Type
             != battleManager.Battlers.GetTypeAndIndexOfBattler(target).Type)
                return base.OnCalculateAccuracyWhenTargeted(move, user, target, battleManager);

            ShowAbilityNotification(target);

            return (0, false, (0, 0));
        }
    }
}