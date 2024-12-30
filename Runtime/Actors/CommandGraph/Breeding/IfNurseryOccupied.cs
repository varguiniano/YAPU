using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.Monster.Breeding;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// If command that checks if a nursery is occupied.
    /// </summary>
    [Serializable]
    public class IfNurseryOccupied : IfCondition
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Check if the nursery is occupied.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) => Nursery.IsOccupied;
    }
}