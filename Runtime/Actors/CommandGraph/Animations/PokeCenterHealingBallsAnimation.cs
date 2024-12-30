using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Sounds;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to play the poke center healing balls animation.
    /// </summary>
    [Serializable]
    public class PokeCenterHealingBallsAnimation : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Animations to play on when ball placing.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CharacterSpriteChangeAnimation[] BallPlaceAnimations = new CharacterSpriteChangeAnimation[6];

        /// <summary>
        /// Animations to play on when ball placing.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CharacterSpriteChangeAnimation[] BallHealAnimations = new CharacterSpriteChangeAnimation[6];

        /// <summary>
        /// Command to play the healing sound.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private PlaySound PlayHealingSound;

        /// <summary>
        /// Play the animation.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            int numberOfMons = parameterData.PlayerCharacter.PlayerRoster.RosterSize;

            yield return BallPlaceAnimations[numberOfMons - 1].RunCommandAndContinue(parameterData);

            yield return PlayHealingSound.RunCommandAndContinue(parameterData);

            for (int i = 0; i < 3; ++i)
                yield return BallHealAnimations[numberOfMons - 1].RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the input ports for this node.
        /// By default, it has one for execution in.
        /// </summary>
        public override List<string> GetInputPorts() => new() {""};

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "BallPlace0",
                "BallPlace1",
                "BallPlace2",
                "BallPlace3",
                "BallPlace4",
                "BallPlace5",
                "BallHeal0",
                "BallHeal1",
                "BallHeal2",
                "BallHeal3",
                "BallHeal4",
                "BallHeal5",
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
                    BallPlaceAnimations[0] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 2:
                    BallPlaceAnimations[1] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 3:
                    BallPlaceAnimations[2] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 4:
                    BallPlaceAnimations[3] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 5:
                    BallPlaceAnimations[4] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 6:
                    BallPlaceAnimations[5] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 7:
                    BallHealAnimations[0] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 8:
                    BallHealAnimations[1] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 9:
                    BallHealAnimations[2] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 10:
                    BallHealAnimations[3] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 11:
                    BallHealAnimations[4] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 12:
                    BallHealAnimations[5] = (CharacterSpriteChangeAnimation) child;
                    break;
                case 13:
                    PlayHealingSound = (PlaySound) child;
                    break;
            }
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        // ReSharper disable once CyclomaticComplexity
        public override void RemoveChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0 when NextCommand == child:
                    NextCommand = null;
                    break;
                case 1 when BallPlaceAnimations[0] == child:
                    BallPlaceAnimations[0] = null;
                    break;
                case 2 when BallPlaceAnimations[1] == child:
                    BallPlaceAnimations[1] = null;
                    break;
                case 3 when BallPlaceAnimations[2] == child:
                    BallPlaceAnimations[2] = null;
                    break;
                case 4 when BallPlaceAnimations[3] == child:
                    BallPlaceAnimations[3] = null;
                    break;
                case 5 when BallPlaceAnimations[4] == child:
                    BallPlaceAnimations[4] = null;
                    break;
                case 6 when BallPlaceAnimations[5] == child:
                    BallPlaceAnimations[5] = null;
                    break;
                case 7 when BallHealAnimations[0] == child:
                    BallHealAnimations[0] = null;
                    break;
                case 8 when BallHealAnimations[1] == child:
                    BallHealAnimations[1] = null;
                    break;
                case 9 when BallHealAnimations[2] == child:
                    BallHealAnimations[2] = null;
                    break;
                case 10 when BallHealAnimations[3] == child:
                    BallHealAnimations[3] = null;
                    break;
                case 11 when BallHealAnimations[4] == child:
                    BallHealAnimations[4] = null;
                    break;
                case 12 when BallHealAnimations[5] == child:
                    BallHealAnimations[5] = null;
                    break;
                case 13 when PlayHealingSound == child:
                    PlayHealingSound = null;
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

            foreach (CharacterSpriteChangeAnimation animation in BallPlaceAnimations)
                if (animation != null)
                    children.Add(animation);

            foreach (CharacterSpriteChangeAnimation animation in BallHealAnimations)
                if (animation != null)
                    children.Add(animation);

            if (PlayHealingSound != null) children.Add(PlayHealingSound);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren()
        {
            List<CommandNode> children = new() {NextCommand};

            children.AddRange(BallPlaceAnimations);
            children.AddRange(BallHealAnimations);

            children.Add(PlayHealingSound);

            return children;
        }
    }
}