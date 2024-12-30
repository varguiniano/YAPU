using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for protecting the user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Protection", fileName = "Protection")]
    public class Protection : VolatileStatus
    {
        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            yield break; // No dialog.
        }

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowUsedMessageNormally,
                                                         Action<bool> callback)
        {
            if (move.AffectedByProtect)
            {
                if (!didShowUsedMessageNormally)
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