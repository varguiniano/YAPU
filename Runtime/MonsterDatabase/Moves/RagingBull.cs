using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for RagingBull.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/RagingBull", fileName = "RagingBull")]
    public class RagingBull : DamageAndRemoveStatusMove
    {
        // TODO: Animation.

        /// <summary>
        /// If the user has one of this forms, change the move type.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Form, MonsterType> TypesPerForm;

        /// <summary>
        /// Change the type if the user has one of the forms.
        /// </summary>
        public override MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager)
        {
            MonsterType type = base.GetMoveTypeInBattle(battler, battleManager);

            if (TypesPerForm.TryGetValue(battler.Form, out MonsterType newType)) type = newType;

            return type;
        }

        /// <summary>
        /// Change the type if the user has one of the forms.
        /// </summary>
        public override MonsterType GetMoveType(MonsterInstance monster, YAPUSettings settings)
        {
            MonsterType type = base.GetMoveType(monster, settings);

            if (monster != null && TypesPerForm.TryGetValue(monster.Form, out MonsterType newType)) type = newType;

            return type;
        }

        /// <summary>
        /// Remove the statuses before hitting.
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
            yield return RemoveStatuses(battleManager,
                                        userType,
                                        userIndex,
                                        user,
                                        targets,
                                        hitNumber,
                                        expectedHits,
                                        externalPowerMultiplier,
                                        ignoresAbilities);

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
        }

        /// <summary>
        /// No secondary effect, it removes the statuses first.
        /// </summary>
        public override bool HasSecondaryEffect() => false;

        /// <summary>
        /// No secondary effect, it removes the statuses first.
        /// </summary>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            yield break;
        }
    }
}