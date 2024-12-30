using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Infestation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Bug/Infestation", fileName = "Infestation")]
    public class Infestation : DamageAndSetVolatileAffectedByBindingBandMove
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
                                                  bool ignoresAbilities) =>
            targets.Select(battlerData => ((InfestationStatus) Status).PlayAnimation(battleManager,
                               battleManager.Battlers
                                            .GetBattlerFromBattleIndex(battlerData)))
                    // ReSharper disable once NotDisposedResourceIsReturned
                   .GetEnumerator();
    }
}