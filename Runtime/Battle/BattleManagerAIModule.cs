using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Battle.AI;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// AI handling module of the battle manager.
    /// </summary>
    public class BattleManagerAIModule : BattleManagerModule<BattleManagerAIModule>
    {
        /// <summary>
        /// Flag to know if the player controls the first roster or it's controller by an AI.
        /// </summary>
        [FoldoutGroup("AIs")]
        [ReadOnly]
        public bool PlayerControlsFirstRoster;

        /// <summary>
        /// AIs that will control each roster of the battle.
        /// </summary>
        [FoldoutGroup("AIs")]
        [ReadOnly]
        [HideInEditorMode]
        public List<BattleAI> AllyAIs;

        /// <summary>
        /// AIs that will control each roster of the battle.
        /// </summary>
        [FoldoutGroup("AIs")]
        [ReadOnly]
        [HideInEditorMode]
        public List<BattleAI> EnemyAIs;

        /// <summary>
        /// Get the AI corresponding to a roster.
        /// </summary>
        /// <param name="type">Type of battler we are looking for.</param>
        /// <param name="index">Index of the roster to look for.</param>
        /// <returns>The AI that controls that roster.</returns>
        /// <exception cref="BattleManager.UnsupportedBattlerException">Thrown if we enter a not supported battler type.</exception>
        public BattleAI GetAI(BattlerType type, int index)
        {
            (int rosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, index);

            return type switch
            {
                BattlerType.Ally => AllyAIs[Mathf.Clamp(rosterIndex,
                                                        0,
                                                        1 - Battlers.GetNumberOfBattlersUnderPlayersControl())],
                BattlerType.Enemy => EnemyAIs[rosterIndex],
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };
        }

        /// <summary>
        /// Setup the battle AIs from the parameters.
        /// </summary>
        /// <param name="parameters">Parameters for this battle.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if wrong battle type.</exception>
        internal void SetupAIs(BattleParameters parameters)
        {
            AllyAIs = new List<BattleAI>();
            EnemyAIs = new List<BattleAI>();

            switch (BattleManager.BattleType)
            {
                case BattleType.SingleBattle:

                    if (PlayerControlsFirstRoster)
                    {
                        if (parameters.AIs.Length < 1)
                        {
                            Logger.Error("We need one AI to control the enemy!");
                            return;
                        }

                        EnemyAIs.Add(parameters.AIs[0]);
                    }
                    else
                    {
                        if (parameters.AIs.Length < 2)
                        {
                            Logger.Error("We need two AIs to control the ally and enemy!");
                            return;
                        }

                        AllyAIs.Add(parameters.AIs[0]);
                        EnemyAIs.Add(parameters.AIs[1]);
                    }

                    break;
                case BattleType.DoubleBattle when Rosters.AllyRosters.Count == 1:

                    if (PlayerControlsFirstRoster)
                    {
                        if (parameters.AIs.Length < 2)
                        {
                            Logger.Error("We need two AIs to control the enemies!");
                            return;
                        }

                        EnemyAIs.Add(parameters.AIs[0]);
                        EnemyAIs.Add(parameters.AIs[1]);
                    }
                    else
                    {
                        if (parameters.AIs.Length < 3)
                        {
                            Logger.Error("We need three AIs to control the ally and enemies!");
                            return;
                        }

                        AllyAIs.Add(parameters.AIs[0]);
                        EnemyAIs.Add(parameters.AIs[1]);
                        EnemyAIs.Add(parameters.AIs[2]);
                    }

                    break;

                case BattleType.DoubleBattle when Rosters.AllyRosters.Count == 2:

                    if (PlayerControlsFirstRoster)
                    {
                        if (parameters.AIs.Length < 3)
                        {
                            Logger.Error("We need three AIs to control the ally and the enemies!");
                            return;
                        }

                        AllyAIs.Add(parameters.AIs[0]);
                        EnemyAIs.Add(parameters.AIs[1]);
                        EnemyAIs.Add(parameters.AIs[2]);
                    }
                    else
                    {
                        if (parameters.AIs.Length < 4)
                        {
                            Logger.Error("We need four AIs to control the ally and enemies!");
                            return;
                        }

                        AllyAIs.Add(parameters.AIs[0]);
                        AllyAIs.Add(parameters.AIs[1]);
                        EnemyAIs.Add(parameters.AIs[2]);
                        EnemyAIs.Add(parameters.AIs[3]);
                    }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}