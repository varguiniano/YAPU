using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Class that stores a reference to a game variable.
    /// </summary>
    [Serializable]
    public class GameVariableReference
    {
        /// <summary>
        /// Reference to the holder of the variable.
        /// </summary>
        [InfoBox("Choose the appropriate scene or the global variable storage.",
                 VisibleIf = "@" + nameof(VariableHolder) + " ==  null")]
        public GameVariableHolder VariableHolder;

        /// <summary>
        /// Type of the variable.
        /// </summary>
        [ShowIf("@" + nameof(VariableHolder) + " !=  null")]
        [ValueDropdown(nameof(GetValueTypes))]
        public string VariableType = "";

        /// <summary>
        /// Name of the variable.
        /// </summary>
        [ShowIf("@"
              + nameof(VariableHolder)
              + " !=  null && "
              + nameof(VariableType)
              + " != null && "
              + nameof(VariableType)
              + " != \"\"")]
        [ValueDropdown(nameof(GetValueNames))]
        public string VariableName = "";

        /// <summary>
        /// Type of the variable.
        /// </summary>
        public Type VariableTypeClass => ReconstructTypeFromString(VariableType);

        /// <summary>
        /// Reset the reference.
        /// </summary>
        [Button]
        private void ResetReference()
        {
            VariableHolder = null;
            VariableType = "";
            VariableName = "";
        }

        /// <summary>
        /// Get the available value types.
        /// </summary>
        /// <returns>A list of types.</returns>
        private List<string> GetValueTypes() =>
            VariableHolder == null
                ? new List<string>()
                : VariableHolder.GetAllVariableTypes().Select(type => type.ToString()).ToList();

        /// <summary>
        /// Get the names of all variables with the type.
        /// </summary>
        /// <returns>A list of strings.</returns>
        private List<string> GetValueNames() =>
            VariableType.IsNullEmptyOrWhiteSpace()
                ? new List<string>()
                : VariableHolder.GetAllVariableNames(ReconstructTypeFromString(VariableType));

        /// <summary>
        /// Reconstruct the type class from a string name.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <returns>Its type class.</returns>
        private Type ReconstructTypeFromString(string typeName) =>
            VariableHolder == null
                ? null
                : VariableHolder.GetAllVariableTypes().FirstOrDefault(type => type.ToString() == typeName);
    }
}