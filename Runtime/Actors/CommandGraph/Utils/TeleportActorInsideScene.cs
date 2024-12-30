using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.World;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to teleport an actor inside the current scene.
    /// </summary>
    [Serializable]
    public class TeleportActorInsideScene : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Target actor.
        /// </summary>
        [InfoBox("This command doesn't check movement restrictions or if the target tile is occupied. Use with caution.",
                 InfoMessageType.Warning)]
        [SerializeField]
        private ActorCharacter Target;

        /// <summary>
        /// Target position to teleport to.
        /// </summary>
        [SerializeField]
        private Vector3Int TargetPosition;

        /// <summary>
        /// Teleport the actor.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            GridController grid = Target.GetCurrentGrid();
            Vector3Int currentPosition = Target.Position;

            grid.CharacterAboutToLeaveTileAsync(Target.CharacterController, currentPosition);
            grid.CharacterAboutToEnterTileAsync(Target.CharacterController, TargetPosition);

            if (grid.HasBlockingInteraction(currentPosition))
                yield return grid.CharacterAboutToLeaveTile(Target.CharacterController,
                                                            currentPosition,
                                                            _ =>
                                                            {
                                                            });

            if (grid.HasBlockingInteraction(TargetPosition))
                yield return grid.CharacterAboutToEnterTile(Target.CharacterController,
                                                            TargetPosition,
                                                            (_, _) =>
                                                            {
                                                            });

            Target.transform.position = TargetPosition;

            grid.CharacterLeftTileAsync(Target.CharacterController, currentPosition);
            grid.CharacterEnterTileAsync(Target.CharacterController, TargetPosition);

            // These calls generate a noticeable frame delay with continuous movement.
            if (grid.HasBlockingInteraction(currentPosition))
                yield return grid.CharacterLeftTile(Target.CharacterController,
                                                    currentPosition,
                                                    _ =>
                                                    {
                                                    });

            if (grid.HasBlockingInteraction(TargetPosition))
                yield return grid.CharacterEnterTile(Target.CharacterController,
                                                     TargetPosition,
                                                     _ =>
                                                     {
                                                     });
        }
    }
}