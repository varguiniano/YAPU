using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an effect of an item that can be used on a target.
    /// </summary>
    public abstract class UseOnTargetItemEffect : MonsterDatabaseScriptable<UseOnTargetItemEffect>
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
        public virtual bool IsCompatible(YAPUSettings settings,
                                         TimeManager timeManager,
                                         MonsterInstance monsterInstance,
                                         Item item,
                                         PlayerCharacter playerCharacter) =>
            true;

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager">Reference to the evolution manager.</param>
        /// <param name="inputManager"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public virtual IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
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
            yield break;
        }

        /// <summary>
        /// Check if the effect can be used on a monster in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="item"></param>
        /// <returns>True if it can be used.</returns>
        public virtual bool IsCompatible(BattleManager battleManager, Battler battler, Item item) => true;

        /// <summary>
        /// Use on a battler.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public virtual IEnumerator UseOnBattler(Item item,
                                                Battler battler,
                                                BattleManager battleManager,
                                                YAPUSettings settings,
                                                ExperienceLookupTable experienceLookupTable,
                                                ILocalizer localizer,
                                                Action<bool> finished,
                                                bool wasFlung = false)
        {
            yield break;
        }
    }
}