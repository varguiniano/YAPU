using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the StickyWebSideStatus.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/StickyWebSideStatus",
                     fileName = "StickyWebSideStatus")]
    public class StickyWebSideStatus : SideStatus
    {
        /// <summary>
        /// Changes to the stats to be applied to the battlers on etb.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<Stat, short> StatChanges;

        /// <summary>
        /// Callback for when a battler enters the battle on the side this status is in.
        /// </summary>
        /// <param name="side">Side of this status.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnBattlerEnteredSide(BattlerType side,
                                                         int battlerIndex,
                                                         BattleManager battleManager)
        {
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(side, battlerIndex);

            if (!battler.IsGrounded(battleManager, false)) yield break;
            
            yield return DialogManager.ShowDialogAndWait("Status/Side/StickyWebSideStatus/Effect",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer));

            foreach (KeyValuePair<Stat, short> statChange in StatChanges)
                yield return
                    battleManager.BattlerStats.ChangeStatStage(battler, statChange.Key, statChange.Value, battler);
        }
    }
}