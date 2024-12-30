﻿using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing a damage move that recoils the user.
    /// </summary>
    public abstract class DamageWithRecoilBasedOnDamageDone : DamageWithRecoilMove
    {
        /// <summary>
        /// Get the value needed to the recoil damage.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>The value to be used when calculating the recoil.</returns>
        protected override float GetValueForRecoil(BattleManager battleManager,
                                                   BattlerType userType,
                                                   int userIndex,
                                                   List<(BattlerType Type, int Index)> targets) =>
            LastDamageMade;
    }
}