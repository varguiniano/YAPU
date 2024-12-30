using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Bind.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Bind", fileName = "Bind")]
    public class Bind : DamageAndSetVolatileAffectedByBindingBandMove
    {
        // TODO: Animation.

        /// <summary>
        /// Prepare the extra data for the volatile status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex)
        {
            List<object> data = base.PrepareExtraData(battleManager, userType, userIndex, targetType, targetIndex)
                                    .ToList();

            data.Add(battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex));

            return data.ToArray();
        }
    }
}