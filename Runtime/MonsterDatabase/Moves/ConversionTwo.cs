using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Conversion2.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Conversion2", fileName = "Conversion2")]
    public class ConversionTwo : SetVolatileStatusMove
    {
        /// <summary>
        /// Fail if the target hasn't used a move.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager,
                              localizer,
                              userType,
                              userIndex,
                              targetType,
                              targetIndex,
                              ignoresAbilities)
         || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).LastPerformedAction.LastMove
         == null;

        /// <summary>
        /// Get resisting types for the last move used by the target.
        /// </summary>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            return new object[]
                   {
                       target.LastPerformedAction.LastMove.GetMoveTypeInBattle(target, battleManager)
                             .AttackMultipliers.Where(pair => pair.Value < 1)
                             .Select(pair => pair.Key)
                             .ToList()
                             .Random(),
                       null
                   };
        }
    }
}