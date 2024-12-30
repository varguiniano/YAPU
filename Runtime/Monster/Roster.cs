using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster.Saves;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Saves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Object that represents a roster of up to 6 monsters that can be used in battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Roster", fileName = "Roster")]
    public class Roster : SavableObject, IEnumerable
    {
        /// <summary>
        /// Default trainer name for this roster.
        /// </summary>
        [SerializeField]
        private string DefaultTrainerName = "Wild";

        /// <summary>
        /// Roster data.
        /// </summary>
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public MonsterInstance[] RosterData = new MonsterInstance[6];

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [FoldoutGroup("References")]
        public YAPUSettings Settings;

        /// <summary>
        /// Reference to the database.
        /// </summary>
        [FoldoutGroup("References")]
        public MonsterDatabaseInstance Database;

        /// <summary>
        /// Get the size of this roster data.
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-1)]
        public int RosterSize
        {
            get
            {
                for (int i = 0; i < RosterData.Length; ++i)
                    if (IsMonsterNull(i))
                        return i;

                return RosterData.Length;
            }
        }

        /// <summary>
        /// Get the number of mons that can battle.
        /// </summary>
        public int NumberThatCanBattle => RosterData.Count(monster => monster is {IsNullEntry: false, CanBattle: true});

        /// <summary>
        /// Quick access to a monster instance.
        /// </summary>
        /// <param name="index">Index of that monster instance.</param>
        public MonsterInstance this[int index]
        {
            get => RosterData[index];
            set => RosterData[index] = value;
        }

        /// <summary>
        /// Quick access to the data enumerator.
        /// </summary>
        /// <returns>The data enumerator.</returns>
        public IEnumerator GetEnumerator() => RosterData.GetEnumerator();

        /// <summary>
        /// Checks if a monster is null.
        /// </summary>
        /// <param name="index">Index to check.</param>
        /// <returns>True if it is null.</returns>
        public bool IsMonsterNull(int index) => this[index] == null || this[index].IsNullEntry;

        /// <summary>
        /// Add a monster to the roster.
        /// </summary>
        /// <param name="species">Species of this monster.</param>
        /// <param name="form">Form of this monster. If an invalid form is passed, it will use the default.</param>
        /// <param name="level">Level of this monster.</param>
        /// <param name="nature">Nature of this monster. Optional, a random one will be chosen.</param>
        /// <param name="ability">Ability of this monster. Optional, a random one will be chosen.</param>
        /// <param name="gender">Gender of this monster. Non binary species will take non binary always, binary species can be overriden by this parameter or a random one will be chosen.</param>
        /// <param name="heightModifierOverride">Override for the height multiplier.</param>
        /// <param name="weightModifierOverride">Override for the weight modifier.</param>
        /// <param name="friendshipModifier">Friendship of this monster. It will be added to the species base.</param>
        /// <param name="moveRosterOverride">Move roster for this monster. Optional, a random one will be chosen. If override contains invalid moves for the species, random ones will be chosen for those moves.</param>
        /// <param name="ivOverride">IVs for this monster. Optional, random ones will be chosen.</param>
        /// <param name="evOverride">EVs for this monster. Optional, default will set them as 0s.</param>
        /// <param name="conditionsOverride">Conditions for this monster. Optional, default will set them as 0s.</param>
        /// <param name="maxFormLevel">Level of the max form for this monster. Optional, default is 0.</param>
        /// <param name="nickname">Nickname for this monster. Optional, it won't have a nick name if not set.</param>
        /// <param name="currentTrainer">Current trainer of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originRegion">Origin region of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originLocation">Origin location of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originalTrainer">Original trainer of this monster. Optional, the current trainer will be used.</param>
        /// <param name="captureBall">Ball it was captured with.</param>
        /// <param name="originType">Type of origin of this monster. Default is caught.</param>
        /// <param name="isAlpha">Is this an alpha monster?</param>
        /// <param name="hasVirus">Does this monster have the virus? Default is false.</param>
        /// <param name="ribbons">Ribbons for this monster. Default is none.</param>
        /// <param name="heldItem">Item this monster is holding.</param>
        /// <param name="isEgg">Is this monster an egg?</param>
        [Button("Add Monster")]
        [HideIf("@RosterSize >= 6")]
        [InfoBox("Only species, form and lever are needed, the rest can be left as default.")]
        // ReSharper disable once FunctionComplexityOverflow
        public void AddMonster(
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllMonsters))]
            #endif
            MonsterEntry species,
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllForms))]
            #endif
            Form form,
            byte level,
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllNatures))]
            #endif
            Nature nature = null,
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllAbilities))]
            #endif
            Ability ability = null,
            MonsterGender gender = MonsterGender.NonBinary,
            float heightModifierOverride = -1,
            float weightModifierOverride = -1,
            int friendshipModifier = 0,
            Move[] moveRosterOverride = null,
            SerializableDictionary<Stat, byte> ivOverride = null,
            SerializableDictionary<Stat, byte> evOverride = null,
            SerializableDictionary<Condition, byte> conditionsOverride = null,
            byte maxFormLevel = 0,
            string nickname = "",
            string currentTrainer = "",
            string originRegion = "YAPULand",
            string originLocation = "Route 1",
            string originalTrainer = "",
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllBalls))]
            #endif
            Ball captureBall = null,
            OriginData.Type originType = OriginData.Type.Caught,
            bool isAlpha = false,
            bool hasVirus = false,
            List<Ribbon> ribbons = null,
            #if UNITY_EDITOR
            [ValueDropdown(nameof(GetAllItems))]
            #endif
            Item heldItem = null,
            bool isEgg = false)
        {
            AddMonster(new MonsterInstance(Settings,
                                           Database,
                                           species,
                                           form,
                                           level,
                                           nature,
                                           ability,
                                           gender,
                                           heightModifierOverride,
                                           weightModifierOverride,
                                           friendshipModifier,
                                           moveRosterOverride,
                                           null,
                                           ivOverride,
                                           evOverride,
                                           conditionsOverride,
                                           maxFormLevel,
                                           nickname,
                                           currentTrainer.IsNullEmptyOrWhiteSpace()
                                               ? DefaultTrainerName
                                               : currentTrainer,
                                           originRegion,
                                           originLocation,
                                           originalTrainer,
                                           originType,
                                           captureBall,
                                           isAlpha,
                                           hasVirus,
                                           ribbons,
                                           heldItem,
                                           isEgg));
        }

        /// <summary>
        /// Add a monster to the roster.
        /// </summary>
        /// <param name="monster">Monster to add.</param>
        /// <returns>The index it was added at.</returns>
        public int AddMonster(MonsterInstance monster)
        {
            for (int i = 0; i < RosterData.Length; ++i)
                if (IsMonsterNull(i))
                {
                    this[i] = monster;
                    return i;
                }

            Logger.Error("All slots already filled!");
            return -1;
        }

        /// <summary>
        /// Removes a monster in the roster.
        /// </summary>
        [Button]
        [HideIf("@RosterSize == 0")]
        public void RemoveMonster(int index)
        {
            if (IsMonsterNull(index)) return;

            this[index] = null;

            for (int i = index; i < 5; ++i) Exchange(i, i + 1);
        }

        /// <summary>
        /// Removes a monster in the roster.
        /// </summary>
        /// <param name="monster">Monster to remove.</param>
        public void RemoveMonster(MonsterInstance monster) => RemoveMonster(RosterData.IndexOf(monster));

        /// <summary>
        /// Exchange two monsters in the roster.
        /// </summary>
        /// <param name="firstIndex">First index to exchange.</param>
        /// <param name="secondIndex">Second index to exchange.</param>
        [Button]
        [HideIf("@RosterSize <= 1")]
        public void Exchange(int firstIndex, int secondIndex) =>
            (this[firstIndex], this[secondIndex]) = (this[secondIndex], this[firstIndex]);

        /// <summary>
        /// Replace the monster at the given index for another.
        /// </summary>
        /// <param name="index">Index to replace.</param>
        /// <param name="newMonster">Monster to add.</param>
        /// <returns>Replaced monster.</returns>
        public MonsterInstance ReplaceMonsterAt(int index, MonsterInstance newMonster)
        {
            MonsterInstance replaced = RosterData[index];
            RosterData[index] = newMonster;
            return replaced;
        }

        /// <summary>
        /// Load the roster changes from a battlers list.
        /// We only need to copy stat data, moves, status, friendship and held item data.
        /// Calculate virus chance.
        /// </summary>
        public void LoadFromBattlers(List<Battler> battlers, YAPUSettings settings)
        {
            for (int i = 0; i < battlers.Count; i++)
            {
                Battler battler = battlers[i];

                if (battler?.IsNullEntry != false) continue;

                RosterData[i].StatData = battler.StatData.DeepClone();
                RosterData[i].CurrentHP = battler.CurrentHP;
                RosterData[i].Friendship = battler.Friendship;
                RosterData[i].SetStatus(battler.GetStatusForOutOfBattle());

                RosterData[i].CurrentMoves = battler.CloneMoveSlots();

                // Recover the moves that were replaced by stuff like Mimic.
                foreach (KeyValuePair<int, MoveSlot> pair in battler.RecoverableMoves)
                    RosterData[i].CurrentMoves[pair.Key] = pair.Value;

                RosterData[i].LearntMoves = battler.LearntMoves.ShallowClone();
                RosterData[i].EggData = battler.EggData;
                RosterData[i].ExtraData = battler.ExtraData;
                RosterData[i].HeldItem = battler.HeldItem;

                // Items that got stolen are recovered and override the ones added after the steal.
                if (battler.ConsumedItemData.HasConsumedItem && battler.ConsumedItemData.CanBeRecoveredAfterBattle)
                    RosterData[i].HeldItem = battler.ConsumedItemData.ConsumedItem;

                if (!RosterData[i].VirusData.CanGetInfected) continue;

                float chance = Random.value;

                if (chance <= settings.AfterBattleVirusChance) RosterData[i].VirusData.Infect();
            }
        }

        /// <summary>
        /// Template roster that can be used to default this roster.
        /// </summary>
        [FoldoutGroup("Data management")]
        [SerializeField]
        private Roster Template;

        /// <summary>
        /// Load the template.
        /// </summary>
        [FoldoutGroup("Data management")]
        [Button]
        public void LoadTemplate() => CopyFrom(Template);

        /// <summary>
        /// Save this data to a serialized string.
        /// </summary>
        /// <param name="serializer">String serializer to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>The serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableRoster(this));

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings"></param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableRoster readData = serializer.From<SavableRoster>(data);

            yield return WaitAFrame;

            readData.LoadRoster(this, Settings, monsterDatabase);
        }

        /// <summary>
        /// Reset the data to its default values.
        /// </summary>
        [FoldoutGroup("Data management")]
        [Button]
        public override IEnumerator ResetSave()
        {
            LoadTemplate();
            yield break;
        }

        /// <summary>
        /// Clone another roster and save it here.
        /// </summary>
        public void CopyFrom(Roster other)
        {
            if (other == null) return;

            for (int i = 0; i < other.RosterData.Length; i++) RosterData[i] = other.RosterData[i].Clone();

            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            #endif
        }

        /// <summary>
        /// Populate this roster with random monsters.
        /// This is more random than the actual default, also randomizing between all available moves (not only level ones),
        /// allowing the hidden abilities to be chosen and randomizing EVs and held item.
        /// </summary>
        [FoldoutGroup("Data management")]
        [Button]
        public void PopulateRandomly(YAPUSettings settings,
                                     int minMonstersInRoster = 1,
                                     byte minLevel = 1,
                                     byte maxLevel = 100,
                                     List<MonsterEntry> customMonsterPool = null)
        {
            if (customMonsterPool == null || customMonsterPool.Count == 0)
                customMonsterPool = Database.GetMonsterEntries(false);

            for (int i = 0; i < 6; ++i) RosterData[i] = null;

            // Random number of mons.
            for (int i = 0; i < Random.Range(minMonstersInRoster, 6); ++i)
            {
                MonsterEntry species = customMonsterPool.Random();
                Form form = species.AvailableForms.Where(form => !form.IsCombatForm).ToList().Random();

                if (form.HasShinyVersion && Random.value < Settings.WildShinyChance) form = form.ShinyVersion;

                byte level = (byte) Random.Range(minLevel, maxLevel + 1);

                Move[] moveRosterOverride = new Move[4];

                for (int j = 0; j < 4; j++)
                {
                    Move candidate;

                    do
                        candidate = species[form].GetAllAvailableMoves(Database).Random();
                    while (moveRosterOverride.Contains(candidate)
                        && moveRosterOverride.Count(move => move != null)
                         < species[form].GetAllAvailableMoves(Database).Count);

                    moveRosterOverride[j] = candidate;
                }

                Ability ability = species[form].AllAvailableAbilities.Random();

                SerializableDictionary<Stat, byte> evs = new();
                foreach (Stat stat in Utils.GetAllItems<Stat>()) evs[stat] = (byte) Random.Range(0, 255);

                Item heldItem = Database.GetItems(false).Where(item => item.CanBeHeld).ToList().Random();

                AddMonster(species,
                           form,
                           level,
                           ability: ability,
                           moveRosterOverride: moveRosterOverride,
                           evOverride: evs,
                           heldItem: heldItem);
            }
        }

        /// <summary>
        /// Regenerate the monsters using only their current species, form and level.
        /// </summary>
        [FoldoutGroup("Data management")]
        [Button]
        private void ReGenerateMonsters()
        {
            List<MonsterEntry> entries = new();
            List<Form> forms = new();
            List<byte> levels = new();

            for (int i = 0; i < RosterData.Length; i++)
            {
                MonsterInstance monsterInstance = RosterData[i];
                if (monsterInstance?.IsNullEntry != false) continue;

                entries.Add(monsterInstance.Species);
                forms.Add(monsterInstance.Form);
                levels.Add(monsterInstance.StatData.Level);
            }

            for (int i = 5; i >= 0; i--) RemoveMonster(i);

            for (int i = 0; i < entries.Count; i++) AddMonster(entries[i], forms[i], levels[i]);
        }

        /// <summary>
        /// Completely heal the roster.
        /// </summary>
        public void CompletelyHeal()
        {
            foreach (MonsterInstance monsterInstance in this)
                if (monsterInstance is {IsNullEntry: false})
                    monsterInstance.CompletelyHeal();
        }

        /// <summary>
        /// Does this roster contain a move?
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if it does.</returns>
        public bool AnyHasMove(Move move)
        {
            foreach (MonsterInstance monsterInstance in this)
                if (monsterInstance is {IsNullEntry: false} && monsterInstance.KnowsMove(move))
                    return true;

            return false;
        }

        /// <summary>
        /// Trigger the virus spread and the infection ticks on each monster.
        /// </summary>
        public void TriggerVirusSpread()
        {
            Logger.Info("Triggering virus spread.");

            for (int i = 0; i < RosterData.Length; ++i)
            {
                MonsterInstance monster = RosterData[i];

                if (monster == null || monster.IsNullEntry) continue;

                if (!monster.VirusData.HasVirus) continue;

                // 1/3 chance to spread.
                if (Random.value <= .333333f)
                {
                    List<MonsterInstance> spreadCandidates = new();

                    if (i - 1 >= 0 && RosterData[i - 1].VirusData.CanGetInfected)
                        spreadCandidates.Add(RosterData[i - 1]);

                    if (i + 1 < RosterData.Length && RosterData[i + 1].VirusData.CanGetInfected)
                        spreadCandidates.Add(RosterData[i + 1]);

                    if (spreadCandidates.Count > 0)
                    {
                        MonsterInstance victim = spreadCandidates.Random();

                        Logger.Info($"Spreading virus from {monster.Species.LocalizableName} to {victim.Species.LocalizableName}.");
                        victim.VirusData.Infect();
                    }
                }

                monster.VirusData.InfectionTick();
            }
        }

        /// <summary>
        /// Regenerates the monsters on the template and then reloads the template.
        /// </summary>
        [FoldoutGroup("Data management")]
        [Button]
        private void ReGenerateInTemplateAndReload()
        {
            Template.ReGenerateMonsters();

            #if UNITY_EDITOR
            EditorUtility.SetDirty(Template);
            AssetDatabase.SaveAssets();
            #endif

            LoadTemplate();
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Called when the inspector initializes.
        /// </summary>
        [OnInspectorInit]
        protected override void InspectorInit()
        {
            base.InspectorInit();
            CheckReferences();
        }

        /// <summary>
        /// Check the references and add them if null.
        /// </summary>
        private void CheckReferences()
        {
            try
            {
                if (Settings == null)
                {
                    EditorUtility.DisplayProgressBar("Roster", "Looking for references...", 25f);
                    Settings = AssetManagementUtils.FindAssetsByType<YAPUSettings>().First();
                }

                if (Database != null) return;
                EditorUtility.DisplayProgressBar("Roster", "Looking for references...", 75f);
                Database = AssetManagementUtils.FindAssetsByType<MonsterDatabaseInstance>().First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        #endif

        /// <summary>
        /// Version of the roster class that can be saved as a string.
        /// </summary>
        [Serializable]
        public class SavableRoster
        {
            /// <summary>
            /// Monsters in the roster.
            /// </summary>
            public List<SavableMonsterInstance> Monsters;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="roster">Original roster.</param>
            public SavableRoster(Roster roster)
            {
                Monsters = new List<SavableMonsterInstance>();

                foreach (MonsterInstance instance in roster)
                    if (instance is {IsNullEntry: false})
                        Monsters.Add(new SavableMonsterInstance(instance));
            }

            /// <summary>
            /// Load the data back to the roster.
            /// </summary>
            /// <param name="roster">roster to load to.</param>
            /// <param name="settings">Settings reference.</param>
            /// <param name="database">Database reference.</param>
            public void LoadRoster(Roster roster, YAPUSettings settings, MonsterDatabaseInstance database)
            {
                roster.RosterData = new MonsterInstance[6];

                for (int i = 0; i < Monsters.Count; i++) roster[i] = Monsters[i].ToMonsterInstance(settings, database);
            }
        }
    }
}