using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Actor command that empties a bag.
    /// </summary>
    [Serializable]
    public class EmptyBag : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Bag to empty.
        /// </summary>
        [SerializeField]
        private Bag Bag;

        /// <summary>
        /// Empty the bag.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return Bag.ResetSave();
        }
    }
}