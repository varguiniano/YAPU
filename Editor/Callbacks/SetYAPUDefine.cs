using UnityEditor;
using WhateverDevs.Core.Editor.Utils;

namespace Varguiniano.YAPU.Editor.Callbacks
{
    /// <summary>
    /// Class that sets the YAPU scripting define when the editor is loaded.
    /// </summary>
    [InitializeOnLoad]
    public static class SetYAPUDefine
    {
        /// <summary>
        /// Set the define.
        /// </summary>
        static SetYAPUDefine() => ScriptingDefines.SetDefine("YAPU", true);
    }
}