using System;
using System.Collections;
using Sirenix.OdinInspector;
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
    /// Raise dynamax level of the target monster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/RaiseDynamaxLevel", fileName = "RaiseDynamaxLevel")]
    public class RaiseDynamaxLevel : UseOnTargetItemEffect
    {
        /// <summary>
        /// Amount to raise.
        /// </summary>
        [SerializeField]
        [PropertyRange(1, 10)]
        private byte Amount = 1;

        /// <summary>
        /// Check if the effect can be used on a monster.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item"></param>
        /// <param name="playerCharacter"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            monsterInstance.MaxFormLevel < 10;

        /// <summary>
        /// Not compatible in battle.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="battler"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item) => false;

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item">Item owner of the effect.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
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
            if (!IsCompatible(settings, timeManager, monsterInstance, item, playerCharacter)) yield break;

            byte amountToRaise = Amount;

            if (monsterInstance.MaxFormLevel + amountToRaise > 10)
                amountToRaise = (byte)(10 - monsterInstance.MaxFormLevel);

            if (amountToRaise == 0) yield break;

            monsterInstance.MaxFormLevel += amountToRaise;

            yield return DialogManager.ShowDialogAndWait("Dialogs/MaxFormLevelGain",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monsterInstance.GetNameOrNickName(localizer),
                                                                        amountToRaise.ToString()
                                                                    });
        }
    }
}