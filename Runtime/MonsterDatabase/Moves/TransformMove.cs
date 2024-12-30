using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Transform.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Transform", fileName = "Transform")]
    public class TransformMove : Move
    {
        /// <summary>
        /// Reference to the transformed status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Transformed TransformedStatus;

        /// <summary>
        /// Also fail if user can't transform or target is transformed.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
         || !battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex).CanTransform(battleManager)
         || battleManager.Statuses.HasStatus(TransformedStatus, targetType, targetIndex)
         || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).Substitute.SubstituteEnabled;

        /// <summary>
        /// Transform into the first target.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            yield return battleManager.Battlers.TransformIntoTarget(userType,
                                                                    userIndex,
                                                                    targets[0].Type,
                                                                    targets[0].Index,
                                                                    TransformedStatus);
        }
    }
}