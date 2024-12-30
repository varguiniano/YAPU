using System;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Actor command that checks if the chosen item is one of the allowed.
    /// </summary>
    [Serializable]
    public class IfChosenItemIs : IfCondition
    {
        /// <summary>
        /// Possible items to check.
        /// </summary>
        [SerializeField]
        private List<Item> PossibleItems;

        /// <summary>
        /// Check if it matches.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            PossibleItems.Contains((Item) parameterData.ExtraParams[0]);
    }
}