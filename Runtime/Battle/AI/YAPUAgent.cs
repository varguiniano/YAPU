using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// ML Agent that gets spawned in the scene because ML Agents doesn't support a Scriptable Object agent.
    /// This class mainly communicates with the MLBattleAI Scriptable Object.
    /// </summary>
    public class YAPUAgent : Agent
    {
        /// <summary>
        /// Observations to send the brain.
        /// These are written by the AI SO.
        /// </summary>
        public List<int> Observations;

        /// <summary>
        /// Decisions taken by the brain based on the observations.
        /// </summary>
        public List<int> Decisions;

        /// <summary>
        /// Flag to indicate that a decision has been taken.
        /// </summary>
        public bool DecisionTaken { get; private set; }

        /// <summary>
        /// Method that sends the observations to the brain.
        /// </summary>
        /// <param name="sensor">Structure that stores all the sensor information.</param>
        public override void CollectObservations(VectorSensor sensor)
        {
            foreach (int observation in Observations) sensor.AddObservation(observation);
        }

        /// <summary>
        /// Method called when the agent receives the actions from the brain.
        /// </summary>
        /// <param name="actions">Structure that stores the actions to take.</param>
        public override void OnActionReceived(ActionBuffers actions)
        {
            Decisions = actions.DiscreteActions.Array.ToList();
            DecisionTaken = true;
        }

        /// <summary>
        /// Request a decision. It will need a step to take the decision.
        /// </summary>
        public void RequestDecisionAsync()
        {
            DecisionTaken = false;
            RequestDecision();
        }
    }
}