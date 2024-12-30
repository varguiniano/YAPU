using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster.Saves;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Serialization;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Storage for the player's monsters that are not on the roster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Player/MonsterStorage", fileName = "MonsterStorage")]
    public class MonsterStorage : SavableObject
    {
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
        /// List of all monsters in the storage.
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private List<MonsterInstance> Monsters = new();

        /// <summary>
        /// Get the monster on the given index.
        /// </summary>
        /// <param name="index">Index to check.</param>
        public MonsterInstance this[int index] => Monsters[index];

        /// <summary>
        /// Total monsters in the storage.
        /// </summary>
        public int Count => Monsters.Count;

        /// <summary>
        /// Get the monster list.
        /// </summary>
        public List<MonsterInstance> GetMonsters() => Monsters;

        /// <summary>
        /// Add a monster to the storage.
        /// </summary>
        /// <param name="monster">Monster to add.</param>
        public void AddMonster(MonsterInstance monster) => Monsters.Add(monster);

        /// <summary>
        /// Remove a monster from the storage.
        /// </summary>
        /// <param name="monster">Monster to remove.</param>
        public void RemoveMonster(MonsterInstance monster) => Monsters.Remove(monster);

        /// <summary>
        /// Background loading of the storage routine.
        /// </summary>
        private Coroutine backgroundLoad;

        /// <summary>
        /// Transfer a monster from a roster into the storage.
        /// </summary>
        /// <param name="origin">Original roster.</param>
        /// <param name="index">Index of the monster to transfer.</param>
        public void TransferMonsterFromRoster(Roster origin, int index)
        {
            if (origin[index] == null || origin[index].IsNullEntry)
            {
                Logger.Warn("No valid monster to transfer.");
                return;
            }

            MonsterInstance monster = origin[index];

            origin.RemoveMonster(index);

            Monsters.Add(monster);
        }

        /// <summary>
        /// Transfer a monster from storage to a roster.
        /// </summary>
        /// <param name="target">Target roster.</param>
        /// <param name="index">Index in the storage.</param>
        /// <returns>The index it was added at.</returns>
        public int TransferMonsterToRoster(Roster target, int index)
        {
            if (Monsters.Count <= index)
            {
                Logger.Warn("No monster in index " + index + ".");
                return -1;
            }

            if (target.RosterSize >= 6)
            {
                Logger.Warn("Target roster is full.");
                return -1;
            }

            MonsterInstance monster = Monsters[index];

            Monsters.Remove(monster);

            return target.AddMonster(monster);
        }

        /// <summary>
        /// Exchange a monster in the storage with a monster in a roster.
        /// </summary>
        /// <param name="roster">Roster to use.</param>
        /// <param name="rosterIndex">Index of the monster to swap in the roster.</param>
        /// <param name="storageIndex">Index of the monster to swap in the storage.</param>
        /// <returns></returns>
        public void ExchangeMonsterWithRoster(Roster roster, int rosterIndex, int storageIndex)
        {
            if (Monsters.Count <= storageIndex)
            {
                Logger.Warn("No monster in index " + storageIndex + ".");
                return;
            }

            if (roster.RosterSize <= rosterIndex)
            {
                Logger.Warn("Target roster doesn't have a monster in index " + rosterIndex + ".");
                return;
            }

            MonsterInstance incomingMonster = Monsters[storageIndex];

            Monsters.Remove(incomingMonster);

            MonsterInstance outgoingMonster = roster[rosterIndex];

            Monsters.Add(outgoingMonster);

            roster[rosterIndex] = incomingMonster;
        }

        /// <summary>
        /// Save this data to a serializable string.
        /// </summary>
        /// <param name="serializer">String serializer to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>The serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter)
        {
            StringBuilder storageString = new();

            for (int i = 0; i < Monsters.Count; i++)
            {
                storageString.AppendLine(serializer.To(new SavableMonsterInstance(Monsters[i])));
                if (i < Monsters.Count - 1) storageString.AppendLine("---");
            }

            return storageString.ToString();
        }

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
            if (data.IsNullEmptyOrWhiteSpace()) yield break;

            Stopwatch stopwatch = new();
            stopwatch.Start();

            Monsters = new List<MonsterInstance>();

            string[] split = data.Split("---");

            int interval = split.Length / Mathf.Min(500, split.Length);

            for (int i = 0; i < split.Length; i++)
            {
                Monsters.Add(serializer.From<SavableMonsterInstance>(split[i])
                                       .ToMonsterInstance(yapuSettings, monsterDatabase));

                if (i % interval != 0) continue;

                if (stopwatch.ElapsedMilliseconds > 5000)
                    DialogManager.ShowLoadingText("Dialogs/LoadingStorage",
                                                  false,
                                                  i.ToString(),
                                                  split.Length.ToString(),
                                                  stopwatch.ElapsedMilliseconds.ToString());

                yield return new WaitForEndOfFrame();
            }

            DialogManager.ClearLoadingText();

            stopwatch.Stop();

            StaticLogger.Info("Monsters loaded in " + stopwatch.ElapsedMilliseconds + " ms.");
        }

        /// <summary>
        /// Reset the list.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            Clear();

            if (TestFillOnSaveReset) TestFill(TestFillAmount, TestFillSpecies, TestFillForm);

            AppEventsListener.Instance.AppQuitting += () => Monsters.Clear();

            yield break;
        }

        /// <summary>
        /// Clear the list.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button("Clear")]
        private void Clear() => Monsters = new List<MonsterInstance>();

        /// <summary>
        /// Test fill the storage on save reset?
        /// </summary>
        [FoldoutGroup("Debug")]
        [SerializeField]
        private bool TestFillOnSaveReset;

        /// <summary>
        /// Amount of monsters to add.
        /// </summary>
        [FoldoutGroup("Debug")]
        [SerializeField]
        [ShowIf(nameof(TestFillOnSaveReset))]
        private int TestFillAmount;

        /// <summary>
        /// Species of this monster.
        /// </summary>
        [FoldoutGroup("Debug")]
        [SerializeField]
        [ShowIf(nameof(TestFillOnSaveReset))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        private MonsterEntry TestFillSpecies;

        /// <summary>
        /// Form of this monster. If an invalid form is passed, it will use the default.
        /// </summary>
        [FoldoutGroup("Debug")]
        [SerializeField]
        [ShowIf(nameof(TestFillOnSaveReset))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private Form TestFillForm;

        /// <summary>
        /// Add a monster to the roster.
        /// </summary>
        /// <param name="amount">Amount of monsters to add.</param>
        /// <param name="species">Species of this monster.</param>
        /// <param name="form">Form of this monster. If an invalid form is passed, it will use the default.</param>
        [Button("Test fill")]
        [FoldoutGroup("Debug")]
        [HideInEditorMode]
        private void TestFill(int amount,
                              #if UNITY_EDITOR
                              [ValueDropdown(nameof(GetAllMonsters))]
                              #endif
                              MonsterEntry species,
                              #if UNITY_EDITOR
                              [ValueDropdown(nameof(GetAllForms))]
                              #endif
                              Form form)
        {
            for (int i = 0; i < amount; ++i)
            {
                MonsterInstance instance = new(Settings,
                                               Database,
                                               species,
                                               form,
                                               (byte)Random.Range(1, 101));

                Monsters.Add(instance);
            }
        }
    }
}