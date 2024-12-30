using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Player;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Rosters module for the battle manager.
    /// </summary>
    public class BattleManagerRostersModule : BattleManagerModule<BattleManagerRostersModule>
    {
        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        internal Roster PlayerRoster;

        /// <summary>
        /// Rosters of the friendly team.
        /// </summary>
        [FoldoutGroup("Rosters")]
        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        public List<List<Battler>> AllyRosters;

        /// <summary>
        /// Rosters of the enemy team.
        /// </summary>
        [FoldoutGroup("Rosters")]
        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        public List<List<Battler>> EnemyRosters;

        /// <summary>
        /// Array to keep track of the original indexes of the first ally roster.
        /// </summary>
        internal int[] FirstRosterIndexes;

        /// <summary>
        /// List of the player battlers that have fought in this battle.
        /// </summary>
        internal readonly List<Battler> PlayerBattlersThatHaveFought = new();

        /// <summary>
        /// List of in battle indexes of the teams that have been defeated.
        /// </summary>
        internal readonly List<(BattlerType, int)> DefeatedTeams = new();

        /// <summary>
        /// List of rosters that had a battler faint this turn.
        /// </summary>
        internal readonly List<(BattlerType, int)> HadABattlerFaintThisTurn = new();

        /// <summary>
        /// List of rosters that had a battler faint this turn.
        /// </summary>
        internal readonly List<(BattlerType, int)> HadABattlerFaintLastTurn = new();

        /// <summary>
        /// Translate an in battle index to a roster index.
        /// </summary>
        /// <param name="tuple">Tuple with the battler type and index.</param>
        /// <returns>The roster index, the index of the monster in that roster.</returns>
        public (int, int) InBattleIndexToRosterIndex((BattlerType, int) tuple) =>
            InBattleIndexToRosterIndex(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Translate the index of a battler to the index of the roster it belongs to.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="inBattleIndex">Its index in the battle.</param>
        /// <returns>The roster index, the index of the monster in that roster.</returns>
        public (int, int) InBattleIndexToRosterIndex(BattlerType type, int inBattleIndex) =>
            type switch
            {
                BattlerType.Enemy => (inBattleIndex, 0),
                BattlerType.Ally => AllyRosters.Count == 1 ? (0, inBattleIndex) : (inBattleIndex, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        /// <summary>
        /// Get the requested roster.
        /// </summary>
        public List<Battler> GetRoster(BattlerType type, int index)
        {
            (int rosterIndex, int _) = InBattleIndexToRosterIndex(type, index);

            return type switch
            {
                BattlerType.Ally => AllyRosters?[rosterIndex],
                BattlerType.Enemy => EnemyRosters?[rosterIndex],
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };
        }

        /// <summary>
        /// Are all the battlers of a type fainted?
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if they are all fainted.</returns>
        internal bool AreAllBattlersOfTypeFainted(BattlerType type)
        {
            bool allFainted = true;

            for (int i = 0; i < Battlers.GetNumberOfBattlers(); ++i)
                if (!AreAllBattlersInRosterFainted(type, i))
                    allFainted = false;

            return allFainted;
        }

        /// <summary>
        /// Check if all the battlers in a roster have fainted.
        /// </summary>
        /// <param name="type">Roster type.</param>
        /// <param name="index">Index of the roster.</param>
        /// <returns>True if all are fainted.</returns>
        internal bool AreAllBattlersInRosterFainted(BattlerType type, int index) =>
            GetNumberNotFainted(type, index) == 0;

        /// <summary>
        /// Get the number of battlers in a roster that haven't fainted.
        /// </summary>
        /// <param name="type">Roster type.</param>
        /// <param name="index">Index of the roster.</param>
        /// <returns>Number not fainted.</returns>
        public int GetNumberNotFainted(BattlerType type, int index)
        {
            (int rosterIndex, int _) = InBattleIndexToRosterIndex(type, index);
            return GetRoster(type, rosterIndex).Count(member => member.CanBattle);
        }

        /// <summary>
        /// Can the player still change battlers?
        /// </summary>
        /// <returns>True if they can.</returns>
        public bool CanPlayerChangeBattlers()
        {
            if (!AI.PlayerControlsFirstRoster) return false;

            List<Battler> roster = GetRoster(BattlerType.Ally, 0);

            switch (BattleManager.BattleType)
            {
                case BattleType.SingleBattle:
                case BattleType.DoubleBattle when AllyRosters.Count == 2:
                    return roster.NotFaintedLeft() > 1;
                case BattleType.DoubleBattle when AllyRosters.Count == 1: return roster.NotFaintedLeft() > 2;
                default: throw new ArgumentOutOfRangeException(nameof(BattleType));
            }
        }

        /// <summary>
        /// Check the fainted roster lists and update them.
        /// </summary>
        internal void AfterTurnFaintedChecks()
        {
            HadABattlerFaintLastTurn.Clear();

            HadABattlerFaintLastTurn.AddRange(HadABattlerFaintThisTurn);

            HadABattlerFaintThisTurn.Clear();
        }

        /// <summary>
        /// Updates the roster indicators.
        /// </summary>
        internal void UpdateRosterIndicators()
        {
            BattleManager.AllyRosterIndicator.UpdateRoster(AllyRosters[0]);
            BattleManager.AllyRosterIndicator.Show();

            switch (BattleManager.BattleType)
            {
                case BattleType.SingleBattle:
                    BattleManager.EnemyRosterIndicators[0].UpdateRoster(EnemyRosters[0]);
                    BattleManager.EnemyRosterIndicators[0].Show(BattleManager.EnemyType == EnemyType.Trainer);
                    break;
                case BattleType.DoubleBattle:
                    // In double battle, indicators look best inverted.
                    BattleManager.EnemyRosterIndicators[0].UpdateRoster(EnemyRosters[1]);
                    BattleManager.EnemyRosterIndicators[0].Show(BattleManager.EnemyType == EnemyType.Trainer);
                    BattleManager.EnemyRosterIndicators[1].UpdateRoster(EnemyRosters[0]);
                    BattleManager.EnemyRosterIndicators[1].Show(BattleManager.EnemyType == EnemyType.Trainer);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Prepare the rosters for the battle from the battle parameters.
        /// </summary>
        /// <param name="parameters">Parameters to use.</param>
        internal void PrepareRosters(BattleParameters parameters)
        {
            AllyRosters = new List<List<Battler>>();
            EnemyRosters = new List<List<Battler>>();

            FirstRosterIndexes = new[] {0, 1, 2, 3, 4, 5};

            switch (BattleManager.BattleType)
            {
                // 1 trainer vs 1 trainer.
                case BattleType.SingleBattle when AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 1:

                    AllyRosters.Add(CreateBattlerRoster(PlayerRoster, BattlerType.Ally, true));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Enemy));

                    break;
                case BattleType.SingleBattle when !AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 2:

                    AllyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Ally));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[1], BattlerType.Enemy));

                    break;

                // 1 trainer vs 2 trainers.
                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 2:

                    AllyRosters.Add(CreateBattlerRoster(PlayerRoster, BattlerType.Ally, true));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Enemy));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[1], BattlerType.Enemy));

                    break;

                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 3:

                    AllyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Ally));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[1], BattlerType.Enemy));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[2], BattlerType.Enemy));

                    break;

                // 1 trainer and a friend vs 2 trainers.
                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 3:

                    AllyRosters.Add(CreateBattlerRoster(PlayerRoster, BattlerType.Ally, true));
                    AllyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Ally));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[1], BattlerType.Enemy));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[2], BattlerType.Enemy));

                    break;
                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster && parameters.Rosters.Length == 4:

                    AllyRosters.Add(CreateBattlerRoster(parameters.Rosters[0], BattlerType.Ally));
                    AllyRosters.Add(CreateBattlerRoster(parameters.Rosters[1], BattlerType.Ally));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[2], BattlerType.Enemy));
                    EnemyRosters.Add(CreateBattlerRoster(parameters.Rosters[3], BattlerType.Enemy));

                    break;
                default:
                    throw new ArgumentException(parameters.BattleType
                                              + " with "
                                              + parameters.Rosters.Length
                                              + " rosters and player"
                                              + (AI.PlayerControlsFirstRoster ? "" : " not")
                                              + " controlling the first roster is not supported.");
            }

            if (BattleManager.EnemyType == EnemyType.Wild)
                if (EnemyRosters[0].Count != 1
                 || (BattleManager.BattleType == BattleType.DoubleBattle && EnemyRosters[1].Count != 1))
                    Logger.Error("Wild battle rosters should only have one monster!");

            CheckFirstRosterFaintedAndSwitch();

            UpdateRosterIndicators();
        }

        /// <summary>
        /// For the first roster (the player), check if the first monster is fainted and switch them if it is the case.
        /// We don't do that for the rest of them because designers should not be dumb and create allied/enemy rosters with fainted monsters.
        /// </summary>
        private void CheckFirstRosterFaintedAndSwitch()
        {
            int indexToSend = 0;
            bool needsSecond = BattleManager.BattleType == BattleType.DoubleBattle && AllyRosters.Count == 1;

            List<Battler> firstRoster = GetRoster(BattlerType.Ally, 0);

            while (!Battlers.GetBattlerFromRosterAndIndex(BattlerType.Ally, 0, indexToSend).CanBattle)
            {
                indexToSend++;

                if (indexToSend != firstRoster.Count) continue;
                Logger.Error("First roster doesn't have monsters that can battle!");
                return;
            }

            (firstRoster[indexToSend], firstRoster[0]) = (firstRoster[0], firstRoster[indexToSend]);

            (FirstRosterIndexes[indexToSend], FirstRosterIndexes[0]) =
                (FirstRosterIndexes[0], FirstRosterIndexes[indexToSend]);

            if (!needsSecond) return;
            int secondIndexToSend = indexToSend + 1;

            while (!Battlers.GetBattlerFromRosterAndIndex(BattlerType.Ally, 0, secondIndexToSend).CanBattle)
            {
                secondIndexToSend++;

                if (secondIndexToSend != firstRoster.Count) continue;
                Logger.Error("First roster doesn't have any more monsters that can battle!");
                return;
            }

            (firstRoster[secondIndexToSend], firstRoster[1]) = (firstRoster[1], firstRoster[secondIndexToSend]);

            (FirstRosterIndexes[secondIndexToSend], FirstRosterIndexes[1]) =
                (FirstRosterIndexes[1], FirstRosterIndexes[secondIndexToSend]);
        }

        /// <summary>
        /// Create a list of battlers from a roster.
        /// </summary>
        /// <param name="monsterInstanceRoster">Monster instance roster coming from outside the battle.</param>
        /// <param name="battlerType">Type of the battler, used for difficulty modifications.</param>
        /// <param name="isPlayer">If this the player's roster? Used for difficulty modifications.</param>
        /// <returns>A list of battlers created from that roster.</returns>
        private List<Battler> CreateBattlerRoster(Roster monsterInstanceRoster,
                                                  BattlerType battlerType,
                                                  bool isPlayer = false)
        {
            float levelMultiplier = BattleManager.GlobalGameData.GameDifficulty switch
            {
                GameDifficulty.Easy when BattleManager.EnemyType == EnemyType.Trainer && !isPlayer =>
                    battlerType == BattlerType.Ally ? 1.2f : .8f,
                GameDifficulty.Hard when BattleManager.EnemyType == EnemyType.Trainer && !isPlayer =>
                    battlerType == BattlerType.Ally ? .8f : 1.2f,
                _ => 1
            };

            List<Battler> battlers = new();

            foreach (MonsterInstance monsterInstance in monsterInstanceRoster)
                if (monsterInstance is {IsNullEntry: false})
                {
                    // Adjust HP if level was modified.
                    if (Math.Abs(levelMultiplier - 1) > .001f)
                    {
                        float hpPercentage =
                            (float) monsterInstance.CurrentHP / monsterInstance.GetStats(null)[Stat.Hp];

                        monsterInstance.StatData.Level =
                            (byte) Mathf.Clamp(monsterInstance.StatData.Level * levelMultiplier, 1, 100);

                        monsterInstance.CurrentHP = (uint) (monsterInstance.GetStats(null)[Stat.Hp] * hpPercentage);
                    }

                    battlers.Add(Battler.FromMonsterInstance(monsterInstance));
                }

            return battlers;
        }
    }
}