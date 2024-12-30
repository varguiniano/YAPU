using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dex
{
    /// <summary>
    /// Register a monster from a roster as seen.
    /// </summary>
    [Serializable]
    public class RegisterMonsterAsSeen : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Roster with the monster to register.
        /// </summary>
        [SerializeField]
        private Roster Roster;

        /// <summary>
        /// Register in the dex.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.PlayerDex.RegisterAsSeen(Roster[0], true, false);
            yield break;
        }
    }
}