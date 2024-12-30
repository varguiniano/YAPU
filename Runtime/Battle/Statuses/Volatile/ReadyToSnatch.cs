using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the volatile status of being ready to snatch a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/ReadyToSnatch", fileName = "ReadyToSnatch")]
    public class ReadyToSnatch : VolatileStatus
    {
        /// <summary>
        /// List of battlers that have already snatched this turn so that none snatches twice.
        /// </summary>
        private readonly List<Battler> alreadySnatchedThisTurn = new();

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// Steal the move and use it itself.
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
            if (!move.AffectedBySnatch || alreadySnatchedThisTurn.Contains(owner))
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            alreadySnatchedThisTurn.Add(owner);

            yield return DialogManager.ShowDialogAndWait("Moves/Snatch/Dialog",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        battleManager.Localizer[move.LocalizableName],
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    });

            (BattlerType newUserType, int newUserIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

            yield return MoveUtils.TryGenerateRandomTargetsForMove(battleManager,
                                                                   move,
                                                                   newUserType,
                                                                   newUserIndex,
                                                                   owner,
                                                                   Logger,
                                                                   newTargets => targets = newTargets);

            yield return battleManager.Moves.ForcePerformMove(newUserType, newUserIndex, targets, move);

            finished.Invoke(false, false, targets);
        }
    }
}