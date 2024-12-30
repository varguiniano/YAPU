using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move VenomDrench.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/VenomDrench", fileName = "VenomDrench")]
    public class VenomDrench : StageChangeMove
    {
        // TODO: Animation.

        /// <summary>
        /// Statuses that count as poisoned.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private List<Status> PoisonStatuses;

        /// <summary>
        /// Fail if there are no poisoned targets.
        /// </summary>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            customFailMessage = "";

            if (!targets.Any(battlerData =>
                                 PoisonStatuses.Contains(battleManager.Battlers
                                                                      .GetBattlerFromBattleIndex(battlerData)
                                                                      .GetStatus())))
                return true;

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage);
        }

        /// <summary>
        /// Remove the non poisoned targets.
        /// </summary>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets) =>
            base.SelectFinalTargets(battleManager, userType, userIndex, targets)
                .Where(battlerData =>
                           PoisonStatuses.Contains(battleManager.Battlers.GetBattlerFromBattleIndex(battlerData)
                                                                .GetStatus()))
                .ToList();

        /// <summary>
        /// Fail on not poisoned targets.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            !PoisonStatuses.Contains(battleManager.Battlers
                                                  .GetBattlerFromBattleIndex(targetType, targetIndex)
                                                  .GetStatus())
         || base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);
    }
}