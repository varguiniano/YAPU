using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Status that reflects affected moves back to the user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/MagicCoatStatus",
                     fileName = "MagicCoatStatus")]
    public class MagicCoatStatus : VolatileStatus
    {
        /// <summary>
        /// No message.
        /// </summary>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

        /// <summary>
        /// No message.
        /// </summary>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Reflect if it can.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has this move been reflected?</param>
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
            // Only affects single target moves affected by magic coat.
            if (move.CanHaveMultipleTargets || !move.AffectedByMagicCoat || hasBeenReflected)
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            (BattlerType Type, int Index) ownerData = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

            // Only trigger if the target was the owner.
            if (targets[0] != ownerData)
            {
                finished.Invoke(true, false, targets);
                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Moves/MagicCoat/Dialog",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        battleManager.Localizer[move.LocalizableName],
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    });

            targets = new List<(BattlerType Type, int Index)> {battleManager.Battlers.GetTypeAndIndexOfBattler(user)};

            yield return battleManager.Moves.ForcePerformMove(ownerData.Type,
                                                              ownerData.Index,
                                                              targets,
                                                              move,
                                                              hasBeenReflected: true);

            finished.Invoke(false, false, targets);
        }
    }
}