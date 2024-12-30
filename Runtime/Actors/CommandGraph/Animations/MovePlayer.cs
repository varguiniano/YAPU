using System;
using System.Collections;
using UnityEngine;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Actor command to move the player.
    /// </summary>
    [Serializable]
    public class MovePlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Direction to move towards.
        /// </summary>
        [SerializeField]
        private CharacterController.Direction Direction;

        /// <summary>
        /// Number of tiles to move in that direction.
        /// </summary>
        [SerializeField]
        private uint NumberOfTiles = 1;

        /// <summary>
        /// Wait for the next tile to be free?
        /// </summary>
        [SerializeField]
        private bool WaitForNextTileToBeFree;

        /// <summary>
        /// Move the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.InputManager.BlockInput();

            for (int i = 0; i < NumberOfTiles; ++i)
                yield return parameterData.PlayerCharacter.CharacterController.Move(Direction,
                    waitForNextTileToBeFree: WaitForNextTileToBeFree);

            parameterData.PlayerCharacter.CharacterController.LookAt(Direction,
                                                                     useBikingSprite: parameterData.PlayerCharacter
                                                                        .CharacterController.IsBiking);

            yield return new WaitForEndOfFrame();

            parameterData.InputManager.BlockInput(false);
        }
    }
}