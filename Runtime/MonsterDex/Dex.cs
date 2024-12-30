using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Serialization;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.MonsterDex
{
    /// <summary>
    /// Scriptable object that contains all the information about monsters the player has seen or caught.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dex/Dex", fileName = "Dex")]
    public class Dex : SavableObject, IPlayerDataReceiver
    {
        /// <summary>
        /// Entries in the dex.
        /// </summary>
        [SerializeField]
        private List<MonsterDexEntry> Entries;

        /// <summary>
        /// Lookup table for easily accessing entries.
        /// </summary>
        private Dictionary<MonsterEntry, MonsterDexEntry> EntryLookupTable
        {
            get
            {
                if (entryLookupTable != null) return entryLookupTable;

                entryLookupTable = new Dictionary<MonsterEntry, MonsterDexEntry>();

                foreach (MonsterDexEntry entry in Entries) entryLookupTable[entry.Species] = entry;

                return entryLookupTable;
            }
        }

        /// <summary>
        /// Backfield for EntryLookupTable.
        /// </summary>
        private Dictionary<MonsterEntry, MonsterDexEntry> entryLookupTable;

        /// <summary>
        /// Get a monster entry by its species.
        /// </summary>
        /// <param name="species">Species to get.</param>
        /// <param name="callback">Callback returning the entry.</param>
        public IEnumerator GetEntry(MonsterEntry species, Action<MonsterDexEntry> callback)
        {
            if (!EntryLookupTable.ContainsKey(species))
            {
                Logger.Warn(species.name + " is missing from the dex, attempting to repopulate the entries.");

                yield return PopulateEntries();
            }

            callback.Invoke(EntryLookupTable[species]);
        }

        /// <summary>
        /// Faster than GetEntry when you need a lot of entries
        /// since you don't need to call a yield return (which skips a frame) for each entry.
        /// </summary>
        /// <param name="speciesList">Species to get the entries of.</param>
        /// <param name="callback">Callback with all the entries.</param>
        public IEnumerator GetEntries(List<MonsterEntry> speciesList, Action<List<MonsterDexEntry>> callback)
        {
            List<MonsterDexEntry> entries = new();

            foreach (MonsterEntry species in speciesList)
            {
                if (!EntryLookupTable.ContainsKey(species))
                {
                    Logger.Warn(species.name + " is missing from the dex, attempting to repopulate the entries.");

                    yield return PopulateEntries();
                }

                entries.Add(EntryLookupTable[species]);
            }

            callback.Invoke(entries);
        }

        /// <summary>
        /// Number of monsters seen in at least one form.
        /// </summary>
        public uint NumberSeenInAtLeastOneForm => (uint) Entries.Count(entry => entry.HasMonsterBeenSeen);

        /// <summary>
        /// Number of monsters caught in at least one form.
        /// </summary>
        public uint NumberCaughtInAtLeastOneForm => (uint) Entries.Count(entry => entry.HasMonsterBeenCaught);

        /// <summary>
        /// Get the number of monsters caught, including forms and gender differences.
        /// </summary>
        public uint NumberCaughtIncludingFormsAndGenders => (uint) Entries.Sum(entry => entry.NumberOfFormsCaught);

        /// <summary>
        /// Get the total number of monsters.
        /// </summary>
        public uint NumberOfMonsters => (uint) Entries.Count;

        /// <summary>
        /// Get the total number of forms including gender differences.
        /// </summary>
        public uint NumberOfFormsIncludingGenderDifferences =>
            (uint) Entries.Sum(entry => entry.NumberOfFormsIncludingGenderDifferences);

        /// <summary>
        /// Register a monster as seen.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="showNotifications">Show the notifications when seen?</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        public void RegisterAsSeen(MonsterInstance monster,
                                   bool showNotifications,
                                   bool alreadyOwned) =>
            CoroutineRunner.RunRoutine(RegisterAsSeenRoutine(monster, showNotifications, alreadyOwned));

        /// <summary>
        /// Register a monster as seen.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="showNotifications">Show the notifications when seen?</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        private IEnumerator RegisterAsSeenRoutine(MonsterInstance monster,
                                                  bool showNotifications,
                                                  bool alreadyOwned)
        {
            MonsterDexEntry monsterDexEntry = null;

            yield return GetEntry(monster.Species,
                                  entry =>
                                  {
                                      monsterDexEntry = entry;
                                  });

            RegisterAsSeen(monster, monsterDexEntry, showNotifications, alreadyOwned);
        }

        /// <summary>
        /// Register a monster as seen.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="monsterDexEntry">Its entry in the dex.</param>
        /// <param name="showNotifications">Show the notifications when seen?</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        private void RegisterAsSeen(MonsterInstance monster,
                                    MonsterDexEntry monsterDexEntry,
                                    bool showNotifications,
                                    bool alreadyOwned)
        {
            FormDexEntry formDexEntry =
                monsterDexEntry.GetEntryForForm(monster.Form);

            if (showNotifications && globalGameData.HasDex)
            {
                // Had monster been previously seen?
                if (!monsterDexEntry.HasMonsterBeenSeen)
                    DialogManager.Notifications
                                 .QueueIconTextNotification(monster.GetIcon(),
                                                            "Dex/NewMonsterSeen",
                                                            localizableModifiers: false,
                                                            modifiers: monster
                                                               .GetNameOrNickName(localizer));

                // Had form been previously seen?
                else if (!formDexEntry.HasFormBeenSeen)
                    DialogManager.Notifications
                                 .QueueIconTextNotification(monster.GetIcon(),
                                                            "Dex/NewFormSeen",
                                                            localizableModifiers: false,
                                                            modifiers: monster
                                                               .GetNameOrNickName(localizer));

                // Is this a previously unseen gender variation?
                else if (!formDexEntry.GendersSeen[monster.PhysicalData.Gender]
                      && monster.FormData.HasMaleMaterialOverride)
                    DialogManager.Notifications
                                 .QueueIconTextNotification(monster.GetIcon(),
                                                            "Dex/NewGenderSeen",
                                                            localizableModifiers: false,
                                                            modifiers: monster
                                                               .GetNameOrNickName(localizer));
            }

            formDexEntry.GendersSeen[monster.PhysicalData.Gender] = true;
            if (!alreadyOwned) formDexEntry.TotalSeen++;
        }

        /// <summary>
        /// Register a monster as caught.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        /// <param name="alreadySeen">Had the player already seen this mon?</param>
        /// <param name="wasHatched">Was the mon hatched instead of caught?</param>
        public void RegisterAsCaught(MonsterInstance monster, bool alreadyOwned, bool alreadySeen, bool wasHatched) =>
            CoroutineRunner.RunRoutine(RegisterAsCaughtRoutine(monster, alreadyOwned, alreadySeen, wasHatched));

        /// <summary>
        /// Register a monster as caught.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        /// <param name="alreadySeen">Had the player already seen this mon?</param>
        /// <param name="wasHatched">Was the mon hatched instead of caught?</param>
        private IEnumerator RegisterAsCaughtRoutine(MonsterInstance monster,
                                                    bool alreadyOwned,
                                                    bool alreadySeen,
                                                    bool wasHatched)
        {
            if (!alreadySeen) yield return RegisterAsSeenRoutine(monster, false, alreadyOwned);

            MonsterDexEntry monsterDexEntry = null;

            yield return GetEntry(monster.Species,
                                  entry =>
                                  {
                                      monsterDexEntry = entry;
                                  });

            RegisterAsCaught(monster, monsterDexEntry, alreadyOwned, wasHatched);
        }

        /// <summary>
        /// Register a monster as caught.
        /// </summary>
        /// <param name="monster">Monster to register.</param>
        /// <param name="monsterDexEntry">Its dex entry.</param>
        /// <param name="alreadyOwned">Did the player already own this mon?</param>
        /// <param name="wasHatched">Was the mon hatched instead of caught?</param>
        private void RegisterAsCaught(MonsterInstance monster,
                                      MonsterDexEntry monsterDexEntry,
                                      bool alreadyOwned,
                                      bool wasHatched)
        {
            try
            {
                FormDexEntry formDexEntry = monsterDexEntry.GetEntryForForm(monster.Form);

                if (globalGameData.HasDex)
                {
                    // Had monster been previously seen?
                    if (!monsterDexEntry.HasMonsterBeenCaught)
                        DialogManager.Notifications.QueueIconTextNotification(monster.GetIcon(),
                                                                              "Dex/NewMonsterCaught",
                                                                              localizableModifiers: false,
                                                                              modifiers: monster
                                                                                 .GetNameOrNickName(localizer));

                    // Had form been previously seen?
                    else if (!formDexEntry.HasFormBeenCaught)
                        DialogManager.Notifications.QueueIconTextNotification(monster.GetIcon(),
                                                                              "Dex/NewFormCaught",
                                                                              localizableModifiers: false,
                                                                              modifiers: monster
                                                                                 .GetNameOrNickName(localizer));

                    // Is this a previously unseen gender variation?
                    else if (!formDexEntry.GendersCaught[monster.PhysicalData.Gender]
                          && monster.FormData.HasMaleMaterialOverride)
                        DialogManager.Notifications.QueueIconTextNotification(monster.GetIcon(),
                                                                              "Dex/NewGenderCaught",
                                                                              localizableModifiers: false,
                                                                              modifiers: monster
                                                                                 .GetNameOrNickName(localizer));
                }

                formDexEntry.GendersCaught[monster.PhysicalData.Gender] = true;

                if (alreadyOwned) return;

                if (wasHatched)
                    formDexEntry.TotalHatched++;
                else
                    formDexEntry.TotalCaught++;
            }
            catch (Exception exception)
            {
                Logger.Fatal("Exception raised when trying to register "
                           + monster.Species
                           + "-"
                           + monster.Form
                           + "-"
                           + monster.PhysicalData.Gender
                           + " as caught.",
                             exception);
            }
        }

        /// <summary>
        /// Invalidate the entry lookup table.
        /// </summary>
        private void InvalidateLookupTable() => entryLookupTable = null;

        /// <summary>
        /// Get all the entries in the dex.
        /// </summary>
        public List<MonsterDexEntry> GetAllEntries() => Entries;

        /// <summary>
        /// Override the entries in the database with a new entry list.
        /// </summary>
        /// <param name="entries">New entries.</param>
        internal void OverrideEntries(List<MonsterDexEntry> entries) => Entries = entries;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance monsterDatabase;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Save the dex to a serialized text.
        /// </summary>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableDex(this));

        /// <summary>
        /// Load the dex from a serialized text.
        /// </summary>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance database)
        {
            serializer.From<SavableDex>(data).LoadDex(this, database);

            yield return PopulateEntries();
        }

        /// <summary>
        /// Reset the dex to its default values.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            Entries.Clear();

            yield return PopulateEntries(true);
        }

        /// <summary>
        /// Go over all entries in the database and populate the dex entries that are missing.
        /// </summary>
        private IEnumerator PopulateEntries(bool invalidateLookupTableBeforeFill = false)

        {
            WaitForEndOfFrame waitAFrame = new();

            // This allows refilling after starting a new game, where the lookup table would be filled with the old entries.
            if (invalidateLookupTableBeforeFill) InvalidateLookupTable();

            List<MonsterEntry> list = monsterDatabase.GetMonsterEntries();

            for (int i = 0; i < list.Count; i++)
            {
                MonsterEntry species = list[i];

                if (EntryLookupTable != null && !EntryLookupTable.ContainsKey(species))
                {
                    Logger.Info("Dex is missing entry for " + species.name + ", adding it.");
                    Entries.Add(new MonsterDexEntry(species));
                }
                else if (EntryLookupTable != null
                      && species.AvailableFormsWithShinnies.Count > EntryLookupTable[species].FormEntries.Count)
                {
                    Logger.Info("Dex entry for " + species.name + " has missing forms, repopulating.");
                    EntryLookupTable[species].PopulateForms();
                }

                if (i % 10 == 0) yield return waitAFrame;
            }

            Entries = Entries.OrderBy(entry => entry.Species.DexNumber).ToList();

            InvalidateLookupTable();
        }
    }

    /// <summary>
    /// Data class that contains all the information about monsters the player has seen or caught.
    /// This class can be saved with the savegame.
    /// </summary>
    [Serializable]
    public class SavableDex
    {
        /// <summary>
        /// Entries in the dex.
        /// </summary>
        public List<SavableMonsterDexEntry> Entries;

        /// <summary>
        /// Build from a Dex object.
        /// </summary>
        /// <param name="dex">Dex to build from.</param>
        public SavableDex(Dex dex)
        {
            Entries = new List<SavableMonsterDexEntry>();

            foreach (MonsterDexEntry entry in dex.GetAllEntries()) Entries.Add(new SavableMonsterDexEntry(entry));
        }

        /// <summary>
        /// Load back into a dex object/
        /// </summary>
        /// <param name="dex">Object to load into.</param>
        /// <param name="database">Reference to the monster database.</param>
        public void LoadDex(Dex dex, MonsterDatabaseInstance database)
        {
            List<MonsterDexEntry> entries = Entries.Select(entry => entry.ToMonsterDexEntry(database)).ToList();

            dex.OverrideEntries(entries);
        }
    }
}