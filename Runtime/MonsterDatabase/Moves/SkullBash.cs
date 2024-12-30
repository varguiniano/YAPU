using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move SkullBash.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/SkullBash", fileName = "SkullBash")]
    public class SkullBash : TwoTurnDamageMove
    {
        /// <summary>
        /// Rotation when it lowers its head.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 LowerHeadRotation;

        /// <summary>
        /// Stat to rise when lowering the head.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Stat StatToRise;

        /// <summary>
        /// Used to add an extra effect to the first turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="targets">Targets of the move.</param>
        protected override IEnumerator FirstTurnExtraEffect(BattleManager battleManager,
                                                            BattlerType userType,
                                                            int userIndex,
                                                            bool ignoresAbilities,
                                                            List<(BattlerType Type, int Index)> targets)
        {
            yield return battleManager.BattlerStats.ChangeStatStage(userType,
                                                                    userIndex,
                                                                    StatToRise,
                                                                    1,
                                                                    userType,
                                                                    userIndex,
                                                                    ignoreAbilities: ignoresAbilities);
        }

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
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

            bool finished = false;

            sprite.Pivot.DOPunchRotation(LowerHeadRotation, 1f / speed, 1, .2f).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }

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
            yield return base.PlayAnimation(battleManager,
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