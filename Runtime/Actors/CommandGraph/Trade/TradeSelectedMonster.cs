using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Trade
{
    /// <summary>
    /// Command to trade the monster the player has chosen with another one.
    /// </summary>
    [Serializable]
    public class TradeSelectedMonster : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Roster to trade, only the first one in the roster will be traded.
        /// </summary>
        [SerializeField]
        [InfoBox("Only the first one in the roster will be traded")]
        private Roster ToTrade;

        /// <summary>
        /// Trade the monster with the new one.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            if (parameterData.ExtraParams[0] is not MonsterInstance chosenMonster
             || parameterData.ExtraParams[1] is not CommandParameter<bool> isFromRoster)
            {
                Logger.Error("No monster chosen!");
                yield break;
            }

            if (parameterData.Actor is not ActorCharacter actorCharacter)
            {
                Logger.Error("Actor interacting is not a character!");
                yield break;
            }

            yield return parameterData.TradeManager.TradeMonster(chosenMonster,
                                                                 ToTrade[0].Clone(),
                                                                 parameterData.PlayerCharacter,
                                                                 actorCharacter.CharacterController.GetCharacterData(),
                                                                 (bool) isFromRoster);

            yield return TransitionManager.BlackScreenFadeOutRoutine();
            parameterData.InputManager.BlockInput(false);
        }
    }
}