using System.Linq;
using UnityEditor;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.MoveMachines;
using WhateverDevs.Core.Editor.Utils;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Utility to open the move machine generator.
    /// </summary>
    public static class MoveMachineGeneratorMenuOption
    {
        /// <summary>
        /// Utility to open the move machine generator.
        /// </summary>
        [MenuItem("YAPU/Move Machine Creator")]
        private static void OpenGenerator() =>
            Selection.activeObject = AssetManagementUtils.FindAssetsByType<MoveMachineCreator>().First();
    }
}