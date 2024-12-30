using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Sounds
{
    /// <summary>
    /// Play the monster cry of the monster set in the character controller.
    /// </summary>
    [Serializable]
    public class PlayMonsterCry : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Play the cry.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter == null
             || actorCharacter.CharacterController.Mode != CharacterController.CharacterMode.Monster)
                yield break;

            bool isAudioPlaying = false;

            yield return AudioManager.Instance.IsAudioPlaying(actorCharacter.CharacterController.GetMonsterData()
                                                                            .FormData.Cry,
                                                              isPlaying => isAudioPlaying = isPlaying);

            if (!isAudioPlaying)
                AudioManager.Instance.PlayAudio(actorCharacter.CharacterController.GetMonsterData().FormData.Cry);
        }
    }
}