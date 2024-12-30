using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Configuration holder for the audio configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/GameFlow/GameplayConfiguration", fileName = "GameplayConfiguration")]
    public class
        GameplayConfigurationFile : ConfigurationScriptableHolderUsingFirstValidPersister<GameplayConfiguration>
    {
    }

    /// <summary>
    /// Configuration for the gameplay.
    /// </summary>
    [Serializable]
    public class GameplayConfiguration : ConfigurationData
    {
        /// <summary>
        /// Mode to run with.
        /// </summary>
        public RunMode RunningMode;

        /// <summary>
        /// Show prompt for nicknaming the 
        /// </summary>
        public bool ShowNicknameDialog;

        /// <summary>
        /// Auto send the monsters to storage.
        /// </summary>
        public bool AutoSendToStorage;

        /// <summary>
        /// Auto save when teleporting?
        /// </summary>
        public bool AutoSaveOnTeleporting;

        /// <summary>
        /// Auto save on story points?
        /// </summary>
        public bool AutoSaveOnStory;

        /// <summary>
        /// Skip the main menu when opening the game?
        /// </summary>
        public bool SkipMainMenuWhenOpeningGame;

        /// <summary>
        /// Formats saves can be saved on.
        /// </summary>
        public SaveFormat SavegameFormat;

        /// <summary>
        /// Modes the player can use to run.
        /// </summary>
        public enum RunMode
        {
            Toggle,
            Hold
        }

        /// <summary>
        /// Formats saves can be saved on.
        /// </summary>
        public enum SaveFormat
        {
            SmallJson,
            ReadableJson
        }

        /// <summary>
        /// Clone this data into a new instance of the same type.
        /// </summary>
        /// <typeparam name="TConfigurationData">Type of the configuration.</typeparam>
        /// <returns>The cloned config.</returns>
        protected override TConfigurationData Clone<TConfigurationData>() =>
            new GameplayConfiguration
            {
                RunningMode = RunningMode,
                ShowNicknameDialog = ShowNicknameDialog,
                AutoSendToStorage = AutoSendToStorage,
                AutoSaveOnTeleporting = AutoSaveOnTeleporting,
                AutoSaveOnStory = AutoSaveOnStory,
                SkipMainMenuWhenOpeningGame = SkipMainMenuWhenOpeningGame,
                SavegameFormat = SavegameFormat
            } as TConfigurationData;
    }
}