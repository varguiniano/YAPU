using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Item effect to raise experience to a monster.
    /// </summary>
    public abstract class RaiseExperience : UseOnTargetItemEffect
    {
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
            monsterInstance.StatData.Level < 100 && !monsterInstance.EggData.IsEgg;

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
            if (!IsCompatible(settings, timeManager, monsterInstance, item, playerCharacter)) yield break;

            uint amount = GetRaiseAmount(monsterInstance, settings, experienceLookupTable);

            List<MonsterInstance.LevelUpData> levelUpData = new();

            monsterInstance.RaiseExperience(experienceLookupTable,
                                            amount,
                                            data =>
                                            {
                                                levelUpData.Add(data);
                                            });

            yield return
                DialogManager.ShowXPGainDialogLearnMoveAndEvolveForSingleMonsterInPlayerRoster(monsterInstance,
                    amount,
                    levelUpData,
                    playerCharacter,
                    localizer);

            finished.Invoke(true);
        }

        /// <summary>
        /// Get the amount by which to raise the experience.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <returns>The amount to raise.</returns>
        protected abstract uint GetRaiseAmount(MonsterInstance monsterInstance,
                                               YAPUSettings settings,
                                               ExperienceLookupTable experienceLookupTable);
    }
}