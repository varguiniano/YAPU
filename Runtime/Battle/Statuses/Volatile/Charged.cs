using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Base class for the Charged status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Charged", fileName = "Charged")]
    public class Charged : VolatileStatus
    {
        /// <summary>
        /// Type that this status boosts.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType TypeToBoost;

        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float Multiplier = 2;
        
        /// <summary>
        /// No message.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="owner">Owner of the status and move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float GetMovePowerMultiplierWhenUsingMove(Battler owner,
                                                                  Move move,
                                                                  Battler target,
                                                                  BattleManager battleManager)
        {
            float multiplier = base.GetMovePowerMultiplierWhenUsingMove(owner, move, target, battleManager);

            if (move.GetMoveTypeInBattle(owner, battleManager) == TypeToBoost) multiplier *= Multiplier;

            return multiplier;
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// Charge is only removed when a move of the boosted type is used.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler, Move move, BattleManager battleManager, List<(BattlerType Type, int Index)> targets, Action<bool> finished)
        {
            if (move.GetMoveTypeInBattle(battler, battleManager) == TypeToBoost)
                battleManager.Statuses.ScheduleRemoveStatus(this, battler);
            
            return base.OnAboutToUseAMove(battler, move, battleManager, targets, finished);
        }
    }
}