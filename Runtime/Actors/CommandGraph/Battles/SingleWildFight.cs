using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// Command to trigger a fight against a wild monster.
    /// </summary>
    [Serializable]
    public class SingleWildFight : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Type of encounter this will be.
        /// </summary>
        [InfoBox("Roster is retrieved from the character controller.")]
        [SerializeField]
        private EncounterType EncounterType;

        /// <summary>
        /// Commands run when the player defeats the wild monster.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnDefeatedWild;

        /// <summary>
        /// Commands run when the player catches the wild monster.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnCaughtWild;

        /// <summary>
        /// Play the battle.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            BattleResultParameters result = null;

            if (actorCharacter != null)
                parameterData.BattleLauncher.LaunchSingleWildEncounter(parameterData.PlayerCharacter,
                                                                       actorCharacter.CharacterController.MonsterRoster,
                                                                       actorCharacter.CharacterController.CurrentGrid
                                                                          .SceneInfo,
                                                                       parameterData.PlayerCharacter.CurrentWeather
                                                                    != null
                                                                           ? parameterData.PlayerCharacter
                                                                              .CurrentWeather
                                                                              .InBattleWeather
                                                                           : null,
                                                                       EncounterType,
                                                                       resultParameters =>
                                                                       {
                                                                           result = resultParameters;

                                                                           if (result.PlayerWon)
                                                                               parameterData.PlayerCharacter
                                                                                  .CharacterController.CurrentGrid
                                                                                  .PlayerEnterGrid(parameterData
                                                                                          .PlayerCharacter,
                                                                                       true);
                                                                       });

            yield return new WaitWhile(() => result == null);

            // ReSharper disable thrice PossibleNullReferenceException
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (result.PlayerWon && result.CapturedMonster == null && OnDefeatedWild != null)
                yield return OnDefeatedWild.RunCommandAndContinue(parameterData);
            else if (result.PlayerWon && result.CapturedMonster != null && OnCaughtWild != null)
                yield return OnCaughtWild.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "On Defeated Wild",
                "On Caught Wild"
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
                    OnDefeatedWild = child;
                    break;
                case 2:
                    OnCaughtWild = child;
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
                case 1 when OnDefeatedWild == child:
                    OnDefeatedWild = null;
                    break;
                case 2 when OnCaughtWild == child:
                    OnCaughtWild = null;
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
            if (OnDefeatedWild != null) children.Add(OnDefeatedWild);
            if (OnCaughtWild != null) children.Add(OnCaughtWild);

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
                OnDefeatedWild,
                OnCaughtWild
            };
    }
}