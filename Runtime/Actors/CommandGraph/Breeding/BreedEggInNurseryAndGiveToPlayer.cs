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
    /// Breed an egg in a nursery and gives it to the player.
    /// </summary>
    [Serializable]
    public class BreedEggInNurseryAndGiveToPlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Take the two mons, breed an egg with the nursery and give it to the player's.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            MonsterInstance egg =
                Nursery.Breed(parameterData.MonsterDatabase,
                              parameterData.YAPUSettings,
                              parameterData.PlayerCharacter,
                              parameterData.Localizer);

            yield return DialogManager.ShowNewMonsterDialog(parameterData.PlayerCharacter, egg, false, false);

            yield return TransitionManager.BlackScreenFadeOutRoutine();
        }
    }
}