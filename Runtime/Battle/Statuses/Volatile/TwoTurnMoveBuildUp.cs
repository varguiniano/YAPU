using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status that stores a build up for a two turn move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/TwoTurnMoveBuildUp",
                     fileName = "TwoTurnMoveBuildUp")]
    public class TwoTurnMoveBuildUp : VolatileStatus
    {
        /// <summary>
        /// Reference to the move to buildup.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTwoTurnMoves))]
        #endif
        [SerializeField]
        private TwoTurnMove Move;

        /// <summary>
        /// Dictionary of the battlers that are building up and who are targeting.
        /// </summary>
        private readonly Dictionary<Battler, List<(BattlerType, int)>> buildersAndTargets = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            buildersAndTargets[battler] = (List<(BattlerType, int)>) extraData[0];

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            buildersAndTargets.Remove(battler);

            yield break; // We don't want to call base so there is no message.
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            usableMoves.Where(slot => slot.Move == Move).ToList();

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            buildersAndTargets.Remove(battler);

            return base.OnBattleEnded(battler);
        }

        /// <summary>
        /// Force the battler to use the second part of the move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battler manager.</param>
        /// <param name="battleAction">Generated battle action.</param>
        public override bool RequestForcedAction(Battler battler,
                                                 BattleManager battleManager,
                                                 out BattleAction battleAction)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            battleAction = new BattleAction
                           {
                               BattlerType = type,
                               Index = index,
                               ActionType = BattleAction.Type.Move
                           };

            bool moveInPool = battler.CurrentMoves.Any(slot => slot.Move == Move);

            List<int> parameters = new();
            List<object> additionalParameters = new();

            if (moveInPool)
                parameters.Add(battler.CurrentMoves.IndexOf(battler.CurrentMoves.First(slot =>
                                                                slot.Move == Move)));
            else
            {
                parameters.Add(5); // Index 5 for the move that is not in the pool.
                additionalParameters.Add(Move);
            }

            foreach ((BattlerType targetType, int targetIndex) in buildersAndTargets[battler])
            {
                parameters.Add((int) targetType);
                parameters.Add(targetIndex);
            }

            battleAction.Parameters = parameters.ToArray();
            battleAction.AdditionalParameters = additionalParameters.ToArray();

            return true;
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler,
                                                      Move move,
                                                      BattleManager battleManager,
                                                      List<(BattlerType Type, int Index)> targets,
                                                      Action<bool> finished)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            bool ignoresAbilities = battler.IgnoresOtherAbilities(battleManager, move);

            yield return battleManager.Moves.PerformMoveAfterCallbacks(type,
                                                                       index,
                                                                       battler,
                                                                       buildersAndTargets[battler],
                                                                       move,
                                                                       ignoresAbilities,
                                                                       true,
                                                                       true);

            // It will be reduced this turn too, so fix that.
            int slotIndex = battler.GetMoveIndex(move);
            MoveSlot slot = battler.CurrentMoves[slotIndex];
            slot.CurrentPP++;
            battler.CurrentMoves[slotIndex] = slot;

            finished.Invoke(false);
        }
    }
}