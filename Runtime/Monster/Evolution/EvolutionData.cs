using System;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Base class that stores the evolution of a monster.
    /// Inheritors implement the actual evolution logic.
    /// </summary>
    [Serializable]
    public abstract class EvolutionData : MonsterDatabaseData<EvolutionData>
    {
        /// <summary>
        /// Checks if the monster can evolve after level up.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public abstract bool CanEvolveAfterLevelUp(MonsterInstance monster,
                                                   DayMoment currentTime,
                                                   PlayerCharacter playerCharacter,
                                                   out bool consumeHeldItem);

        /// <summary>
        /// Check if the monster can evolve after a battle due to some extra data values.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public abstract bool CanEvolveAfterBattleThroughExtraData(MonsterInstance monster,
                                                                  DayMoment currentTime,
                                                                  out bool consumeHeldItem);

        /// <summary>
        /// Checks if the monster can evolve when using an item on it.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="item">Item being used on the monster.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public abstract bool CanEvolveWhenUsingItem(MonsterInstance monster,
                                                    DayMoment currentTime,
                                                    Item item,
                                                    PlayerCharacter playerCharacter,
                                                    out bool consumeHeldItem);

        /// <summary>
        /// Checks if the monster can evolve when trading it with another.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="otherMonster">Monster it's being traded with.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public abstract bool CanEvolveWhenTrading(MonsterInstance monster,
                                                  DayMoment currentTime,
                                                  MonsterInstance otherMonster,
                                                  PlayerCharacter playerCharacter,
                                                  out bool consumeHeldItem);

        /// <summary>
        /// Get the species and form of the monster to evolve to.
        /// </summary>
        /// <param name="monster">Current monster.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <returns>A tuple with the species and the form to evolve to.</returns>
        public abstract (MonsterEntry, Form) GetTargetEvolution(MonsterInstance monster, DayMoment currentTime);

        /// <summary>
        /// Called after the evolution has been triggered.
        /// </summary>
        /// <param name="evolvedMonster">Monster that recently evolved.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="database">Reference to the YAPU database.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public virtual void AfterEvolutionCallback(MonsterInstance evolvedMonster,
                                                   PlayerCharacter playerCharacter,
                                                   YAPUSettings settings,
                                                   MonsterDatabaseInstance database,
                                                   ILocalizer localizer)
        {
        }

        /// <summary>
        /// Get the relationships this evolution data can have to be displayed on the dex.
        /// </summary>
        /// <param name="entry">Monster entry being displayed.</param>
        /// <param name="formEntry">Form entry being displayed.</param>
        /// <param name="gender">Gender being displayed.</param>
        /// <param name="localizer">Reference to the localizer to use.</param>
        /// <returns>A list of the relationships generated.</returns>
        public abstract List<DexMonsterRelationshipData> GetDexRelationships(MonsterDexEntry entry,
                                                                             FormDexEntry formEntry,
                                                                             MonsterGender gender,
                                                                             ILocalizer localizer);

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public abstract EvolutionData Clone();
    }
}