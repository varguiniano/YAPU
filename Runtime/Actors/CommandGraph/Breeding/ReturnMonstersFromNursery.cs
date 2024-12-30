using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// Return the monsters in a nursery to the player.
    /// </summary>
    [Serializable]
    public class ReturnMonstersFromNursery : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Return the monsters to the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            (MonsterInstance firstMonster, MonsterInstance secondMonster) = Nursery.RetrieveMonsters();

            yield return DialogManager.ShowNewMonsterDialog(parameterData.PlayerCharacter, firstMonster, true, false);
            yield return DialogManager.ShowNewMonsterDialog(parameterData.PlayerCharacter, secondMonster, true, false);

            yield return TransitionManager.BlackScreenFadeOutRoutine();
        }
    }
}