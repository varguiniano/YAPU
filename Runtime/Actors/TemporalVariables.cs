using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Storage that allows actors to store variables that can be used across multiple commands.
    /// These, however, won't be saved with the game.
    /// </summary>
    public class TemporalVariables : WhateverBehaviour<TemporalVariables>
    {
        /// <summary>
        /// Bool variables stored in this behaviour.
        /// </summary>
        private readonly Dictionary<string, bool> boolVariables = new();

        /// <summary>
        /// Int variables stored in this behaviour.
        /// </summary>
        private readonly Dictionary<string, int> intVariables = new();

        /// <summary>
        /// Float variables stored in this behaviour.
        /// </summary>
        private readonly Dictionary<string, float> floatVariables = new();

        /// <summary>
        /// String variables stored in this behaviour.
        /// </summary>
        private readonly Dictionary<string, string> stringVariables = new();

        /// <summary>
        /// GameObject variables stored in this behaviour.
        /// </summary>
        private readonly Dictionary<string, GameObject> gameObjectVariables = new();

        /// <summary>
        /// Get a variable of the given type with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to get.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(string variableName)
        {
            if (typeof(T) == typeof(bool))
                if (boolVariables.TryGetValue(variableName, out bool variable))
                    return (T) Convert.ChangeType(variable, typeof(T));
                else
                {
                    Logger.Warn("Variable " + variableName + " not found, returning default value.");
                    return default;
                }

            if (typeof(T) == typeof(int))
                if (intVariables.TryGetValue(variableName, out int variable))
                    return (T) Convert.ChangeType(variable, typeof(T));
                else
                {
                    Logger.Warn("Variable " + variableName + " not found, returning default value.");
                    return default;
                }

            if (typeof(T) == typeof(float))
                if (floatVariables.TryGetValue(variableName, out float variable))
                    return (T) Convert.ChangeType(variable, typeof(T));
                else
                {
                    Logger.Warn("Variable " + variableName + " not found, returning default value.");
                    return default;
                }

            if (typeof(T) == typeof(string))
                if (stringVariables.TryGetValue(variableName, out string variable))
                    return (T) Convert.ChangeType(variable, typeof(T));
                else
                {
                    Logger.Warn("Variable " + variableName + " not found, returning default value.");
                    return default;
                }

            if (typeof(T) == typeof(GameObject))
                if (gameObjectVariables.TryGetValue(variableName, out GameObject variable))
                    return (T) Convert.ChangeType(variable, typeof(T));
                else
                {
                    Logger.Warn("Variable " + variableName + " not found, returning default value.");
                    return default;
                }

            Logger.Error("The variable type " + typeof(T) + " is not supported.");

            return default;
        }

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(string variableName, T value)
        {
            if (typeof(T) == typeof(bool))
            {
                boolVariables[variableName] = (bool) Convert.ChangeType(value, typeof(T));
                return;
            }

            if (typeof(T) == typeof(int))
            {
                intVariables[variableName] = (int) Convert.ChangeType(value, typeof(T));
                return;
            }

            if (typeof(T) == typeof(float))
            {
                floatVariables[variableName] = (float) Convert.ChangeType(value, typeof(T));
                return;
            }

            if (typeof(T) == typeof(string))
            {
                stringVariables[variableName] = (string) Convert.ChangeType(value, typeof(T));
                return;
            }

            if (typeof(T) == typeof(GameObject))
            {
                gameObjectVariables[variableName] = (GameObject) Convert.ChangeType(value, typeof(T));
                return;
            }

            Logger.Error("The variable type " + typeof(T) + " is not supported.");
        }
    }
}