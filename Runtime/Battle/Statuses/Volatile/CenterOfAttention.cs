using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Base class for statuses that make the target the center of attention.
    /// All moves that can target them will do so.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/CenterOfAttention",
                     fileName = "CenterOfAttention")]
    public class CenterOfAttention : VolatileStatus
    {
        /// <summary>
        /// Moves immune to this effect.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private List<Move> ImmuneMoves;

        /// <summary>
        /// Held items immune to this effect.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        private List<Item> ImmuneItems;

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
        /// Move the target here if it can target.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected"></param>
        /// <param name="finished">Callback stating if the move will still be used, if the targets are modified and the new targets for the move.</param>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, bool,
                                                                      List<(BattlerType Type, int Index)>> finished)
        {
            // Only affects single target moves.
            if (move.CanHaveMultipleTargets || ImmuneMoves.Contains(move))
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            if (user.CanUseHeldItemInBattle(battleManager) && ImmuneItems.Contains(user.HeldItem))
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            (BattlerType ownType, int ownIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            // Only affect opponents.
            if (ownType == userType)
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            // If the move can't target the battler, do nothing.
            List<Battler> potentialTargets =
                MoveUtils.GenerateValidTargetsForMove(battleManager, userType, userIndex, move, StaticLogger);

            if (!potentialTargets.Contains(owner))
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            finished.Invoke(true, true, new List<(BattlerType Type, int Index)> { (ownType, ownIndex) });
        }
    }
}