using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Sounds;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to make the sprite of the actor animate by changing its sprites.
    /// </summary>
    [Serializable]
    public class CharacterSpriteChangeAnimation : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Animation interval.
        /// </summary>
        [SerializeField]
        private float Interval = .1f;

        /// <summary>
        /// Sprites to use in the animation.
        /// </summary>
        [SerializeField]
        [PreviewField]
        private List<Sprite> Sprites;

        /// <summary>
        /// Sound to play each animation.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private PlaySound Sound;

        /// <summary>
        /// Animate the sprite.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter == null)
            {
                Logger.Warn("This command will only run on actor characters.");
                yield break;
            }

            WaitForSeconds interval = new(Interval);

            foreach (Sprite sprite in Sprites)
            {
                if (Sound != null) yield return Sound.RunCommandAndContinue(parameterData);

                actorCharacter.CharacterController.ChangeSprite(sprite);
                yield return interval;
            }
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "Sound"
            };

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0:
                    NextCommand = child;
                    break;
                case 1:
                    Sound = (PlaySound) child;
                    break;
            }
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void RemoveChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0 when NextCommand == child:
                    NextCommand = null;
                    break;
                case 1 when Sound == child:
                    Sound = null;
                    break;
            }
        }

        /// <summary>
        /// Get all the children of this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetChildren()
        {
            List<CommandNode> children = new();

            if (NextCommand != null) children.Add(NextCommand);
            if (Sound != null) children.Add(Sound);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                NextCommand,
                Sound
            };
    }
}