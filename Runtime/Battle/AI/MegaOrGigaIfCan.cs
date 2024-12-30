using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that changes to mega or giga forms if available, falls back if not.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/MegaOrGigaIfCan", fileName = "MegaOrGigaIfCan")]
    public class MegaOrGigaIfCan : BattleAI
    {
        /// <summary>
        /// Fallback to go to.
        /// </summary>
        [InfoBox("This AI changes to mega or giga forms if available, falls back if not.")]
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
            BattleAction action = default;

            yield return Fallback.RequestPerformAction(settings,
                                                       battleManager,
                                                       type,
                                                       inBattleIndex,
                                                       newAction => action = newAction);

            if (battleManager.Megas.CanMegaevolve(type, inBattleIndex)) action.TriggerMegaForm = true;

            callback.Invoke(action);
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