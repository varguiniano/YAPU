using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;
using Move = Varguiniano.YAPU.Runtime.MonsterDatabase.Moves.Move;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Actor controller for scene elements that can be targeted with a move.
    /// </summary>
    public class ActorMoveTarget : PushableActor
    {
        /// <summary>
        /// Move to use to trigger the command graph.
        /// </summary>
        [FoldoutGroup("Moves")]
        [SerializeField]
        private Move Move;

        /// <summary>
        /// Commands that are triggered when the move is used.
        /// </summary>
        [FoldoutGroup("Moves")]
        [SerializeReference]
        [SerializeField]
        [InfoBox("These commands are run when the player uses the move looking at this actor.")]
        protected ActorCommandGraph MoveUsedCommandGraph;

        /// <summary>
        /// Check if a move is compatible with this actor.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if it is.</returns>
        public bool IsMoveCompatible(Move move) => Move == move;

        /// <summary>
        /// Use a move on this actor.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="playerDirection">Direction the player is at.</param>
        /// <param name="monster">The monster using the move.</param>
        /// <param name="move">The move being used.</param>
        public IEnumerator UseMove(PlayerCharacter playerCharacter,
                                   CharacterController.Direction playerDirection,
                                   MonsterInstance monster,
                                   Move move)
        {
            Vector3Int interactPosition = Position;

            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            yield return new WaitWhile(() => LoopRunning);

            if (Position == interactPosition)
            {
                GlobalGridManager.StopAllActors = true;

                if (move == Move)
                    yield return MoveUsedCommandGraph.Run(new CommandParameterData(gameObject,
                                                              this,
                                                              playerDirection,
                                                              playerCharacter,
                                                              GlobalGameData,
                                                              Settings,
                                                              Database,
                                                              BattleLauncher,
                                                              Localizer,
                                                              ConfigurationManager,
                                                              InputManager,
                                                              PlayerTeleporter,
                                                              SavegameManager,
                                                              MapSceneLauncher,
                                                              TimeManager,
                                                              QuestManager,
                                                              TradeManager,
                                                              callbackParams =>
                                                              {
                                                                  if (callbackParams.UpdateLooping)
                                                                      wasLooping =
                                                                          callbackParams.NewLooping;
                                                              },
                                                              usingMove: true,
                                                              moveUser: monster,
                                                              move: move));

                GlobalGridManager.StopAllActors = false;
            }

            RunningCommands = false;

            ShouldLoop = wasLooping;
        }
    }
}