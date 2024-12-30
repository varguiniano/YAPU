using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;

namespace Varguiniano.YAPU.Editor.Actors.CommandGraph
{
    /// <summary>
    /// Class that wraps the inspector for a command node so it can be displayed in the inspector.
    /// </summary>
    [Serializable]
    [HideMonoScript]
    public class CommandNodeInspectorWrapper : ScriptableObject
    {
        /// <summary>
        /// Node to inspect.
        /// </summary>
        [SerializeReference]
        [InlineProperty(LabelWidth = 200)]
        [HideLabel]
        [UsedImplicitly]
        [HideReferenceObjectPicker]
        public CommandNode Node;
    }
}