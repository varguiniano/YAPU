using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move HealPulse.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/HealPulse", fileName = "HealPulse")]
    public class HealPulse : HPPercentageRegenMove
    {
        /// <summary>
        /// Regen percentage by default.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float DefaultRegenPercentage = 0.5f;

        /// <summary>
        /// Regen percentage if the user has certain abilities.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Ability, float> AbilityOverrides;

        /// <summary>
        /// Get the percentage of HP to regen.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="userType"></param>
        /// <param name="userIndex"></param>
        /// <param name="targetType"></param>
        /// <param name="targetIndex"></param>
        /// <returns>A number between 0 and 1.</returns>
        protected override float GetRegenPercentage(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            Ability ability = user.GetAbility();

            if (!user.CanUseAbility(battleManager, false) || !AbilityOverrides.ContainsKey(ability))
                return DefaultRegenPercentage;

            ability.ShowAbilityNotification(user);
            return AbilityOverrides[ability];
        }

        /// <summary>
        /// Play the move animation.
        /// TODO: Animation.
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
            if (UsesFallbackAnimation)
                yield return FallbackAnimation.PlayAnimation(battleManager,
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