using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the Grudge volatile status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/GrudgeStatus", fileName = "GrudgeStatus")]
    public class GrudgeStatus : VolatileStatus
    {
        /// <summary>
        /// No message.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data used by certain statuses.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

        /// <summary>
        /// No message.
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
        /// Called when this battler is knocked out by another battler
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="owner">Owner of the status</param>
        /// <param name="userType">User of the effect.</param>
        /// <param name="userIndex">User of the effect.</param>
        /// <param name="userMove">Move that knocked out, if any.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnKnockedOutByBattler(Battler owner,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Move userMove,
                                                          BattleManager battleManager)
        {
            if (userMove == null) yield break;

            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            int index = user.GetMoveIndex(userMove);

            if (index < 0) yield break;

            user.CurrentMoves[index].CurrentPP = 0;

            yield return DialogManager.ShowDialogAndWait("Moves/Grudge/Effect",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    });
        }
    }
}