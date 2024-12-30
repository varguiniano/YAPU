using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to teleport the player to another location.
    /// </summary>
    [Serializable]
    public class TeleportPlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Location to teleport to.
        /// </summary>
        [SerializeField]
        private SceneLocation SceneLocation;

        /// <summary>
        /// Teleport the player to that location.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.LockInput();
            CoroutineRunner.Instance.StartCoroutine(parameterData.Teleporter.TeleportPlayer(SceneLocation));
            parameterData.Callback.Invoke(new CommandCallbackParams {StopFurtherPlayerMovement = true});
            yield break;
        }
    }
}