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
    /// Data class for an implementation of the ability Damp.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Damp", fileName = "Damp")]
    public class Damp : Ability
    {
        /// <summary>
        /// Moves prevented by this ability.
        /// </summary>
        [SerializeField]
        private List<Move> PreventedMoves;

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
            if (!PreventedMoves.Contains(move))
            {
                finished(true, targets);
                yield break;
            }

            ShowAbilityNotification(owner);

            yield return DialogManager.ShowDialogAndWait("Abilities/Damp/Effect",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetNameOrNickName(battleManager.Localizer),
                                                                        move.GetLocalizedName(battleManager.Localizer),
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        battleManager.Localizer[LocalizableName]
                                                                    });

            finished(false, targets);
        }
    }
}