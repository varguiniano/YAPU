using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the side status of Quick Guard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/QuickGuard", fileName = "QuickGuardStatus")]
    public class QuickGuardStatus : SideStatus
    {
        /// <summary>
        /// Play an animation when this status ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        public override IEnumerator EndAnimation(BattleManager battleManager, BattlerType side, string sideOwner)
        {
            // No dialog.
            yield break;
        }

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowMoveMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowMoveMessageNormally,
                                                         Action<bool> callback)
        {
            if (move.AffectedByProtect && move.GetPriority(user, new List<Battler> {target}, battleManager, false) > 0)
            {
                if (!didShowMoveMessageNormally)
                    DialogManager.ShowDialog("Battle/Move/Used",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            user.GetNameOrNickName(battleManager.Localizer),
                                                            battleManager.Localizer[move.LocalizableName]
                                                        },
                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Battle/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: target
                                                                .GetNameOrNickName(battleManager.Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                callback.Invoke(false);
            }
            else
                callback.Invoke(true);
        }
    }
}