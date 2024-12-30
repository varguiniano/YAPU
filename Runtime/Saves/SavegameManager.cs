using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Serialization;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Class in charge of saving, loading and resetting data.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Savegame/Manager", fileName = "SavegameManager")]
    public class SavegameManager : WhateverScriptable<SavegameManager>
    {
        /// <summary>
        /// List of all the objects that are part of the savegame.
        /// </summary>
        public List<SavableObject> SavegameObjects;

        /// <summary>
        /// Reference to the serializer to use when saving and loading.
        /// </summary>
        private ISerializer<string> serializer;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        private MonsterDatabaseInstance monsterDatabase;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the player teleporter.
        /// </summary>
        private PlayerTeleporter teleporter;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        private IInputManager inputManager;

        /// <summary>
        /// Path to the saves folder.
        /// </summary>
        private static string SavesFolder => Application.persistentDataPath + "/Saves/";

        /// <summary>
        /// Save file extension.
        /// </summary>
        private const string Extension = ".sav";

        /// <summary>
        /// Name of the autosave.
        /// </summary>
        private const string AutosaveName = "Autosave";

        /// <summary>
        /// Name of the older autosave.
        /// </summary>
        private const string OlderAutosaveName = "OlderAutosave";

        /// <summary>
        /// Name of the hash equivalence file.
        /// </summary>
        private const string HashEquivalenceFileName = "HashEquivalence.txt";

        /// <summary>
        /// Content of the hash equivalence file.
        /// </summary>
        private string hashEquivalence;

        /// <summary>
        /// Inject the references and initialize.
        /// </summary>
        [Inject]
        private void Construct(ISerializer<string> saveSerializer,
                               MonsterDatabaseInstance monsterDatabaseInstance,
                               YAPUSettings yapuSettings,
                               PlayerTeleporter playerTeleporter,
                               IInputManager inputManagerReference)
        {
            serializer = saveSerializer;
            monsterDatabase = monsterDatabaseInstance;
            settings = yapuSettings;
            teleporter = playerTeleporter;
            inputManager = inputManagerReference;

            hashEquivalence = null;
        }

        /// <summary>
        /// Test method to test saving the game with the current time.
        /// </summary>
        [Button("Save game")]
        [HideInEditorMode]
        private void TestSaveGameWithCurrentDate(PlayerCharacter playerCharacter) =>
            CoroutineRunner.Instance.StartCoroutine(SaveGameWithCurrentDate(playerCharacter));

        /// <summary>
        /// Save the game with the current date.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        public IEnumerator SaveGameWithCurrentDate(PlayerCharacter playerCharacter)
        {
            yield return SaveGame(playerCharacter, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// Save the game as an autosave, that keeps getting overwritten.
        /// </summary>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        public IEnumerator Autosave(PlayerCharacter playerCharacter)
        {
            string olderAutosavePath = SavesFolder + OlderAutosaveName;
            string autosavePath = SavesFolder + AutosaveName;

            if (Directory.Exists(olderAutosavePath))
            {
                Utils.DeleteDirectory(olderAutosavePath);
                yield return WaitAFrame;
            }

            if (Directory.Exists(autosavePath))
            {
                Utils.CopyFilesRecursively(new DirectoryInfo(autosavePath), new DirectoryInfo(olderAutosavePath));
                yield return WaitAFrame;
                Utils.DeleteDirectory(autosavePath);
                yield return WaitAFrame;
            }

            yield return SaveGame(playerCharacter, AutosaveName);
        }

        /// <summary>
        /// Save the game.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="saveName">Name to give to the save.</param>
        /// <returns></returns>
        private IEnumerator SaveGame(PlayerCharacter playerCharacter, string saveName)
        {
            Logger.Info("Saving the game.");

            inputManager.BlockInput();

            yield return DialogManager.Notifications.ShowSavingNotification();

            if (!Directory.Exists(SavesFolder)) Directory.CreateDirectory(SavesFolder);

            string savePath = SavesFolder + saveName + "/";

            Directory.CreateDirectory(savePath);

            foreach (SavableObject savableObject in SavegameObjects)
            {
                Logger.Info("Saving data for " + savableObject.name + ".");

                string data = savableObject.SaveToText(serializer, playerCharacter);

                File.WriteAllText(savePath + savableObject.name + Extension, data);

                yield return WaitAFrame;
            }

            CreateHashEquivalenceFile(savePath);

            yield return DialogManager.Notifications.ShowSavingNotification(false);

            inputManager.BlockInput(false);

            Logger.Info("Saved the game.");
        }

        /// <summary>
        /// Test loading a save game.
        /// </summary>
        [Button("Load last game")]
        [HideInEditorMode]
        private void TestLoadLastGame() => CoroutineRunner.RunRoutine(LoadLastSavegame());

        /// <summary>
        /// Test loading a save game.
        /// </summary>
        /// <param name="saveName">Name of the save to load.</param>
        [Button("Load game")]
        [HideInEditorMode]
        private void TestLoadGame(string saveName) => CoroutineRunner.RunRoutine(LoadGame(saveName));

        /// <summary>
        /// Method to check if there are any savegames.
        /// </summary>
        /// <returns>True if there are.</returns>
        public static bool AreThereSavegames() =>
            Directory.Exists(SavesFolder) && Directory.GetDirectories(SavesFolder).Length > 0;

        /// <summary>
        /// Get all the savegames available.
        /// </summary>
        /// <param name="savegames">Savegames available.</param>
        /// <returns>True if there are savegames available.</returns>
        public static bool GetAllSavegames(out List<string> savegames)
        {
            savegames = null;
            if (!AreThereSavegames()) return false;

            savegames = Directory.GetDirectories(SavesFolder)
                                 .OrderByDescending(File.GetLastWriteTimeUtc)
                                 .Select(Path.GetFileName)
                                 .ToList();

            return true;
        }

        /// <summary>
        /// Get the basic info from a save.
        /// When this routine finishes, the global game data, time data, player character data and player roster and bag data will have been updated with the save information.
        /// </summary>
        /// <param name="saveName">Save name.</param>
        public IEnumerator LoadBasicSaveInfo(string saveName)
        {
            DialogManager.ShowLoadingIcon();

            yield return WaitAFrame;

            string savePath = SavesFolder + saveName + "/";

            Logger.Info("Loading basic info from save " + saveName + ".");

            if (!Directory.Exists(savePath))
            {
                Logger.Error("That save doesn't exist.");
                DialogManager.ShowLoadingIcon();
                inputManager.BlockInput(false);
                yield break;
            }

            yield return LoadSavableObject(savePath, SavegameObjects.OfType<GlobalGameData>().First());
            yield return LoadSavableObject(savePath, SavegameObjects.OfType<TimeManager>().First());
            yield return LoadSavableObject(savePath, SavegameObjects.OfType<CharacterData>().First());

            DialogManager.ShowLoadingIcon(false);
        }

        /// <summary>
        /// Loads the last savegame.
        /// </summary>
        public IEnumerator LoadLastSavegame()
        {
            if (!GetAllSavegames(out List<string> savegames))
            {
                Logger.Error("No saves to load.");
                yield break;
            }

            yield return LoadGame(Path.GetFileNameWithoutExtension(savegames.First()));
        }

        /// <summary>
        /// Load the game.
        /// </summary>
        /// <param name="saveName">Savegame to load.</param>
        public IEnumerator LoadGame(string saveName)
        {
            yield return WaitAFrame;

            Logger.Info("Loading game " + saveName + ".");

            yield return TransitionManager.BlackScreenFadeInRoutine();
            DialogManager.ShowLoadingIcon();

            string savePath = SavesFolder + saveName + "/";

            if (!Directory.Exists(savePath))
            {
                Logger.Error("That save doesn't exist.");
                yield break;
            }

            foreach (SavableObject savableObject in SavegameObjects)
                yield return LoadSavableObject(savePath, savableObject);

            Logger.Info("Loaded the game, teleporting player.");

            GlobalGameData globalGameData = SavegameObjects.OfType<GlobalGameData>().First();

            yield return teleporter.TeleportPlayer(globalGameData.LastPlayerLocation,
                                                   globalGameData.WasPlayerOnBridgeOnLastLocation,
                                                   true);
        }

        /// <summary>
        /// Load a savable object from its file.
        /// </summary>
        /// <param name="savePath">Full save path.</param>
        /// <param name="savableObject">Object to load.</param>
        private IEnumerator LoadSavableObject(string savePath, SavableObject savableObject)
        {
            string path = savePath + savableObject.name + Extension;

            if (!File.Exists(path))
            {
                Logger.Info("File " + savableObject.name + " does not exist. Skipping.");
                yield break;
            }

            Logger.Info("Loading data for " + savableObject.name + ".");

            NativeQueue<byte> returnData = new(Allocator.Persistent);

            ReadTextFromFileJob readJob = new()
                                          {
                                              Path = new NativeArray<byte>(Encoding.UTF8.GetBytes(path),
                                                                           Allocator.Persistent),
                                              Data = returnData
                                          };

            JobHandle handle = readJob.Schedule();
            yield return new WaitUntil(() => handle.IsCompleted);
            handle.Complete();

            List<byte> readData = new();

            while (returnData.TryDequeue(out byte datum)) readData.Add(datum);

            returnData.Dispose();

            string dataString = Encoding.UTF8.GetString(readData.ToArray());

            yield return savableObject.LoadFromText(serializer, dataString, settings, monsterDatabase);
        }

        /// <summary>
        /// Reset all data.
        /// </summary>
        public IEnumerator ResetSave()
        {
            Logger.Info("Resetting game variables.");
            foreach (SavableObject savegameObject in SavegameObjects) yield return savegameObject.ResetSave();
            hashEquivalence = null;
        }

        /// <summary>
        /// Create the hash equivalence file.
        /// </summary>
        private void CreateHashEquivalenceFile(string savePath) =>
            File.WriteAllText(savePath + HashEquivalenceFileName, GetHashEquivalenceContent());

        /// <summary>
        /// Create the content for the hash equivalence file.
        /// </summary>
        /// <returns></returns>
        private string GetHashEquivalenceContent()
        {
            if (!hashEquivalence.IsNullEmptyOrWhiteSpace()) return hashEquivalence;

            StringBuilder stringBuilder = new();

            stringBuilder
               .AppendLine("Species, forms, abilities, moves and items are stored as hashes of their names to improve performance when loading.");

            stringBuilder
               .AppendLine("If you want to modify the data of any of these, you can use the following equivalence table to find the hash of the name you want to use.");

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Species:");

            // Alphabetical order independent of language.
            foreach (MonsterEntry entry in monsterDatabase.GetMonsterEntries(false).OrderBy(entry => entry.name))
            {
                stringBuilder.Append(entry.name);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendLine(entry.name.GetHashCode().ToString());
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Forms:");

            foreach (Form form in monsterDatabase.GetFormEntries(false).OrderBy(entry => entry.name))
            {
                stringBuilder.Append(form.name);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendLine(form.name.GetHashCode().ToString());
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Abilities:");

            foreach (Ability ability in monsterDatabase.GetAbilities(false).OrderBy(entry => entry.name))
            {
                stringBuilder.Append(ability.name);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendLine(ability.name.GetHashCode().ToString());
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Moves:");

            foreach (Move move in monsterDatabase.GetMoves(false).OrderBy(entry => entry.name))
            {
                stringBuilder.Append(move.name);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendLine(move.name.GetHashCode().ToString());
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Items:");

            foreach (Item item in monsterDatabase.GetItems(false).OrderBy(entry => entry.name))
            {
                stringBuilder.Append(item.name);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendLine(item.name.GetHashCode().ToString());
            }

            hashEquivalence = stringBuilder.ToString();

            return hashEquivalence;
        }
    }
}