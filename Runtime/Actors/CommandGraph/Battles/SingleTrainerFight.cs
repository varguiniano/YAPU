using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// Command for fighting a single trainer.
    /// </summary>
    [Serializable]
    public class SingleTrainerFight : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Roster the trainer will use.
        /// </summary>
        [SerializeField]
        private Roster Roster;

        /// <summary>
        /// Bag the trainer will use.
        /// </summary>
        [SerializeField]
        private Bag Bag;

        /// <summary>
        /// Type of encounter this will be.
        /// </summary>
        [SerializeField]
        private EncounterType EncounterType;

        /// <summary>
        /// Localization key for the message shown when the trainer is defeated.
        /// </summary>
        [SerializeField]
        private string DefeatLocalizationKey;

        /// <summary>
        /// Respawn the player if they lose?
        /// </summary>
        [SerializeField]
        [InfoBox("On Lost commands will only be run if this is false.")]
        private bool RespawnThePlayerIfTheyLose = true;

        /// <summary>
        /// Commands run when the player wins.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnWin;

        /// <summary>
        /// Commands run when the player loses.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnLost;

        /// <summary>
        /// Run the battle.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            BattleResultParameters result = null;

            if (actorCharacter != null)
                parameterData.BattleLauncher.LaunchSingleTrainerEncounter(parameterData.PlayerCharacter,
                                                                          Roster,
                                                                          Bag,
                                                                          actorCharacter.CharacterController
                                                                             .GetCharacterData(),
                                                                          actorCharacter.CharacterController.CurrentGrid
                                                                             .SceneInfo,
                                                                          parameterData.PlayerCharacter.CurrentWeather
                                                                       != null
                                                                              ? parameterData.PlayerCharacter
                                                                                 .CurrentWeather
                                                                                 .InBattleWeather
                                                                              : null,
                                                                          EncounterType,
                                                                          DefeatLocalizationKey,
                                                                          RespawnThePlayerIfTheyLose,
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

            // ReSharper disable once PossibleNullReferenceException
            if (result.PlayerWon && OnWin != null)
                yield return OnWin.RunCommandAndContinue(parameterData);
            else if (!RespawnThePlayerIfTheyLose && OnLost != null)
                yield return OnLost.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "On Player Won",
                "On Player Lost"
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
                    OnWin = child;
                    break;
                case 2:
                    OnLost = child;
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
                case 1 when OnWin == child:
                    OnWin = null;
                    break;
                case 2 when OnLost == child:
                    OnLost = null;
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
            if (OnWin != null) children.Add(OnWin);
            if (OnLost != null) children.Add(OnLost);

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
                OnWin,
                OnLost
            };
    }
}