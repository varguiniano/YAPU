using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to make the character look in a direction.
    /// </summary>
    [Serializable]
    public class Look : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Direction to look at.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(Random))]
        [InfoBox("Direction shouldn't be None.",
                 InfoMessageType.Error,
                 VisibleIf = "@ Direction == Varguiniano.YAPU.Runtime.Characters.CharacterController.Direction.None")]
        private CharacterController.Direction Direction;

        /// <summary>
        /// Look in a random direction?
        /// </summary>
        [SerializeField]
        private bool Random;

        /// <summary>
        /// Have the own actor look?
        /// </summary>
        [SerializeField]
        private bool OwnCharacter = true;

        /// <summary>
        /// Character to look.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(OwnCharacter))]
        private CharacterController TargetCharacter;

        /// <summary>
        /// Look in a direction.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (OwnCharacter && actorCharacter == null) yield break;

            // ReSharper disable once PossibleNullReferenceException
            CharacterController character = OwnCharacter ? actorCharacter.CharacterController : TargetCharacter;

            CharacterController.Direction direction = Direction;

            if (Random)
                do
                    direction = WhateverDevs.Core.Runtime.Common.Utils.GetAllItems<CharacterController.Direction>()
                                            .ToList()
                                            .Random();
                while (direction == CharacterController.Direction.None);

            character.LookAt(direction, useBikingSprite: character.IsBiking);
        }
    }
}