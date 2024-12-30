using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Characters;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Base class for actors that can be pushed.
    /// </summary>
    public class PushableActor : ActorCharacter
    {
        /// <summary>
        /// Commands that are triggered when it gets pushed by the player.
        /// </summary>
        [FoldoutGroup("Push")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run when the player is strong and pushes this actor.")]
        protected ActorCommandGraph PushCommandGraph;

        /// <summary>
        /// Push the actor.
        /// </summary>
        /// <param name="playerCharacter">Player pushing.</param>
        /// <param name="playerDirection">Player direction.</param>
        public IEnumerator Push(PlayerCharacter playerCharacter, CharacterController.Direction playerDirection)
        {
            Vector3Int interactPosition = Position;

            bool destroy = false;

            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            yield return new WaitWhile(() => LoopRunning);

            if (Position == interactPosition)
            {
                GlobalGridManager.StopAllActors = true;

                yield return PushCommandGraph.Run(new CommandParameterData(gameObject,
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
                                                                           }));

                foreach (TriggerActor trigger in GetCurrentGrid().GetListOfActorTypesInPosition<TriggerActor>(Position))
                    yield return trigger.ActorPushedInside(playerCharacter,
                                                           this,
                                                           shouldDestroy => destroy = shouldDestroy);

                GlobalGridManager.StopAllActors = false;
            }

            RunningCommands = false;

            ShouldLoop = wasLooping;

            if (destroy) Destroy(gameObject);
        }
    }
}