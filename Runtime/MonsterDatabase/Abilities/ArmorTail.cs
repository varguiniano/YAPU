using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// ArmorTail ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ArmorTail", fileName = "ArmorTail")]
    public class ArmorTail : Ability
    {
        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has the same move been reflected?</param>
        /// <param name="finished">Callback stating if the move will still be used, the new targets for the move.</param>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, List<(BattlerType Type, int Index)>>
                                                                      finished)
        {
            (BattlerType ownerType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            (BattlerType userType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            if (ownerType == userType
             || targets.Count > 1
             || targets.All(pair => pair.Type != ownerType)
             || move.GetPriority(user,
                                 targets.Select(target => battleManager.Battlers.GetBattlerFromBattleIndex(target))
                                        .ToList(),
                                 battleManager,
                                 false)
             <= 0)
            {
                yield return base.OnOtherBattlerAboutToUseAMove(owner,
                                                                user,
                                                                move,
                                                                battleManager,
                                                                targets,
                                                                hasBeenReflected,
                                                                finished);

                yield break;
            }

            ShowAbilityNotification(owner);

            yield return DialogManager.ShowDialogAndWait("Abilities/ArmorTail/Effect",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetLocalizedName(battleManager.Localizer),
                                                                        move.GetLocalizedName(battleManager.Localizer),
                                                                        user.GetLocalizedName(battleManager.Localizer)
                                                                    });

            finished.Invoke(false, targets);
        }
    }
}