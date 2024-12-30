using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// FriendGuard ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/FriendGuard", fileName = "FriendGuard")]
    public class FriendGuard : Ability
    {
        /// <summary>
        /// Multiplier for moves that hit allies.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float AllyHitMultiplier = .75f;

        /// <summary>
        /// Called when calculating a move's damage on an ally.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenAllyTargeted(Battler owner,
                                                                          DamageMove move,
                                                                          float multiplier,
                                                                          Battler user,
                                                                          Battler target,
                                                                          BattleManager battleManager,
                                                                          Action<float> finished)
        {
            ShowAbilityNotification(owner);

            finished.Invoke(multiplier * AllyHitMultiplier);
            yield break;
        }
    }
}