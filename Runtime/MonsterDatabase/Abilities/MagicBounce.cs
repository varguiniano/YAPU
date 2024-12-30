using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the MagicBounce ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MagicBounce", fileName = "MagicBounce")]
    public class MagicBounce : Ability
    {
        /// <summary>
        /// Reflect the same way as Magic Coat.
        /// </summary>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, List<(BattlerType Type, int Index)>>
                                                                      finished)
        {
            // Only affects single target moves affected by magic coat.
            if (move.CanHaveMultipleTargets || !move.AffectedByMagicCoat || hasBeenReflected)
            {
                finished.Invoke(true, targets);
                yield break;
            }

            (BattlerType Type, int Index) ownerData = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

            // Only trigger if the target was the owner.
            if (targets[0] != ownerData)
            {
                finished.Invoke(true, targets);
                yield break;
            }

            ShowAbilityNotification(owner);

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

            finished.Invoke(false, targets);
        }
    }
}