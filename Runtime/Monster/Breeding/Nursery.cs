using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster.Saves;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Saves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using WhateverDevs.Localization.Runtime;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Scriptable object that allows for breeding two monsters and create a new one.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Breeding/Nursery", fileName = "Nursery")]
    public class Nursery : SavableObject
    {
        /// <summary>
        /// First monster in the nursery.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private MonsterInstance FirstMonster;

        /// <summary>
        /// Second monster in the nursery.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private MonsterInstance SecondMonster;

        /// <summary>
        /// Steps walked by player when the monsters were left.
        /// </summary>
        [SerializeField]
        private uint StepsWalkedWhenLeft;

        /// <summary>
        /// Steps it takes to lay an egg.
        /// </summary>
        [FoldoutGroup("Settings")]
        [SerializeField]
        private uint StepsToLayEgg = 256;

        /// <summary>
        /// List of forms that will be prioritized when breeding.
        /// </summary>
        [FoldoutGroup("Settings")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private List<Form> PrioritizedForms;

        /// <summary>
        /// Egg groups that can't be bred in this nursery.
        /// </summary>
        [FoldoutGroup("Settings")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllEggGroups))]
        #endif
        private List<EggGroup> UnbreedableGroups;

        /// <summary>
        /// Egg groups that can breed with any other.
        /// </summary>
        [FoldoutGroup("Settings")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllEggGroups))]
        #endif
        private List<EggGroup> WildcardGroups;

        /// <summary>
        /// Check if the nursery is occupied.
        /// </summary>
        public bool IsOccupied => FirstMonster is { IsNullEntry: false } || SecondMonster is { IsNullEntry: false };

        /// <summary>
        /// Can the two given monsters be bred?
        /// </summary>
        /// <param name="firstMonster">First monster to check.</param>
        /// <param name="secondMonster">Second monster to check.</param>
        /// <returns>True if they can be bred, false otherwise.</returns>
        public bool CanBeBred(MonsterInstance firstMonster, MonsterInstance secondMonster)
        {
            // Can't breed if they are the same.
            if (firstMonster == secondMonster) return false;

            // Can't breed if they are eggs.
            if (firstMonster.EggData.IsEgg || secondMonster.EggData.IsEgg) return false;

            // Can't breed if any of them has an unbreedable egg group.
            if (firstMonster.FormData.EggGroups.Any(group => UnbreedableGroups.Contains(group))) return false;
            if (secondMonster.FormData.EggGroups.Any(group => UnbreedableGroups.Contains(group))) return false;

            // Can't breed if none of them has offspring.
            List<(MonsterEntry, Form)> totalPossibleOffspring = new();

            foreach (BreedingData breedingData in firstMonster.FormData.BreedingData)
                totalPossibleOffspring.AddRange(breedingData.GetPossibleOffspring(firstMonster, secondMonster));

            foreach (BreedingData breedingData in secondMonster.FormData.BreedingData)
                totalPossibleOffspring.AddRange(breedingData.GetPossibleOffspring(secondMonster, firstMonster));

            if (totalPossibleOffspring.Count == 0) return false;

            // Can breed if any has a wildcard egg group, regardless of the gender.
            if (firstMonster.FormData.EggGroups.Any(group => WildcardGroups.Contains(group))) return true;
            if (secondMonster.FormData.EggGroups.Any(group => WildcardGroups.Contains(group))) return true;

            // Can't breed if they don't share an egg group.
            if (!firstMonster.FormData.EggGroups.Any(group => secondMonster.FormData.EggGroups.Contains(group)))
                return false;

            // Can breed if they are female and male.
            return (firstMonster.PhysicalData.Gender == MonsterGender.Female
                 && secondMonster.PhysicalData.Gender == MonsterGender.Male)
                || (firstMonster.PhysicalData.Gender == MonsterGender.Male
                 && secondMonster.PhysicalData.Gender == MonsterGender.Female);
        }

        /// <summary>
        /// Store the two given monsters in the nursery.
        /// </summary>
        /// <param name="firstToStore">First monster to store.</param>
        /// <param name="secondToStore">Second monster to store.</param>
        /// <param name="globalGameData">Player's global game data.</param>
        public void StoreMonsters(MonsterInstance firstToStore,
                                  MonsterInstance secondToStore,
                                  GlobalGameData globalGameData)
        {
            FirstMonster = firstToStore;
            SecondMonster = secondToStore;
            UpdateSteps(globalGameData);
        }

        /// <summary>
        /// Get the reference to the stored monsters.
        /// </summary>
        /// <returns>A tuple with the two monsters.</returns>
        public (MonsterInstance, MonsterInstance) GetMonsterReferences() => (FirstMonster, SecondMonster);

        /// <summary>
        /// Retrieve the two monsters from the nursery.
        /// </summary>
        /// <returns>A tuple with the two monsters.</returns>
        public (MonsterInstance, MonsterInstance) RetrieveMonsters()
        {
            (MonsterInstance, MonsterInstance) returnTuple = (FirstMonster, SecondMonster);

            FirstMonster = null;
            SecondMonster = null;

            return returnTuple;
        }

        /// <summary>
        /// Check if an egg is ready to be collected.
        /// </summary>
        /// <param name="globalGameData">Reference to the player's global game data.</param>
        public bool IsEggReady(GlobalGameData globalGameData) =>
            IsOccupied && globalGameData.StepsTaken - StepsWalkedWhenLeft >= StepsToLayEgg;

        /// <summary>
        /// Update the steps since the last visit.
        /// </summary>
        /// <param name="globalGameData">Player's global game data.</param>
        public void UpdateSteps(GlobalGameData globalGameData) => StepsWalkedWhenLeft = globalGameData.StepsTaken;

        /// <summary>
        /// Breed the two stored monsters and create an egg.
        /// </summary>
        /// <param name="database">Reference to the monster database.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The new generated egg.</returns>
        public MonsterInstance Breed(MonsterDatabaseInstance database,
                                     YAPUSettings settings,
                                     PlayerCharacter playerCharacter,
                                     ILocalizer localizer) =>
            Breed(FirstMonster, SecondMonster, database, settings, playerCharacter, localizer);

        /// <summary>
        /// Breed two monsters and create a new egg.
        /// </summary>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="database">Reference to the monster database.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The new generated egg.</returns>
        public MonsterInstance Breed(MonsterInstance firstParent,
                                     MonsterInstance secondParent,
                                     MonsterDatabaseInstance database,
                                     YAPUSettings settings,
                                     PlayerCharacter playerCharacter,
                                     ILocalizer localizer)
        {
            if (!CanBeBred(firstParent, secondParent)) return null;

            (MonsterInstance mother, MonsterInstance father) = DetermineParents(firstParent, secondParent);
            (MonsterEntry species, Form form) = DetermineSpeciesAndForm(mother, father);

            List<Move> learntMoves = DetermineLearntMoves(mother, father, species, form);

            SerializableDictionary<Stat, byte> ivOverride = DetermineIVs(mother, father, species, form);

            Nature nature = DetermineNature(mother, father, species, form);

            Ability ability = DetermineAbility(mother);

            Ball ball = DetermineBall(mother, father);

            if (form.HasShinyVersion && Random.value < settings.WildShinyChance) form = form.ShinyVersion;

            string trainerName = playerCharacter.CharacterController.GetCharacterData()
                                                .LocalizableName;

            return new MonsterInstance(settings,
                                       database,
                                       species,
                                       form,
                                       1,
                                       learntMovesOverride: learntMoves,
                                       ivOverride: ivOverride,
                                       nature: nature,
                                       ability: ability,
                                       captureBall: ball,
                                       originType: OriginData.Type.Hatched,
                                       currentTrainer: trainerName,
                                       originalTrainer: trainerName,
                                       originRegion: localizer[playerCharacter.Region.LocalizableName],
                                       originLocation: localizer[playerCharacter.Scene.LocalizableNameKey],
                                       isEgg: true);
        }

        /// <summary>
        /// Determine which parent is the mother and which is the father.
        /// </summary>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <returns>Mother and father.</returns>
        private (MonsterInstance, MonsterInstance) DetermineParents(MonsterInstance firstParent,
                                                                    MonsterInstance secondParent)
        {
            MonsterInstance mother;
            MonsterInstance father;

            if (firstParent.FormData.EggGroups.Any(group => WildcardGroups.Contains(group)))
            {
                mother = secondParent;
                father = firstParent;
            }
            else if (secondParent.FormData.EggGroups.Any(group => WildcardGroups.Contains(group)))
            {
                mother = firstParent;
                father = secondParent;
            }
            else if (firstParent.PhysicalData.Gender == MonsterGender.Male)
            {
                mother = secondParent;
                father = firstParent;
            }
            else
            {
                mother = firstParent;
                father = secondParent;
            }

            return (mother, father);
        }

        /// <summary>
        /// Determine the species and form of the offspring.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <returns>Species and form of the offspring.</returns>
        private (MonsterEntry, Form) DetermineSpeciesAndForm(MonsterInstance mother,
                                                             MonsterInstance father)
        {
            List<(MonsterEntry, Form)> possibleOffspring = new();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (BreedingData data in mother.FormData.BreedingData)
            {
                List<(MonsterEntry, Form)> candidates = data.GetPossibleOffspring(mother, father);
                if (candidates.Count <= 0) continue;

                foreach ((MonsterEntry, Form) candidate in candidates) possibleOffspring.AddIfNew(candidate);
            }

            // If there are offsprings that are forced by this nursery, like an alolan nursery, return a random one of those.
            // Held items may also force a specific form too.
            List<(MonsterEntry, Form)> forcedOffspring = new();

            bool forcePassMothersForm = mother.OnCalculateFormToPassOnBreeding(father, true);
            bool forcePassFathersForm = father.OnCalculateFormToPassOnBreeding(mother, false);
            Form mothersForm = mother.Form;
            Form fathersForm = father.Form;

            foreach ((MonsterEntry possibleSpecies, Form possibleForm) in possibleOffspring)
            {
                if (forcePassMothersForm && possibleForm == mothersForm)
                    forcedOffspring.AddIfNew((possibleSpecies, possibleForm));

                if (forcePassFathersForm && possibleForm == fathersForm)
                    forcedOffspring.AddIfNew((possibleSpecies, possibleForm));

                if (PrioritizedForms.Contains(possibleForm)) forcedOffspring.AddIfNew((possibleSpecies, possibleForm));
            }

            if (forcedOffspring.Count > 0) return forcedOffspring.Random();

            // If there are no forced offsprings, collect the first form of each species option and return a random one among them.
            // For example, this will collect both Nidoran species with only the first form of each and return a random one among them.
            List<(MonsterEntry Species, Form)> firstFormPossibleOffspring = new();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach ((MonsterEntry Species, Form) offspringData in possibleOffspring)
                if (firstFormPossibleOffspring.All(entry => entry.Species != offspringData.Species))
                    firstFormPossibleOffspring.Add(offspringData);

            return firstFormPossibleOffspring.Random();
        }

        /// <summary>
        /// Determine the moves the offspring will know when hatched.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <returns>A list of moves that the offspring will know when hatched.</returns>
        private static List<Move> DetermineLearntMoves(MonsterInstance mother,
                                                       MonsterInstance father,
                                                       MonsterEntry species,
                                                       Form form)
        {
            List<Move> offspringLearntMoves = new();

            // If any parent knows an egg move, the offspring will know it.
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Move eggMove in species[form].EggMoves)
            {
                if (offspringLearntMoves.Contains(eggMove)) continue;

                if (mother.LearntMoves.Contains(eggMove) || father.LearntMoves.Contains(eggMove))
                    offspringLearntMoves.Add(eggMove);
            }

            // If both parents know a move by level, the offspring will know it.
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (MoveLevelPair pair in species[form].MovesByLevel)
            {
                if (offspringLearntMoves.Contains(pair.Move)) continue;

                if (mother.LearntMoves.Contains(pair.Move) && father.LearntMoves.Contains(pair.Move))
                    offspringLearntMoves.Add(pair.Move);
            }

            foreach (Move move in mother.AddExtraLearntMovesWhenBreeding(true, father, species, form)
                                        .Where(move => !offspringLearntMoves.Contains(move)))
                offspringLearntMoves.Add(move);

            foreach (Move move in father.AddExtraLearntMovesWhenBreeding(false, mother, species, form)
                                        .Where(move => !offspringLearntMoves.Contains(move)))
                offspringLearntMoves.Add(move);

            return offspringLearntMoves;
        }

        /// <summary>
        /// Determine the IVs of the offspring.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <returns>IVs to set on the offspring. Non calculated IVs will be random.</returns>
        private static SerializableDictionary<Stat, byte> DetermineIVs(MonsterInstance mother,
                                                                       MonsterInstance father,
                                                                       MonsterEntry species,
                                                                       Form form)
        {
            SerializableDictionary<Stat, byte> ivs = new();
            List<Stat> allStats = Utils.GetAllItems<Stat>().ToList();

            List<MonsterInstance> parents = new()
                                            {
                                                mother,
                                                father
                                            };

            int numberOfIVsToPassDown = 3;

            int ivPassDownOverride = mother.OnCalculateNumberOfIVsToPassOnBreeding(father, species, form, true);
            if (ivPassDownOverride != -1) numberOfIVsToPassDown = ivPassDownOverride;
            ivPassDownOverride = father.OnCalculateNumberOfIVsToPassOnBreeding(mother, species, form, false);
            if (ivPassDownOverride != -1) numberOfIVsToPassDown = ivPassDownOverride;

            SerializableDictionary<Stat, byte> motherIVsToForcePass =
                mother.OnCalculateSpecificIVsToPassOnBreeding(father, species, form, true);

            SerializableDictionary<Stat, byte> fatherIVsToForcePass =
                father.OnCalculateSpecificIVsToPassOnBreeding(mother, species, form, false);

            foreach (KeyValuePair<Stat, byte> pair in motherIVsToForcePass)
            {
                // If both want to pass it, choose one at random.
                if (fatherIVsToForcePass.TryGetValue(pair.Key, out byte iv))
                    ivs[pair.Key] = new List<byte>
                                    {
                                        pair.Value,
                                        iv
                                    }.Random();
                else
                    ivs[pair.Key] = pair.Value;

                numberOfIVsToPassDown--;
            }

            foreach (KeyValuePair<Stat, byte> pair in fatherIVsToForcePass)
            {
                if (ivs.ContainsKey(pair.Key)) continue;

                ivs[pair.Key] = pair.Value;

                numberOfIVsToPassDown--;
            }

            while (numberOfIVsToPassDown > 0)
            {
                Stat stat = allStats.Random();

                if (ivs.ContainsKey(stat)) continue;

                ivs[stat] = parents.Random().StatData.IndividualValues[stat];

                numberOfIVsToPassDown--;
            }

            return ivs;
        }

        /// <summary>
        /// Determine the nature of the offspring.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <returns>Nature the offspring will have. If null, it will be random.</returns>
        private static Nature DetermineNature(MonsterInstance mother,
                                              MonsterInstance father,
                                              MonsterEntry species,
                                              Form form)
        {
            bool inheritMothersNature = mother.OnCalculateNatureToPassOnBreeding(father, species, form, true);
            bool inheritFathersNature = father.OnCalculateNatureToPassOnBreeding(mother, species, form, false);

            return inheritMothersNature switch
            {
                true when !inheritFathersNature => mother.StatData.Nature,
                false when inheritFathersNature => father.StatData.Nature,
                // If both want to pass it, choose one at random.
                true => new List<Nature>
                        {
                            mother.StatData.Nature,
                            father.StatData.Nature
                        }.Random(),
                _ => null
            };
        }

        /// <summary>
        /// Determine the ability of the offspring.
        /// Hidden abilities from the mother have a 60% chance of being passed down, non hidden have an 80% chance.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <returns>Ability the offspring will have. If null, it will be random.</returns>
        private static Ability DetermineAbility(MonsterInstance mother) =>
            Random.value <= (mother.FormData.HiddenAbilities.Contains(mother.GetAbility()) ? .6f : .8f)
                ? mother.GetAbility()
                : null;

        /// <summary>
        /// Determine the Ball of the offspring.
        /// It is random between parents if same species.
        /// It's the mother's ball if different species.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <returns>Ball the offspring will have. If null, it will be random.</returns>
        private static Ball DetermineBall(MonsterInstance mother, MonsterInstance father)
        {
            if (mother.Species == father.Species)
                return new List<MonsterInstance>
                       {
                           mother,
                           father
                       }.Random()
                        .OriginData.Ball;

            return mother.OriginData.Ball;
        }

        /// <summary>
        /// Save this data to a serializable string.
        /// </summary>
        /// <param name="serializer">String serializer to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>The serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableNursery(this));

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings">Reference to the YAPUSettings.</param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableNursery readData = serializer.From<SavableNursery>(data);

            yield return WaitAFrame;

            readData.LoadNursery(this, yapuSettings, monsterDatabase);
        }

        /// <summary>
        /// Reset the list.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            FirstMonster = null;
            SecondMonster = null;
            StepsWalkedWhenLeft = 0;
            yield break;
        }

        /// <summary>
        /// Savable version of the nursery.
        /// </summary>
        [Serializable]
        public class SavableNursery
        {
            /// <summary>
            /// First monster in the nursery.
            /// </summary>
            public SavableMonsterInstance FirstMonster;

            /// <summary>
            /// Second monster in the nursery.
            /// </summary>
            public SavableMonsterInstance SecondMonster;

            /// <summary>
            /// Steps walked by the player when they left the monsters in the nursery.
            /// </summary>
            public uint StepsWalkedWhenLeft;

            /// <summary>
            /// Create a savable nursery from a nursery.
            /// </summary>
            /// <param name="nursery">Nursery to create from.</param>
            public SavableNursery(Nursery nursery)
            {
                FirstMonster = new SavableMonsterInstance(nursery.FirstMonster);
                SecondMonster = new SavableMonsterInstance(nursery.SecondMonster);
                StepsWalkedWhenLeft = nursery.StepsWalkedWhenLeft;
            }

            /// <summary>
            /// Load back into the Nursery.
            /// </summary>
            /// <param name="nursery">Nursery to load back to.</param>
            /// <param name="settings">Settings reference.</param>
            /// <param name="database">Database reference.</param>
            public void LoadNursery(Nursery nursery, YAPUSettings settings, MonsterDatabaseInstance database)
            {
                nursery.FirstMonster = FirstMonster.ToMonsterInstance(settings, database);
                nursery.SecondMonster = SecondMonster.ToMonsterInstance(settings, database);
                nursery.StepsWalkedWhenLeft = StepsWalkedWhenLeft;
            }
        }
    }
}