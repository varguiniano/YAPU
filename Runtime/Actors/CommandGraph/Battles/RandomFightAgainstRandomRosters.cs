using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Runtime.Common;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// Actor command to run a random fight against one or two random rosters.
    /// </summary>
    [Serializable]
    public class RandomFightAgainstRandomRosters : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Configuration for this command.
        /// </summary>
        [SerializeField]
        private RandomFightAgainstRandomRostersConfig Configuration;

        /// <summary>
        /// Commands run when the player defeats the opponents.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnDefeatedOpponents;

        /// <summary>
        /// Play the battle.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            BattleResultParameters result = null;

            BattleTemplate template = Random.value <= Configuration.SingleBattleChance
                                          ? Configuration.SingleBattleTemplate
                                          : Random.value < .5f
                                              ? Configuration.OneVSTwoDoubleBattleTemplate
                                              : Configuration.TwoVSTwoDoubleBattleTemplate;

            byte minLevel = (byte) Random.Range(1, 101);
            byte maxLevel = (byte) Mathf.Clamp(minLevel + 10, 1, 100);

            for (int i = 0; i < template.Rosters.Length; i++)
            {
                Roster roster = template.Rosters[i];
                roster.Database = parameterData.MonsterDatabase;
                roster.Settings = parameterData.YAPUSettings;

                // The first roster must have at least 2 monsters if it's a double battle of 1 vs 2 and it's not using the player roster.
                int minMonsters =
                    i == 0 && template.Rosters.Length == 3 && template == Configuration.OneVSTwoDoubleBattleTemplate
                        ? 2
                        : 1;

                roster.PopulateRandomly(parameterData.YAPUSettings,
                                        minMonsters,
                                        minLevel,
                                        maxLevel,
                                        Configuration.CustomMonsterPool);
            }

            foreach (Bag bag in template.Bags)
                bag.PopulateRandomlyForBattle(Configuration.MaxTypesOfItemsPerBag,
                                              Configuration.MaxNumberOfSingleItem,
                                              parameterData.MonsterDatabase);

            // Also randomize scenario and encounter.
            template.Scenario = Configuration.PossibleScenarios.Random();
            template.Weather = Configuration.PossibleWeathers.Random();

            template.EncounterType =
                WhateverDevs.Core.Runtime.Common.Utils.GetAllItems<EncounterType>().ToList().Random();

            if (actorCharacter != null)
                parameterData.BattleLauncher.LaunchBattle(parameterData.PlayerCharacter,
                                                          template,
                                                          battleResult => result = battleResult);

            yield return new WaitWhile(() => result == null);

            // ReSharper disable once PossibleNullReferenceException
            if (!result.PlayerWon || OnDefeatedOpponents == null) yield break;

            yield return OnDefeatedOpponents.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "On Player Won"
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
                case 0: NextCommand = child; break;
                case 1: OnDefeatedOpponents = child; break;
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
                case 0 when NextCommand == child: NextCommand = null; break;
                case 1 when OnDefeatedOpponents == child: OnDefeatedOpponents = null; break;
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
            if (OnDefeatedOpponents != null) children.Add(OnDefeatedOpponents);

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
                OnDefeatedOpponents
            };
    }
}