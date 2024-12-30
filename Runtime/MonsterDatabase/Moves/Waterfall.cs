using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Waterfall.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/Waterfall", fileName = "Waterfall")]
    public class Waterfall : StatusChanceDamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Make the player use Waterfall.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player.</param>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="mapSceneLauncher"></param>
        public override IEnumerator UseOutOfBattle(PlayerCharacter playerCharacter,
                                                   MonsterInstance monster,
                                                   ILocalizer localizer,
                                                   MapSceneLauncher mapSceneLauncher)
        {
            if (playerCharacter.CharacterController.CurrentGrid.SceneInfo.Region.IsMoveLockedByBadge(this,
                    out Badge badge))
                if (!playerCharacter.GlobalGameData.HasBadge(badge, playerCharacter.Region))
                {
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/BlockedByBadge",
                                                                 modifiers: new[]
                                                                            {
                                                                                badge.LocalizableName, LocalizableName
                                                                            });

                    yield break;
                }

            if (!playerCharacter.CharacterController.CanClimbWaterfall())
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            yield return DialogManager.CloseMenus();

            yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monster.GetNameOrNickName(localizer),
                                                                        localizer[LocalizableName]
                                                                    });

            yield return playerCharacter.CharacterController.ClimbWaterfall();

            playerCharacter.GlobalGridManager.StopAllActors = false;
            playerCharacter.LockInput(false);
        }
    }
}