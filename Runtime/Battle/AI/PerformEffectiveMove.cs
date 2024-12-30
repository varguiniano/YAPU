using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that performs the most powerful effective moves they have. Falls back if there are no super effective moves.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/PerformEffectiveMove", fileName = "PerformEffectiveMove")]
    public class PerformEffectiveMove : BattleAI
    {
        /// <summary>
        /// Anything above this threshold will be considered an effective move.
        /// </summary>
        [InfoBox("This AI performs the most powerful effective damage move they have. "
               + "If chance is not met, it will fall back. "
               + "Anything above the threshold will be considered an effective move. "
               + "Among effective, it will prioritize multiple targeting. "
               + "Among single targeting, it will prioritize more move power. "
               + "Falls back if there are no super effective moves (or no available moves at all).")]
        [SerializeField]
        private float EffectivenessThreshold = 1f;

        /// <summary>
        /// Chance to switch to a more effective monster.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float Chance;

        /// <summary>
        /// Fallback to go to.
        /// </summary>
        [SerializeField]
        private BattleAI Fallback;

        /// <summary>
        /// Request to choose to perform an action.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="callback">Callback stating the action to take along with its parameters.</param>
        /// <returns>The action taken along with its parameters.</returns>
        public override IEnumerator RequestPerformAction(YAPUSettings settings,
                                                         BattleManager battleManager,
                                                         BattlerType type,
                                                         int inBattleIndex,
                                                         Action<BattleAction> callback)
        {
            float fallChance = battleManager.RandomProvider.Value01();

            if (fallChance > Chance)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            List<Battler> enemies = GetEnemies(battleManager, type);

            List<MoveSlot> movesAvailable = battler.GetUsableDamageMoves(battleManager);

            Dictionary<MoveSlot, Battler> moveCandidates = new();

            // If there are no available moves to perform, fallback.
            if (movesAvailable.Count <= 0)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            foreach (MoveSlot slot in movesAvailable)
            {
                foreach (Battler enemy in enemies)
                {
                    if (!enemy.GetEffectivenessOfMove(battler,
                                                      slot.Move,
                                                      false,
                                                      battleManager,
                                                      false,
                                                      out float effectiveness))
                        continue;

                    if (effectiveness > EffectivenessThreshold) moveCandidates[slot] = enemy;
                }
            }

            // If there are no super effective moves, fallback.
            if (moveCandidates.Count == 0)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            List<MoveSlot> multipleTargetCandidates =
                (from moveCandidate in moveCandidates
                 where moveCandidate.Key.Move.CanHaveMultipleTargets
                 select moveCandidate.Key).ToList();

            // Prioritize multiple targets.
            if (multipleTargetCandidates.Count > 0)
            {
                MoveSlot bestMove = multipleTargetCandidates
                                   .OrderByDescending(slot =>
                                                          ((DamageMove) slot.Move).GetMovePowerInBattle(battleManager,
                                                              battler,
                                                              null,
                                                              false))
                                   .First();

                callback.Invoke(GenerateActionForMove(battleManager, type, inBattleIndex, battler, bestMove.Move));
            }
            else
            {
                MoveSlot bestMove = moveCandidates
                                   .Keys.OrderByDescending(slot =>
                                                               ((DamageMove) slot.Move)
                                                              .GetMovePowerInBattle(battleManager,
                                                                   battler,
                                                                   null,
                                                                   false))
                                   .First();

                callback.Invoke(GenerateActionForMove(battleManager,
                                                      type,
                                                      inBattleIndex,
                                                      battler,
                                                      bestMove.Move,
                                                      moveCandidates[bestMove]));
            }
        }

        /// <summary>
        /// Request the AI to send a new monster after a monster has fainted.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index of the monster that just fainted..</param>
        /// <param name="forbiddenBattlers"></param>
        /// <returns>The index of the monster to send from the AI's roster.</returns>
        public override int
            RequestNewMonster(YAPUSettings settings,
                              BattleManager battleManager,
                              BattlerType type,
                              int inBattleIndex,
                              List<Battler> forbiddenBattlers) =>
            Fallback.RequestNewMonster(settings, battleManager, type, inBattleIndex, forbiddenBattlers);
    }
}