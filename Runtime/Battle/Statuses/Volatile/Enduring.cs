using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the enduring volatile status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Enduring", fileName = "Enduring")]
    public class Enduring : VolatileStatus
    {
        /// <summary>
        /// No dialog.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

        /// <summary>
        /// No dialog.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Called when the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finished">Callback stating the multiplier for the effectiveness and if it will force survive.</param>
        public override IEnumerator OnHitByMove(DamageMove move,
                                                float effectiveness,
                                                Battler battler,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                bool ignoresAbilities,
                                                Action<float, bool> finished)
        {
            yield return DialogManager.ShowDialogAndWait("Battle/Endured",
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            finished.Invoke(1f, true);
        }
    }
}