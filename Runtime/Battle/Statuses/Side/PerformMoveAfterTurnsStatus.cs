using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Base class for a side status that performs a move on remove, like Future Sight.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/PerformMoveAfterTurns",
                     fileName = "PerformMoveAfterTurnsStatus")]
    public class PerformMoveAfterTurnsStatus : SideStatus
    {
        /// <summary>
        /// Move to use.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move Move;

        /// <summary>
        /// Turns until hitting.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int TurnsUntilHit = 3;

        /// <summary>
        /// Dialog to show when the move is used.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        private string MoveUsedKey = "Moves/FutureSight/Effect";

        /// <summary>
        /// Dictionary containing the positions that are being targeted, the user of the move and the turns left to hit.
        /// </summary>
        private Dictionary<(BattlerType battlerType, int battlerIndex), (Battler, int)> moves = new();

        /// <summary>
        /// Play an animation when this status starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="sideOwner">Used for dialogs.</param>
        /// <param name="extraData">Extra data provided when adding the status.</param>
        public override IEnumerator StartAnimation(BattleManager battleManager,
                                                   BattlerType side,
                                                   string sideOwner,
                                                   params object[] extraData)
        {
            // No message.

            (BattlerType, int) targetData = (side, (int)extraData[0]);
            Battler user = (Battler)extraData[1];

            moves[targetData] = (user, TurnsUntilHit);

            yield break;
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, BattlerType side)
        {
            yield return base.OnTickStatus(battleManager, side);

            Dictionary<(BattlerType battlerType, int battlerIndex), (Battler, int)> remainingMoves = new();

            foreach (KeyValuePair<(BattlerType, int), (Battler, int)> moveData in moves)
            {
                (BattlerType battlerType, int battlerIndex) battlerData = moveData.Key;
                (Battler user, int turns) = moveData.Value;

                turns--;

                if (turns > 0)
                    remainingMoves[battlerData] = (user, turns);
                else
                    yield return TriggerMove(battlerData.battlerType, battlerData.battlerIndex, user, battleManager);
            }

            moves = remainingMoves;

            CheckRemainingMoves(battleManager);
        }

        /// <summary>
        /// Play an animation when this status ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        public override IEnumerator EndAnimation(BattleManager battleManager, BattlerType side, string sideOwner)
        {
            // No dialog.

            Dictionary<(BattlerType battlerType, int battlerIndex), (Battler, int)> remainingMoves = new();

            // Remove the wishes from a side if the status on that side is removed.
            foreach (((BattlerType battlerType, int battlerIndex), (Battler, int) moveData) in remainingMoves)
                if (battlerType != side)
                    remainingMoves[(battlerType, battlerIndex)] = moveData;

            moves = remainingMoves;

            yield break;
        }

        /// <summary>
        /// Trigger a wish effect and heal..
        /// </summary>
        private IEnumerator TriggerMove(BattlerType targetType,
                                        int targetIndex,
                                        Battler user,
                                        BattleManager battleManager)
        {
            // Nobody on that slot.
            if (!battleManager.Battlers.IsBattlerFighting(targetType, targetIndex)) yield break;

            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            yield return DialogManager.ShowDialogAndWait(MoveUsedKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: target.GetNameOrNickName(battleManager.Localizer));

            (BattlerType type, int _, int _) = battleManager.Battlers.GetTypeAndRosterIndexOfBattler(user);

            yield return battleManager.Moves.ForcePerformMove(type,
                                                              0,
                                                              new List<(BattlerType Type, int Index)>
                                                              {
                                                                  (targetType, targetIndex)
                                                              },
                                                              Move,
                                                              user);
        }

        /// <summary>
        /// Check the remaining wishes and remove unnecessary ones.
        /// </summary>
        private void CheckRemainingMoves(BattleManager battleManager)
        {
            if (moves.Count(slot => slot.Key.battlerType == BattlerType.Ally) == 0)
                battleManager.Statuses.ScheduleRemoveStatus(this, BattlerType.Ally);

            if (moves.Count(slot => slot.Key.battlerType == BattlerType.Enemy) == 0)
                battleManager.Statuses.ScheduleRemoveStatus(this, BattlerType.Enemy);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="side">Side the status is setup in.</param>
        public override IEnumerator OnBattleEnded(BattlerType side)
        {
            yield return base.OnBattleEnded(side);

            moves.Clear();
        }
    }
}