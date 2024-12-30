using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.Monster.Breeding;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// If command to check if an egg is ready in a specific nursery.
    /// </summary>
    [Serializable]
    public class IfEggIsReady : IfCondition
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Check if the egg is ready.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            Nursery.IsEggReady(parameterData.PlayerCharacter.GlobalGameData);
    }
}