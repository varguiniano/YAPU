using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move MeteorBeam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Rock/MeteorBeam", fileName = "MeteorBeam")]
    public class MeteorBeam : TwoTurnDamageMove
    {
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
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
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
            if (UsesFallbackAnimation)
                yield return ((TwoTurnMove) SecondTurnFallbackAnimation).PlaySecondTurnAnimation(battleManager,
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