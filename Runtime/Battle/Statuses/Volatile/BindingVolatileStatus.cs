using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Class for volatile statuses that are affected by binding band.
    /// </summary>
    public class BindingVolatileStatus : VolatileStatus
    {
        /// <summary>
        /// Message to show when the battler can't switch.
        /// </summary>
        [FoldoutGroup("Binding")]
        [SerializeField]
        private string CantSwitchMessage;

        /// <summary>
        /// Items that can bypass this binding effect and allow to switch.
        /// </summary>
        [FoldoutGroup("Binding")]
        [SerializeField]
        private List<Item> SwitchBypassingItems;

        /// <summary>
        /// List of battlers that are affected by binding band.
        /// </summary>
        protected readonly List<Battler> AffectedByBindingBand = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need. 0 must be if affected by binding band.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return base.OnAddStatus(battleManager, battler, extraData);

            if ((bool) extraData[0]) AffectedByBindingBand.Add(battler);
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield return base.OnRemoveStatus(battleManager, battler, playAnimation);

            if (AffectedByBindingBand.Contains(battler)) AffectedByBindingBand.Remove(battler);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            yield return base.OnBattleEnded(battler);

            AffectedByBindingBand.Clear();
        }

        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanSwitch(Battler battler,
                                       BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       Move userMove,
                                       Item item,
                                       bool itemBelongsToUser,
                                       bool showMessages)
        {
            if (battler.CanUseHeldItemInBattle(battleManager) && SwitchBypassingItems.Contains(battler.HeldItem))
                return true;

            if (showMessages)
                DialogManager.ShowDialog(CantSwitchMessage,
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }
    }
}