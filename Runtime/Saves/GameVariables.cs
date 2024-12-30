using System;
using System.Collections.Generic;
using System.Linq;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Class that stores game variables. These can belong to a scene or be global to the entire game.
    /// </summary>
    [Serializable]
    public class GameVariables : Loggable<GameVariables>
    {
        /// <summary>
        /// Collection of bool variables.
        /// </summary>
        public List<GameVariable<bool>> BoolVariables = new();

        /// <summary>
        /// Collection of int variables.
        /// </summary>
        public List<GameVariable<int>> IntVariables = new();
        
        /// <summary>
        /// Collection of float variables.
        /// </summary>
        public List<GameVariable<float>> FloatVariables = new();
        
        /// <summary>
        /// Collection of string variables.
        /// </summary>
        public List<GameVariable<string>> StringVariables = new();

        /// <summary>
        /// Get a variable of the given type with the given name.
        /// </summary>
        /// <param name="name">Name of the variable to get.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(string name)
        {
            if (typeof(T) == typeof(bool))
                return (T)Convert.ChangeType(GetVariableFromList(BoolVariables, name), typeof(T));

            if (typeof(T) == typeof(int))
                return (T)Convert.ChangeType(GetVariableFromList(IntVariables, name), typeof(T));
            
            if (typeof(T) == typeof(float))
                return (T)Convert.ChangeType(GetVariableFromList(FloatVariables, name), typeof(T));
            
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(GetVariableFromList(StringVariables, name), typeof(T));

            Logger.Error("The variable type " + typeof(T) + " is not supported.");

            return default;
        }

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="name">Name of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(string name, T value)
        {
            if (typeof(T) == typeof(bool))
            {
                SetVariableOnList(BoolVariables, name, (bool)Convert.ChangeType(value, typeof(bool)));
                return;
            }

            if (typeof(T) == typeof(int))
            {
                SetVariableOnList(IntVariables, name, (int)Convert.ChangeType(value, typeof(int)));
                return;
            }
            
            if (typeof(T) == typeof(float))
            {
                SetVariableOnList(FloatVariables, name, (float)Convert.ChangeType(value, typeof(float)));
                return;
            }
            
            if (typeof(T) == typeof(string))
            {
                SetVariableOnList(StringVariables, name, (string)Convert.ChangeType(value, typeof(string)));
                return;
            }

            Logger.Error("The variable type " + typeof(T) + " is not supported.");
        }

        /// <summary>
        /// Get all available variable types.
        /// </summary>
        /// <returns>A list of all types.</returns>
        public List<Type> GetAllTypes() =>
            new()
            {
                typeof(bool),
                typeof(int),
                typeof(float),
                typeof(string)
            };

        /// <summary>
        /// Get all the variable names of a type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>A list of all the variable names.</returns>
        public List<string> GetAllNames(Type type)
        {
            if (type == typeof(bool)) return BoolVariables.Select(variable => variable.Name).ToList();

            if (type == typeof(int)) return IntVariables.Select(variable => variable.Name).ToList();
            
            if (type == typeof(float)) return FloatVariables.Select(variable => variable.Name).ToList();
            
            if (type == typeof(string)) return StringVariables.Select(variable => variable.Name).ToList();

            Logger.Error("The variable type " + type + " is not supported.");

            return null;
        }

        /// <summary>
        /// Reset all variables to their default values.
        /// </summary>
        public void ResetVariables()
        {
            ResetVariables(BoolVariables);
            ResetVariables(IntVariables);
            ResetVariables(FloatVariables);
            ResetVariables(StringVariables);
        }

        /// <summary>
        /// Get a variable with the given name from the given list.
        /// </summary>
        /// <param name="list">List to check.</param>
        /// <param name="name">Name to check.</param>
        /// <typeparam name="T">Type of variable.</typeparam>
        /// <returns>The variable value.</returns>
        private T GetVariableFromList<T>(IEnumerable<GameVariable<T>> list, string name) =>
            list.Where(variable => variable.Name == name).Select(variable => variable.Value).FirstOrDefault();

        /// <summary>
        /// Set a variable with the given name on the given list.
        /// </summary>
        /// <param name="list">List to check.</param>
        /// <param name="name">Name to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of variable.</typeparam>
        private void SetVariableOnList<T>(IEnumerable<GameVariable<T>> list, string name, T value)
        {
            foreach (GameVariable<T> variable in list)
                if (variable.Name == name)
                    variable.Value = value;
        }

        /// <summary>
        /// Reset all the variables of a list.
        /// </summary>
        /// <param name="list">List to reset.</param>
        /// <typeparam name="T">type of variable.</typeparam>
        private void ResetVariables<T>(IEnumerable<GameVariable<T>> list)
        {
            foreach (GameVariable<T> variable in list) variable.Value = variable.DefaultValue;
        }
    }
}