using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Debug
{
    /// <summary>
    /// Give the player X of each item.
    /// </summary>
    [Serializable]
    public class DebugGiveEachItem : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Amount to give.
        /// </summary>
        [PropertyRange(1, 999)]
        [SerializeField]
        private int Amount;

        /// <summary>
        /// Give them X of each item.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            foreach (Item item in parameterData.MonsterDatabase.GetItems())
                parameterData.PlayerCharacter.PlayerBag.ChangeItemAmount(item, Amount);

            yield break;
        }
    }
}