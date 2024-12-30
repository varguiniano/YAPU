using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for DragonCheer.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dragon/DragonCheer", fileName = "DragonCheer")]
    public class DragonCheer : StageChangeMove
    {
        // TODO: Animation.

        /// <summary>
        /// Stage change multipliers to apply to different types.
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> StageMultipliersPerType;

        /// <summary>
        /// Fail if the stage has already been raised.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
         || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).CriticalStage > 0;

        /// <summary>
        /// Get a multiplier for the stage set in the data.
        /// Multiplier for certain types.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="targetType">Current target.</param>
        /// <param name="targetIndex">Current target,</param>
        /// <param name="targets">Move targets.</param>
        /// <returns>The multiplier to apply.</returns>
        protected override float GetStageChangeMultiplier(BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          List<(BattlerType Type, int Index)> targets)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            float multiplier =
                base.GetStageChangeMultiplier(battleManager, userType, userIndex, targetType, targetIndex, targets);

            foreach (KeyValuePair<MonsterType, float> pair in StageMultipliersPerType)
                if (target.IsOfType(pair.Key, battleManager.YAPUSettings))
                    multiplier *= pair.Value;

            return multiplier;
        }
    }
}