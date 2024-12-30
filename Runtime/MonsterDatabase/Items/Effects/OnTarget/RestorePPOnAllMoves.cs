using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an item effect that restores PPs to all moves of a monster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RestorePPOnAllMoves", fileName = "RestorePPOnAllMoves")]
    public class RestorePPOnAllMoves : UseOnTargetItemEffect
    {
        /// <summary>
        /// Number of PP to restore.
        /// </summary>
        [SerializeField]
        private byte PPToRestore;

        /// <summary>
        /// It will be compatible if any move has less PP than the max.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item"></param>
        /// <param name="playerCharacter"></param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            monsterInstance.CurrentMoves.Any(IsSlotCompatible) && !monsterInstance.EggData.IsEgg;

        /// <summary>
        /// Apply the friendship changing effect.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager"></param>
        /// <param name="inputManager"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                         Item item,
                                                         YAPUSettings settings,
                                                         ExperienceLookupTable experienceLookupTable,
                                                         PlayerCharacter playerCharacter,
                                                         TimeManager timeManager,
                                                         EvolutionManager evolutionManager,
                                                         IInputManager inputManager,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            if (!IsCompatible(settings, timeManager, monsterInstance, item, playerCharacter))
            {
                Logger.Error("Restoring "
                           + PPToRestore
                           + " PP to  monster "
                           + monsterInstance.GetNameOrNickName(localizer)
                           + " is not possible.");

                finished?.Invoke(false);

                yield break;
            }

            for (int i = 0; i < monsterInstance.CurrentMoves.Length; i++)
            {
                MoveSlot slot = monsterInstance.CurrentMoves[i];

                if (slot.Move == null) continue;

                Logger.Info("Restoring "
                          + PPToRestore
                          + " PP to move "
                          + localizer[slot.Move.LocalizableName]
                          + " of monster "
                          + monsterInstance.GetNameOrNickName(localizer)
                          + ".");

                slot.CurrentPP = (byte) Mathf.Min(slot.CurrentPP + PPToRestore, slot.MaxPP);

                monsterInstance.CurrentMoves[i] = slot;
            }

            yield return DialogManager.ShowDialogAndWait("Battle/RestorePPAllMoves",
                                                         localizableModifiers: false,
                                                         modifiers: monsterInstance.GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f);

            finished?.Invoke(true);
        }

        /// <summary>
        /// It will be compatible if any move has less PP than the max.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="item"></param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item) =>
            IsCompatible(battleManager.YAPUSettings,
                         battleManager.TimeManager,
                         battler,
                         item,
                         battleManager.PlayerCharacter);

        /// <summary>
        /// Apply the friendship changing effect.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public override IEnumerator UseOnBattler(Item item,
                                                 Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer,
                                                 Action<bool> finished,
                                                 bool wasFlung = false)
        {
            yield return UseOnMonsterInstance(battler,
                                              item,
                                              settings,
                                              experienceLookupTable,
                                              battleManager.PlayerCharacter,
                                              battleManager.TimeManager,
                                              null,
                                              null,
                                              localizer,
                                              finished);
        }

        /// <summary>
        /// Check if a move slot is compatible.
        /// It will be compatible if the move has less PP than the max.
        /// </summary>
        /// <param name="slot">Slot to check.</param>
        /// <returns>True if compatible.</returns>
        private static bool IsSlotCompatible(MoveSlot slot) => slot.Move != null && slot.CurrentPP < slot.MaxPP;
    }
}