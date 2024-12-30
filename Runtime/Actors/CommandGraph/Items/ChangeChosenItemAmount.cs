using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Actor command to change the amount of the chosen item by the given number.
    /// </summary>
    [Serializable]
    public class ChangeChosenItemAmount : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Amount to change.
        /// </summary>
        [SerializeField]
        private int Amount = -1;

        /// <summary>
        /// Change the amount.
        /// </summary>">
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.PlayerBag.ChangeItemAmount((Item) parameterData.ExtraParams[0], Amount);
            yield break;
        }
    }
}