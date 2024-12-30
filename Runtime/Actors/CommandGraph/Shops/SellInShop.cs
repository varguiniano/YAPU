using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Shops
{
    /// <summary>
    /// Command to run all shop logic related to selling.
    /// </summary>
    [Serializable]
    public class SellInShop : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Open the sell UI.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            bool finished = false;

            DialogManager.ShowBag((_, _) => finished = true,
                                  parameterData.PlayerCharacter,
                                  selection: true,
                                  selling: true);

            yield return new WaitUntil(() => finished);
        }
    }
}