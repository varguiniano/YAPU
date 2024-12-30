using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle
{
    /// <summary>
    /// Item effect to enable or disable the player biking
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OutOfBattle/EnableDisableBiking", fileName = "EnableDisableBiking")]
    public class EnableDisableBiking : UseOutOfBattleItemEffect
    {
        /// <summary>
        /// Enable or disable biking.
        /// </summary>
        public override IEnumerator Use(Item item,
                                        PlayerSettings playerSettings,
                                        PlayerCharacter playerCharacter,
                                        ILocalizer localizer,
                                        Action<bool> finished)
        {
            finished.Invoke(false);

            if (playerCharacter.CharacterController.IsSwimming
             || playerCharacter.CharacterController.CurrentGrid
                               .GetTypeOfTileDirectlyBelowSortOrder(playerCharacter.Position,
                                                                    playerCharacter.CharacterController.SortOrder)
             == TileType.WalkableNotBikable)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            if (DialogManager.IsBagScreenOpen) yield return DialogManager.CloseMenus();

            playerCharacter.CharacterController.IsBiking = !playerCharacter.CharacterController.IsBiking;

            playerCharacter.CharacterController.LookAt(playerCharacter.CharacterController.CurrentDirection,
                                                       useBikingSprite: playerCharacter.CharacterController.IsBiking);
        }
    }
}