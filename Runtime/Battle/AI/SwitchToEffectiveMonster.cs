using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that switches to a more effective monster if available. Falls back if there are monsters to switch with super effective moves.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/SwitchToEffectiveMonster", fileName = "SwitchToEffectiveMonster")]
    public class SwitchToEffectiveMonster : BattleAI
    {
        /// <summary>
        /// Anything above this threshold will be considered an effective move.
        /// </summary>
        [InfoBox("This AI switches to a more effective monster if available. "
               + "The chance determines if it will switch or not, to a bit of randomization."
               + "Falls back if there are monsters to switch with super effective moves."
               + "This AI also chooses the battler to send after fainted (it they have an effective move, falls back otherwise).")]
        [SerializeField]
        private float EffectivenessThreshold = 1f;

        /// <summary>
        /// Chance to switch to a more effective monster.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float SwitchingChance;

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
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            if (!battler.CanSwitch(battleManager, type, inBattleIndex, null, false, null, false, false)
             || (type == BattlerType.Enemy
              && battleManager.EnemyType == EnemyType.Wild))
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            int newBattler = DetermineMoreEffectiveMonster(battleManager, type, inBattleIndex, new List<Battler>());

            if (newBattler == -1)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            float dontSwitchChance = battleManager.RandomProvider.Value01();

            if (dontSwitchChance > SwitchingChance)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            callback.Invoke(new BattleAction
                            {
                                BattlerType = type,
                                Index = inBattleIndex,
                                ActionType = BattleAction.Type.Switch,
                                Parameters = new[] {newBattler}
                            });
        }

        /// <summary>
        /// Request the AI to send a new monster after a monster has fainted.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index of the monster that was forced out.</param>
        /// <param name="forbiddenBattlers">Battlers that can't be sent in.</param>
        /// <returns>The index of the monster to send from the AI's roster.</returns>
        public override int
            RequestNewMonster(YAPUSettings settings,
                              BattleManager battleManager,
                              BattlerType type,
                              int inBattleIndex,
                              List<Battler> forbiddenBattlers)
        {
            int newBattler = DetermineMoreEffectiveMonster(battleManager, type, inBattleIndex, forbiddenBattlers);

            return newBattler == -1
                       ? Fallback.RequestNewMonster(settings, battleManager, type, inBattleIndex, forbiddenBattlers)
                       : newBattler;
        }

        /// <summary>
        /// Determine the mover effective monster index to send with the enemies in the battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="forbiddenBattlers">Battlers that can't be sent in.</param>
        /// <returns>The index of the monster to send.</returns>
        private int DetermineMoreEffectiveMonster(BattleManager battleManager,
                                                  BattlerType type,
                                                  int inBattleIndex,
                                                  ICollection<Battler> forbiddenBattlers)
        {
            List<Battler> nonFighting = battleManager.Battlers.GetBattlersNotFighting(type, inBattleIndex)
                                                     .Where(battler => battler.CanBattle
                                                                    && !forbiddenBattlers.Contains(battler))
                                                     .ToList();

            if (nonFighting.Count == 0)
            {
                Logger.Warn("There are no battlers available in roster " + type + " " + inBattleIndex + ".");
                return -1;
            }

            List<Battler> enemies = GetEnemies(battleManager, type);

            foreach (Battler battler in nonFighting)
            {
                foreach (MoveSlot slot in battler.GetUsableDamageMoves(battleManager))
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

                        if (effectiveness > EffectivenessThreshold)
                            return battleManager.Rosters.GetRoster(type, inBattleIndex).IndexOf(battler);
                    }
                }
            }

            Logger.Info("No monster with effective moves found.");

            return -1;
        }
    }
}