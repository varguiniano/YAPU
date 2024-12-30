using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dex
{
    /// <summary>
    /// Command to give the dex to the player.
    /// </summary>
    [Serializable]
    public class GiveDex : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Audio to play when obtained.
        /// </summary>
        [SerializeField]
        private AudioReference ObtainedAudio;

        /// <summary>
        /// Give the dex to the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            AudioManager.Instance.PlayAudio(ObtainedAudio);

            parameterData.PlayerCharacter.GlobalGameData.HasDex = true;

            yield return DialogManager.ShowDialogAndWait("Dialogs/ObtainedDex");
        }
    }
}