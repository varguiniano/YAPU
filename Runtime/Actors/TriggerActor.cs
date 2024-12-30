using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Characters;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Actor that may or may not have graphics that has events that get triggered when the player goes through them.
    /// </summary>
    public class TriggerActor : GridSubscribingActor
    {
        /// <summary>
        /// Triggers don't block movement.
        /// </summary>
        public override bool BlocksMovement => false;

        /// <summary>
        /// Commands that are run before the player enters the trigger.
        /// </summary>
        [FoldoutGroup("Trigger")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run before the player enters the trigger.")]
        private ActorCommandGraph OnPlayerAboutToEnterCommandGraph;

        /// <summary>
        /// Does this actor have about to enter triggers?
        /// </summary>
        public bool HasAboutToEnterTriggers => OnPlayerAboutToEnterCommandGraph is {IsGraphEmpty: false};

        /// <summary>
        /// Commands that run when the player enters the trigger.
        /// </summary>
        [FoldoutGroup("Trigger")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run when the player enters the trigger.")]
        private ActorCommandGraph OnPlayerEnteredCommandGraph;

        /// <summary>
        /// Does this actor has triggers when the player enters?
        /// </summary>
        public bool HasEnteredTriggers => OnPlayerEnteredCommandGraph is {IsGraphEmpty: false};

        /// <summary>
        /// Commands that run when the player is about to be teleported here.
        /// </summary>
        [FoldoutGroup("Trigger")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run just before the player is teleported here.")]
        private ActorCommandGraph OnPlayerAboutToBeTeleportedCommandGraph;

        /// <summary>
        /// Commands that run when the player is teleported here.
        /// </summary>
        [FoldoutGroup("Trigger")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run when the player is teleported here.")]
        private ActorCommandGraph OnPlayerTeleportedCommandGraph;

        /// <summary>
        /// Commands that run when an actor is pushed inside this trigger.
        /// </summary>
        [FoldoutGroup("Trigger")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands are run when an actor is pushed inside this trigger.")]
        private ActorCommandGraph OnActorPushedInsideCommandGraph;

        /// <summary>
        /// Player about to enter.
        /// </summary>
        /// <param name="playerCharacter">Player entering.</param>
        /// <param name="playerDirection">Player direction.</param>
        /// <param name="callback">Callback that can be used to stop the player from further movement.</param>
        public IEnumerator PlayerAboutToEnter(PlayerCharacter playerCharacter,
                                              CharacterController.Direction playerDirection,
                                              Action<bool> callback)
        {
            yield return RunCommands(playerCharacter,
                                     playerDirection,
                                     OnPlayerAboutToEnterCommandGraph,
                                     (stop, _) => callback.Invoke(stop));
        }

        /// <summary>
        /// Player entered.
        /// </summary>
        /// <param name="playerCharacter">Player entering.</param>
        /// <param name="playerDirection">Player direction.</param>
        public IEnumerator PlayerEntered(PlayerCharacter playerCharacter,
                                         CharacterController.Direction playerDirection)
        {
            yield return RunCommands(playerCharacter, playerDirection, OnPlayerEnteredCommandGraph);
        }

        /// <summary>
        /// Player about to be teleported.
        /// </summary>
        /// <param name="playerCharacter">Player entering.</param>
        public IEnumerator PlayerAboutToBeTeleported(PlayerCharacter playerCharacter)
        {
            yield return RunCommands(playerCharacter,
                                     playerCharacter.CharacterController.CurrentDirection,
                                     OnPlayerAboutToBeTeleportedCommandGraph);
        }

        /// <summary>
        /// Player teleported.
        /// </summary>
        /// <param name="playerCharacter">Player entering.</param>
        public IEnumerator PlayerTeleported(PlayerCharacter playerCharacter)
        {
            yield return RunCommands(playerCharacter,
                                     playerCharacter.CharacterController.CurrentDirection,
                                     OnPlayerTeleportedCommandGraph);
        }

        /// <summary>
        /// An actor was pushed inside.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="actor">Actor pushed.</param>
        /// <param name="callback">Callback used to destroy the pushed actor.</param>
        public IEnumerator ActorPushedInside(PlayerCharacter playerCharacter,
                                             PushableActor actor,
                                             Action<bool> callback)
        {
            yield return RunCommands(playerCharacter,
                                     playerCharacter.CharacterController.CurrentDirection,
                                     OnActorPushedInsideCommandGraph,
                                     (_, destroy) => callback.Invoke(destroy),
                                     actor);
        }

        /// <summary>
        /// run the given list of commands.
        /// </summary>
        /// <param name="playerCharacter">Player entering.</param>
        /// <param name="playerDirection">Player direction.</param>
        /// <param name="commands">Commands to run.</param>
        /// <param name="callback">Callback that can be used to stop the player from further movement or to destroy the pushed actor.</param>
        /// <param name="pushableActor">If an actor was pushed inside this trigger, the reference to it.</param>
        private IEnumerator RunCommands(PlayerCharacter playerCharacter,
                                        CharacterController.Direction playerDirection,
                                        ActorCommandGraph commands,
                                        Action<bool, bool> callback = null,
                                        PushableActor pushableActor = null)
        {
            Vector3Int interactPosition = Position;

            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            yield return new WaitWhile(() => LoopRunning);

            if (Position == interactPosition)
            {
                GlobalGridManager.StopAllActors = true;

                if (commands != null)
                    yield return commands.Run(new CommandParameterData(gameObject,
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
                                                                               wasLooping = callbackParams.NewLooping;

                                                                           if (callbackParams.StopFurtherPlayerMovement)
                                                                               callback?.Invoke(true, false);

                                                                           if (callbackParams.DestroyPushedActor)
                                                                               callback?.Invoke(false, true);
                                                                       },
                                                                       extraParams: pushableActor != null
                                                                           ? new ICommandParameter[] {pushableActor}
                                                                           : Array.Empty<ICommandParameter>()));

                GlobalGridManager.StopAllActors = false;
            }

            RunningCommands = false;

            ShouldLoop = wasLooping;
        }
    }
}