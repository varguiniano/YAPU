using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using Zenject;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.UI.CharacterSelection
{
    /// <summary>
    /// Controller for the character selection screen.
    /// </summary>
    public class CharacterSelector : WhateverBehaviour<CharacterSelector>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the menu that allows choosing the character.
        /// </summary>
        [SerializeField]
        private MenuSelector CharacterMenu;

        /// <summary>
        /// Reference to the new game initializer.
        /// </summary>
        [Inject]
        private NewGameInitializer newGameInitializer;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        [Inject]
        private CharacterData playerCharacter;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Flag to know when the player has selected a character.
        /// </summary>
        private bool hasSelectedCharacter;

        /// <summary>
        /// Run the selection routine.
        /// </summary>
        private void OnEnable() => StartCoroutine(CharacterSelection());

        /// <summary>
        /// Routine with the logic for character selection.
        /// </summary>
        private IEnumerator CharacterSelection()
        {
            CharacterMenu.OnButtonSelected += OnCharacterSelected;

            CharacterMenu.RequestInput();

            yield return new WaitUntil(() => hasSelectedCharacter);

            yield return DialogManager.RequestTextInput(settings.MaxNicknameSize,
                                                        "CharacterSelection/EnterName",
                                                        new string[] { },
                                                        playerCharacter.GetLooking(CharacterController.Direction.Down,
                                                            false,
                                                            false,
                                                            false,
                                                            false),
                                                        (_, text) =>
                                                        {
                                                            playerCharacter.LocalizableName = text;
                                                        },
                                                        false,
                                                        localizer[playerCharacter.LocalizableName]);

            newGameInitializer.OnCharacterSelectionFinished();
        }

        /// <summary>
        /// Called when the player chooses a character from the menu.
        /// </summary>
        /// <param name="index">Index of the chosen character.</param>
        private void OnCharacterSelected(int index) => StartCoroutine(OnCharacterSelectedRoutine(index));

        /// <summary>
        /// Called when the player chooses a character from the menu.
        /// </summary>
        /// <param name="index">Index of the chosen character.</param>
        private IEnumerator OnCharacterSelectedRoutine(int index)
        {
            yield return WaitAFrame;

            CharacterData character = ((CharacterSelectionButton)CharacterMenu.MenuOptions[index]).Character;

            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>()
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => choice = playerChoice,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "CharacterSelection/Confirmation");

            yield return new WaitWhile(() => choice == -1);

            if (choice != 0) yield break;

            CharacterMenu.OnButtonSelected -= OnCharacterSelected;
            CharacterMenu.ReleaseInput();
            playerCharacter.CopyFrom(character, true, localizer);
            hasSelectedCharacter = true;
        }
    }
}