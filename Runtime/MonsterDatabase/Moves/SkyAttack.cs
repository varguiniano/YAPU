using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move SkyAttack.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Flying/SkyAttack", fileName = "SkyAttack")]
    public class SkyAttack : StatusChanceTwoTurnDamageMove
    {
        /// <summary>
        /// Reference to the fallback to use for the second turn.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move SecondTurnFallback;

        /// <summary>
        /// Play the animation for the second turn of this move.
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
        public override IEnumerator PlaySecondTurnAnimation(BattleManager battleManager,
                                                            float speed,
                                                            BattlerType userType,
                                                            int userIndex,
                                                            Battler user,
                                                            Transform userPosition,
                                                            List<(BattlerType Type, int Index)> targets,
                                                            List<Transform> targetPositions,
                                                            bool ignoresAbilities)
        {
            yield return SecondTurnFallback.PlayAnimation(battleManager,
                                                          speed,
                                                          userType,
                                                          userIndex,
                                                          user,
                                                          userPosition,
                                                          targets,
                                                          targetPositions,
                                                          ignoresAbilities);
        }
    }
}