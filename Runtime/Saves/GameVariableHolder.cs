using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using WhateverDevs.Core.Runtime.Serialization;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Base class for classes that can hold a set of game variables.
    /// </summary>
    public abstract class GameVariableHolder : SavableObject
    {
        /// <summary>
        /// Set of game variables.
        /// </summary>
        [FoldoutGroup("Game Variables")]
        [HideLabel]
        public GameVariables GameVariables;

        /// <summary>
        /// Get a variable of the given type with the given reference.
        /// </summary>
        /// <param name="variable">Variable reference.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(GameVariableReference variable) => GetVariable<T>(variable.VariableName);

        /// <summary>
        /// Get a variable of the given type with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to get.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(string variableName) => GameVariables.GetVariable<T>(variableName);

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="variable">Reference of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(GameVariableReference variable, T value) =>
            SetVariable(variable.VariableName, value);

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(string variableName, T value) => GameVariables.SetVariable(variableName, value);

        /// <summary>
        /// Get all available variable types.
        /// </summary>
        /// <returns>A list of all types.</returns>
        public List<Type> GetAllVariableTypes() => GameVariables.GetAllTypes();

        /// <summary>
        /// Get all the variable names of a type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>A list of all the variable names.</returns>
        public List<string> GetAllVariableNames(Type type) => GameVariables.GetAllNames(type);

        /// <summary>
        /// Save the data to a persistable string.
        /// </summary>
        /// <param name="serializer">Serializer to be used to save to strings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>A serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(GameVariables);

        /// <summary>
        /// Load the object from a persistable text.
        /// Going through the variables one by one ensures that new variables that have been added are not removed completely.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings"></param>
        /// <param name="monsterDatabase"></param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            GameVariables newGameVariables = serializer.From<GameVariables>(data);

            yield return WaitAFrame;

            foreach (GameVariable<bool> gameVariable in GameVariables.BoolVariables)
                gameVariable.Value = newGameVariables.GetVariable<bool>(gameVariable.Name);

            yield return WaitAFrame;

            foreach (GameVariable<int> gameVariable in GameVariables.IntVariables)
                gameVariable.Value = newGameVariables.GetVariable<int>(gameVariable.Name);
        }

        /// <summary>
        /// Reset the data to its default values.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            GameVariables.ResetVariables();
            yield break;
        }
    }
}