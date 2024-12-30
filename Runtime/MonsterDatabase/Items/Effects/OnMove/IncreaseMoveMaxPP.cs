using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnMove
{
    /// <summary>
    /// Item effect to restore PP to a target move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTargetMove/IncreaseMoveMaxPP", fileName = "IncreaseMoveMaxPP")]
    public class IncreaseMoveMaxPP : UseOnTargetMoveItemEffect
    {
        /// <summary>
        /// Percentage to increase.
        /// </summary>
        [SerializeField]
        private float PercentageToIncrease = 1f / 5;

        /// <summary>
        /// It will be compatible if any move has less PP than the max.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(MonsterInstance monsterInstance) =>
            monsterInstance.CurrentMoves.Any(IsSlotCompatible);

        /// <summary>
        /// It will be compatible if the move has less PP than the max.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="index">Move slot to check</param>
        /// <returns>True if compatible.</returns>
        public override bool IsMoveCompatible(MonsterInstance monsterInstance, int index) =>
            IsSlotCompatible(monsterInstance.CurrentMoves[index]);

        /// <summary>
        /// Restore the PP to the move.
        /// </summary>
        /// <param name="monsterInstance">Monster to restore.</param>
        /// <param name="index">Move to restore.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finish callback returning if the object should be consumed.</param>
        public override IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                         int index,
                                                         PlayerCharacter playerCharacter,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            if (!IsMoveCompatible(monsterInstance, index))
            {
                Logger.Error("Adding more PP to  monster "
                           + monsterInstance.GetNameOrNickName(localizer)
                           + "'s move "
                           + index
                           + " is not possible.");

                finished?.Invoke(false);

                yield break;
            }

            MoveSlot slot = monsterInstance.CurrentMoves[index];

            byte addition = (byte)Mathf.FloorToInt(slot.Move.BasePowerPoints * PercentageToIncrease);

            Logger.Info("Adding " + addition + " PP to " + localizer[slot.Move.LocalizableName]);

            slot.MaxPP += addition;

            monsterInstance.CurrentMoves[index] = slot;

            yield return DialogManager.ShowDialogAndWait("Items/IncreasePP",
                                                         modifiers: slot.Move.LocalizableName);

            finished?.Invoke(true); // Should consume.
        }

        /// <summary>
        /// It will be compatible if any move has less PP than the max.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler) => IsCompatible(battler);

        /// <summary>
        /// It will be compatible if the move has less PP than the max.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="index">Move slot to check</param>
        /// <returns>True if compatible.</returns>
        public override bool IsMoveCompatible(BattleManager battleManager, Battler battler, int index) =>
            IsMoveCompatible(battler, index);

        /// <summary>
        /// Restore the PP to the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="index">Move to restore.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finish callback returning if the object should be consumed.</param>
        public override IEnumerator UseOnBattler(BattleManager battleManager,
                                                 Battler battler,
                                                 int index,
                                                 ILocalizer localizer,
                                                 Action<bool> finished)
        {
            yield return UseOnMonsterInstance(battler, index, battleManager.PlayerCharacter, localizer, finished);
        }

        /// <summary>
        /// Check if a move slot is compatible.
        /// It will be compatible if the move has less PP than the max.
        /// </summary>
        /// <param name="slot">Slot to check.</param>
        /// <returns>True if compatible.</returns>
        private static bool IsSlotCompatible(MoveSlot slot) =>
            slot.Move != null && slot.MaxPP < slot.Move.MaxPowerPoints;
    }
}