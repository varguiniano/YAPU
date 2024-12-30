using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Mimic.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Mimic", fileName = "Mimic")]
    public class Mimic : Move
    {
        /// <summary>
        /// Moves immune to mimic.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Move> ImmuneMoves;

        /// <summary>
        /// Fail if the target hasn't used a move yet.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Move lastMove = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex)
                                         .LastPerformedAction.LastMove;

            return base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
                || lastMove == null
                || ImmuneMoves.Contains(lastMove);
        }

        /// <summary>
        /// Copy the target's last move.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            int mimicIndex = user.CurrentMoves.IndexOf(user.CurrentMoves.First(slot => slot.Move == this));

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                Move move = target.LastPerformedAction.LastMove;

                user.TemporaryReplaceMove(mimicIndex, move, true);

                yield return DialogManager.ShowDialogAndWait("Moves/Mimic/Effect",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            user.GetNameOrNickName(localizer),
                                                                            target.GetNameOrNickName(localizer),
                                                                            move.GetLocalizedName(localizer)
                                                                        });
            }
        }
    }
}