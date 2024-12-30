using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that reduce the damage taken from super effective moves.
    /// </summary>
    public abstract class ReduceDamageFromSuperEffectiveAbility : Ability
    {
        /// <summary>
        /// Threshold from which the effect should trigger.
        /// </summary>
        [SerializeField]
        private float EffectivenessThreshold = 1f;

        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = .75f;

        /// <summary>
        /// Called when the owner is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="finished">Callback stating the new effectiveness.</param>
        public override IEnumerator OnHitByMove(DamageMove move,
                                                float effectiveness,
                                                Battler owner,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                Action<float> finished)
        {
            if (effectiveness <= EffectivenessThreshold)
            {
                finished.Invoke(effectiveness);
                yield break;
            }

            ShowAbilityNotification(owner);

            effectiveness *= Multiplier;

            yield return DialogManager.WaitForDialog;

            finished.Invoke(effectiveness);
        }
    }
}