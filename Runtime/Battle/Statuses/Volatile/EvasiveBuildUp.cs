using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Base class for build up statuses that allow the monster to evade other moves.
    /// </summary>
    public abstract class EvasiveBuildUp : TwoTurnMoveBuildUp
    {
        /// <summary>
        /// Moves that still hit when building up and multipliers to their power.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [SerializeField]
        private SerializableDictionary<Move, float> BypassingMoves;

        /// <summary>
        /// Abilities that make moves still hit when building up and multipliers to their power.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [SerializeField]
        private SerializableDictionary<Ability, float> BypassingAbilities;

        /// <summary>
        /// Reset the sprite position.
        /// </summary>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield return base.OnRemoveStatus(battleManager, battler, playAnimation);

            BattleMonsterSprite monsterSprite = battleManager.GetMonsterSprite(battler);

            // It may have been moved by the animation of a previous turn, like with fly.
            if (monsterSprite.ShouldResetOnMoveFail) monsterSprite.ResetSpritePosition();
        }

        /// <summary>
        /// Does this status allow the monster to be affected by terrain?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="terrain">Terrain in place.</param>
        /// <returns>True if is affected by the terrain.</returns>
        public override bool IsAffectedByTerrain(Battler battler, Terrain.Terrain terrain) => false;

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowUsedMessageNormally,
                                                         Action<bool> callback)
        {
            if (BypassingMoves.ContainsKey(move))
                callback.Invoke(true);
            else if (user.CanUseAbility(battleManager, false) && BypassingAbilities.ContainsKey(user.GetAbility()))
                callback.Invoke(true);
            else
            {
                if (!didShowUsedMessageNormally)
                    DialogManager.ShowDialog("Battle/Move/Used",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            user.GetNameOrNickName(battleManager.Localizer),
                                                            battleManager.Localizer[move.LocalizableName]
                                                        },
                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Evaded",
                                                             localizableModifiers: false,
                                                             modifiers: target
                                                                .GetNameOrNickName(battleManager.Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                callback.Invoke(false);
            }
        }

        /// <summary>
        /// Get the power of a move that is going to hit this battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Battler being hit, owner of this status.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float GetMovePowerMultiplierWhenHit(BattleManager battleManager,
                                                            Move move,
                                                            Battler user,
                                                            Battler target)
        {
            float multiplier = BypassingMoves.TryGetValue(move, out float moveMultiplier) ? moveMultiplier : 1;

            if (user.CanUseAbility(battleManager, false))
                multiplier *= BypassingAbilities.TryGetValue(user.GetAbility(), out float abilityMultiplier)
                                  ? abilityMultiplier
                                  : 1;

            return base.GetMovePowerMultiplierWhenHit(battleManager, move, user, target) * multiplier;
        }

        /// <summary>
        /// Can this monster be caught by a ball?
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can be caught.</returns>
        public override bool CanBeCaught(Battler battler, BattleManager battleManager) => false;
    }
}