using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDex
{
    /// <summary>
    /// Class representing the entry of a form in the monster dex.
    /// This form entry belongs to a monster entry.
    /// </summary>
    [Serializable]
    public class FormDexEntry
    {
        /// <summary>
        /// Form this entry belongs to.
        /// </summary>
        public Form Form;

        /// <summary>
        /// Genders seen for this form.
        /// </summary>
        public SerializedDictionary<MonsterGender, bool> GendersSeen = new();

        /// <summary>
        /// Genders caught for this form.
        /// </summary>
        public SerializedDictionary<MonsterGender, bool> GendersCaught = new();

        /// <summary>
        /// Has this form been seen in at least one gender?
        /// </summary>
        public bool HasFormBeenSeen => GendersSeen.Any(pair => pair.Value);

        /// <summary>
        /// Has this form been caught in at least one gender?
        /// </summary>
        public bool HasFormBeenCaught => GendersCaught.Any(pair => pair.Value);

        /// <summary>
        /// Number of genders of this form seen.
        /// </summary>
        private uint NumberOfGendersSeen => (uint) GendersSeen.Count(pair => pair.Value);

        /// <summary>
        /// Number of genders of this form caught.
        /// </summary>
        private uint NumberOfGendersCaught => (uint) GendersCaught.Count(pair => pair.Value);

        /// <summary>
        /// Total number of this form seen.
        /// </summary>
        public uint TotalSeen;
        
        /// <summary>
        /// Total number of this form caught.
        /// </summary>
        public uint TotalCaught;

        /// <summary>
        /// Total number of this form hatched.
        /// </summary>
        public uint TotalHatched;

        /// <summary>
        /// Parameterless constructor to be used by the save system.
        /// </summary>
        public FormDexEntry()
        {
        }

        /// <summary>
        /// Constructor with species and form to be used when populating.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="form">Form for this entry.</param>
        public FormDexEntry(MonsterEntry species, Form form)
        {
            Form = form;

            DataByFormEntry data = species[form];

            if (data.HasBinaryGender)
            {
                GendersSeen[MonsterGender.Female] = false;
                GendersSeen[MonsterGender.Male] = false;

                GendersCaught[MonsterGender.Female] = false;
                GendersCaught[MonsterGender.Male] = false;
            }
            else
            {
                GendersSeen[MonsterGender.NonBinary] = false;
                GendersCaught[MonsterGender.NonBinary] = false;
            }
        }

        /// <summary>
        /// Get the number of different mons of this form that have been seen.
        /// </summary>
        /// <param name="monsterEntry">Monster this form belongs to.</param>
        /// <returns>0 if not caught, 1 if no gender differences, 2 if there are.</returns>
        public uint GetNumberOfFormsIncludingGenderDifferencesSeen(MonsterDexEntry monsterEntry)
        {
            if (!HasFormBeenSeen) return 0;

            DataByFormEntry data = monsterEntry.Species[Form];

            if (data.HasBinaryGender && data.HasMaleMaterialOverride) return NumberOfGendersSeen;

            return 1;
        }

        /// <summary>
        /// Get the number of different mons of this form that have been caught.
        /// </summary>
        /// <param name="monsterEntry">Monster this form belongs to.</param>
        /// <returns>0 if not caught, 1 if no gender differences, 2 if there are.</returns>
        public uint GetNumberOfFormsIncludingGenderDifferencesCaught(MonsterDexEntry monsterEntry)
        {
            if (!HasFormBeenCaught) return 0;

            DataByFormEntry data = monsterEntry.Species[Form];

            if (data.HasBinaryGender && data.HasMaleMaterialOverride) return NumberOfGendersCaught;

            return 1;
        }

        /// <summary>
        /// Get the total number of different mons this form can have taking into account gender differences.
        /// </summary>
        /// <param name="monsterEntry">Monster this form belongs to.</param>
        /// <returns>1 if no gender differences, 2 if there are.</returns>
        public uint GetNumberOfFormsIncludingGenderDifferences(MonsterDexEntry monsterEntry)
        {
            DataByFormEntry data = monsterEntry.Species[Form];

            if (data.HasBinaryGender && data.HasMaleMaterialOverride) return (uint) GendersSeen.Count;

            return 1;
        }
    }

    /// <summary>
    /// Class representing the entry of a form in the monster dex.
    /// This class can be saved in the savegame.
    /// </summary>
    [Serializable]
    public class SavableFormDexEntry
    {
        /// <summary>
        /// Form this entry belongs to.
        /// </summary>
        public int Form;

        /// <summary>
        /// Genders seen for this form.
        /// </summary>
        public SerializedDictionary<MonsterGender, bool> GendersSeen;

        /// <summary>
        /// Genders caught for this form.
        /// </summary>
        public SerializedDictionary<MonsterGender, bool> GendersCaught;
        
        /// <summary>
        /// Total number of this form seen.
        /// </summary>
        public uint TotalSeen;
        
        /// <summary>
        /// Total number of this form caught.
        /// </summary>
        public uint TotalCaught;

        /// <summary>
        /// Total number of this form hatched.
        /// </summary>
        public uint TotalHatched;

        /// <summary>
        /// Constructor using a form dex entry.
        /// </summary>
        /// <param name="formDexEntry">Form dex entry to build from.</param>
        public SavableFormDexEntry(FormDexEntry formDexEntry)
        {
            Form = formDexEntry.Form.name.GetHashCode();

            GendersSeen = new SerializedDictionary<MonsterGender, bool>();

            foreach (KeyValuePair<MonsterGender, bool> pair in formDexEntry.GendersSeen)
                GendersSeen[pair.Key] = pair.Value;

            GendersCaught = new SerializedDictionary<MonsterGender, bool>();

            foreach (KeyValuePair<MonsterGender, bool> pair in formDexEntry.GendersCaught)
                GendersCaught[pair.Key] = pair.Value;
            
            TotalSeen = formDexEntry.TotalSeen;
            TotalCaught = formDexEntry.TotalCaught;
            TotalHatched = formDexEntry.TotalHatched;
        }

        /// <summary>
        /// Convert this savable form dex entry to a form dex entry.
        /// </summary>
        /// <param name="database">Reference to the monster database.</param>
        public FormDexEntry ToFormDexEntry(MonsterDatabaseInstance database)
        {
            FormDexEntry entry = new()
                                 {
                                     Form = database.GetFormByHash(Form),
                                     GendersSeen = new SerializedDictionary<MonsterGender, bool>(),
                                     GendersCaught = new SerializedDictionary<MonsterGender, bool>()
                                 };

            foreach (KeyValuePair<MonsterGender, bool> pair in GendersSeen) entry.GendersSeen[pair.Key] = pair.Value;

            foreach (KeyValuePair<MonsterGender, bool> pair in GendersCaught)
                entry.GendersCaught[pair.Key] = pair.Value;
            
            entry.TotalSeen = TotalSeen;
            entry.TotalCaught = TotalCaught;
            entry.TotalHatched = TotalHatched;

            return entry;
        }
    }
}