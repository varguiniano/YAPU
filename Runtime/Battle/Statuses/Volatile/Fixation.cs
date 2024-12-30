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
    /// Volatile status that stores a fixation for using a specific move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Fixation", fileName = "Fixation")]
    public class Fixation : VolatileStatus
    {
        /// <summary>
        /// Reference to the move to buildup.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllFixationMoves))]
        #endif
        [SerializeField]
        private FixationMove Move;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break; // We don't want to call base so there is no message.
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
        /// Force the battler to use the second part of the move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battler manager.</param>
        /// <param name="battleAction">Generated battle action.</param>
        public override bool RequestForcedAction(Battler battler,
                                                 BattleManager battleManager,
                                                 out BattleAction battleAction)
        {
            Logger.Info("Fixation forcing to use move " + Move.name + ".");

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            battleAction = new BattleAction
                           {
                               BattlerType = type,
                               Index = index,
                               ActionType = BattleAction.Type.Move
                           };

            int targetType = (int) type;
            int targetIndex = index;

            // If it normally targets other than itself, keep targeting the same.
            if (Move.MovePossibleTargets != MonsterDatabase.Moves.Move.PossibleTargets.Self
             && Move.LastTargets.ContainsKey(battler))
            {
                targetType = (int) Move.LastTargets[battler].Type;
                targetIndex = Move.LastTargets[battler].Index;
            }

            List<int> parameters = new()
                                   {
                                       battler.CurrentMoves.IndexOf(battler.CurrentMoves.First(slot =>
                                                                        slot.Move == Move)),
                                       targetType,
                                       targetIndex
                                   };

            battleAction.Parameters = parameters.ToArray();

            return true;
        }

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// Remove the status if the move failed.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="user">User of the action.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            yield return base.AfterAction(owner, action, user, battleManager);

            if (owner == user && !user.LastPerformedAction.LastMoveSuccessful)
                battleManager.Statuses.ScheduleRemoveStatus(this, owner);
        }
    }
}