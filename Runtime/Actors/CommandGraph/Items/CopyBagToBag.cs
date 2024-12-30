using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Actor command to copy the contents of a bag to another bag.
    /// </summary>
    [Serializable]
    public class CopyBagToBag : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Bag to get the contents from.
        /// </summary>
        [SerializeField]
        private Bag OriginBag;

        /// <summary>
        /// Bag to copy the contents to.
        /// </summary>
        [SerializeField]
        private Bag TargetBag;

        /// <summary>
        /// Copy the contents of the origin bag to the target bag.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            TargetBag.CopyFrom(OriginBag);
            yield break;
        }
    }
}