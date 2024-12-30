using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that runs if it is a wild mon with a run chance and then fallback.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/IfWildWithRunChanceRun", fileName = "IfWildWithRunChanceRun")]
    public class IfWildWithRunChanceRun : BattleAI
    {
        /// <summary>
        /// Fallback to go to.
        /// </summary>
        [InfoBox("This AI checks if the monster is wild and its species has a chance to run. Runs if the random chance is met and passes on to the fallback if not.")]
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
            BattleAction action = new()
                                  {
                                      BattlerType = type,
                                      Index = inBattleIndex
                                  };

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            // ReSharper disable once InvertIf
            if (battler.CanRunAway(battleManager, false, false)
             && battleManager.EnemyType == EnemyType.Wild
             && type == BattlerType.Enemy)
            {
                float runChance = battleManager.RandomProvider.Value01();

                // ReSharper disable once InvertIf
                if (runChance < battler.FormData.WildRunChance)
                {
                    action.ActionType = BattleAction.Type.Run;
                    callback.Invoke(action);
                    yield break;
                }
            }

            yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
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