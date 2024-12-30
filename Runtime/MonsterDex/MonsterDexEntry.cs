using System;
using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDex
{
    /// <summary>
    /// Class representing an entry in the monster dex.
    /// </summary>
    [Serializable]
    public class MonsterDexEntry
    {
        /// <summary>
        /// Species this entry belongs to.
        /// </summary>
        public MonsterEntry Species;

        /// <summary>
        /// List of form entries this monster has.
        /// </summary>
        public List<FormDexEntry> FormEntries = new();

        /// <summary>
        /// Get the entry for a form.
        /// </summary>
        /// <param name="form">Form to get the entry from.</param>
        /// <returns>Form entry.</returns>
        public FormDexEntry GetEntryForForm(Form form) => FormEntries.FirstOrDefault(entry => entry.Form == form);

        /// <summary>
        /// Get the first known form of this monster.
        /// </summary>
        /// <returns>The first known form entry.</returns>
        public (FormDexEntry, MonsterGender) GetFirstKnownFormAndGender()
        {
            FormDexEntry form = FormEntries.FirstOrDefault(entry => entry.HasFormBeenSeen);

            return form == null
                       ? (null, MonsterGender.NonBinary)
                       : (form, form.GendersSeen.First(pair => pair.Value).Key);
        }

        /// <summary>
        /// Get all the forms seen.
        /// </summary>
        public List<FormDexEntry> GetAllFormsSeen() => FormEntries.Where(entry => entry.HasFormBeenSeen).ToList();

        /// <summary>
        /// Has this monster been seen in at least one form and gender?
        /// </summary>
        public bool HasMonsterBeenSeen => FormEntries.Any(entry => entry.HasFormBeenSeen);

        /// <summary>
        /// Has this monster been caught in at least one form and gender?
        /// </summary>
        public bool HasMonsterBeenCaught => FormEntries.Any(entry => entry.HasFormBeenCaught);

        /// <summary>
        /// Get the number of forms seen, including gender differences.
        /// </summary>
        public uint NumberOfFormsSeen =>
            FormEntries.Aggregate<FormDexEntry, uint>(0,
                                                      (current, entry) =>
                                                          current
                                                        + entry.GetNumberOfFormsIncludingGenderDifferencesSeen(this));

        /// <summary>
        /// Get the number of forms seen without including shinies and genders.
        /// </summary>
        public uint NumberOfFormsWithoutShiniesAndGendersSeen =>
            (uint) FormEntries.Count(entry => !entry.Form.IsShiny && entry.HasFormBeenSeen);

        /// <summary>
        /// Get the number of forms caught without including shinies and genders.
        /// </summary>
        public uint NumberOfFormsWithoutShiniesAndGendersCaught =>
            (uint) FormEntries.Count(entry => !entry.Form.IsShiny && entry.HasFormBeenCaught);

        /// <summary>
        /// Get the total number of forms without including shinies and genders.
        /// </summary>
        public uint NumberOfFormsWithoutShiniesAndGenders => (uint) FormEntries.Count(entry => !entry.Form.IsShiny);

        /// <summary>
        /// Get the number of shiny forms seen without including genders.
        /// </summary>
        public uint NumberOfShinyFormsWithoutGendersSeen =>
            (uint) FormEntries.Count(entry => entry.Form.IsShiny && entry.HasFormBeenSeen);

        /// <summary>
        /// Get the number of shiny forms caught without including genders.
        /// </summary>
        public uint NumberOfShinyFormsWithoutGendersCaught =>
            (uint) FormEntries.Count(entry => entry.Form.IsShiny && entry.HasFormBeenCaught);

        /// <summary>
        /// Get the total number of shiny forms without including genders.
        /// </summary>
        public uint NumberOfShinyFormsWithoutGenders => (uint) FormEntries.Count(entry => entry.Form.IsShiny);

        /// <summary>
        /// Get the number of gender variations seen.
        /// </summary>
        public uint NumberOfGenderVariationsSeen =>
            FormEntries.Where(entry => Species[entry.Form].HasGenderVariations)
                       .Aggregate<FormDexEntry, uint>(0,
                                                      (current, entry) =>
                                                          current
                                                        + entry
                                                             .GetNumberOfFormsIncludingGenderDifferencesSeen(this));

        /// <summary>
        /// Get the number of gender variations caught.
        /// </summary>
        public uint NumberOfGenderVariationsCaught =>
            FormEntries.Where(entry => Species[entry.Form].HasGenderVariations)
                       .Aggregate<FormDexEntry, uint>(0,
                                                      (current, entry) =>
                                                          current
                                                        + entry
                                                             .GetNumberOfFormsIncludingGenderDifferencesCaught(this));

        /// <summary>
        /// Get the total number of gender variations.
        /// </summary>
        public uint NumberOfGenderVariations =>
            (uint) FormEntries.Count(entry => Species[entry.Form].HasGenderVariations)
          * 2;

        /// <summary>
        /// Are all the forms and variations of this monster caught?
        /// </summary>
        public bool AllFormsAndVariationsCaught => NumberOfFormsCaught == NumberOfFormsIncludingGenderDifferences;

        /// <summary>
        /// Get the number of forms caught, including gender differences.
        /// </summary>
        public uint NumberOfFormsCaught =>
            FormEntries.Aggregate<FormDexEntry, uint>(0,
                                                      (current, entry) =>
                                                          current
                                                        + entry.GetNumberOfFormsIncludingGenderDifferencesCaught(this));

        /// <summary>
        /// Get the total number of forms including gender differences.
        /// </summary>
        public uint NumberOfFormsIncludingGenderDifferences =>
            FormEntries.Aggregate<FormDexEntry, uint>(0,
                                                      (current, entry) =>
                                                          current
                                                        + entry.GetNumberOfFormsIncludingGenderDifferences(this));

        /// <summary>
        /// Parameterless constructor to be used by the save system.
        /// </summary>
        public MonsterDexEntry()
        {
        }

        /// <summary>
        /// Constructor with species to be used when populating.
        /// </summary>
        /// <param name="species">Species to generate from.</param>
        public MonsterDexEntry(MonsterEntry species)
        {
            Species = species;

            PopulateForms();
        }

        /// <summary>
        /// Populate the entries for forms of this monster.
        /// </summary>
        public void PopulateForms()
        {
            foreach (Form form in Species.AvailableForms.Where(form => GetEntryForForm(form) == null))
            {
                FormEntries.Add(new FormDexEntry(Species, form));
                if (form.HasShinyVersion) FormEntries.Add(new FormDexEntry(Species, form.ShinyVersion));
            }
        }
    }

    /// <summary>
    /// Class representing an entry in the monster dex.
    /// This class can be saved to the savegame.
    /// </summary>
    [Serializable]
    public class SavableMonsterDexEntry
    {
        /// <summary>
        /// Species this entry belongs to.
        /// </summary>
        public int Species;

        /// <summary>
        /// List of form entries this monster has.
        /// </summary>
        public List<SavableFormDexEntry> FormEntries;

        /// <summary>
        /// Constructor using a monster dex entry.
        /// </summary>
        /// <param name="monsterDexEntry">Monster dex entry to use.</param>
        public SavableMonsterDexEntry(MonsterDexEntry monsterDexEntry)
        {
            Species = monsterDexEntry.Species.name.GetHashCode();

            FormEntries = new List<SavableFormDexEntry>();

            foreach (FormDexEntry formDexEntry in monsterDexEntry.FormEntries)
                FormEntries.Add(new SavableFormDexEntry(formDexEntry));
        }

        /// <summary>
        /// Convert to a monster dex entry.
        /// </summary>
        /// <param name="database">Reference to the monster database.</param>
        /// <returns>The generated monster dex entry.</returns>
        public MonsterDexEntry ToMonsterDexEntry(MonsterDatabaseInstance database)
        {
            MonsterDexEntry result = new()
                                     {
                                         Species = database.GetMonsterByHash(Species)
                                     };

            foreach (SavableFormDexEntry savableFormDexEntry in FormEntries)
                result.FormEntries.Add(savableFormDexEntry.ToFormDexEntry(database));

            return result;
        }
    }
}