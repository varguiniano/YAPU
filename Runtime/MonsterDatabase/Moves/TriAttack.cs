using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Tri Attack.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/TriAttack", fileName = "TriAttack")]
    public class TriAttack : StatusChanceDamageMove
    {
        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                foreach (KeyValuePair<Status, float> pair in StatusChance)
                    yield return
                        pair.Key.PlayAnimation(battleManager.Battlers
                                                            .GetBattlerFromBattleIndex(targetType, targetIndex),
                                               battleManager);
            }
        }
    }
}