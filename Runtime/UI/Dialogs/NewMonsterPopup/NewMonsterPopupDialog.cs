using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.NewMonsterPopup
{
    /// <summary>
    /// Dialog to popup when a new monster has been acquired.
    /// This dialog helps the player nickname and store or add the monster to the team.
    /// </summary>
    public class NewMonsterPopupDialog : HidableUiElement<NewMonsterPopupDialog>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [SerializeField]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; private set; }

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        [Inject]
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the storage.
        /// </summary>
        [Inject]
        private MonsterStorage storage;

        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [Inject]
        private Roster roster;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="newMonster">New monster.</param>
        /// <param name="requestNickname">Request the player to show a nickname?</param>
        /// <param name="alreadySeen">Had the player already seen this monster?</param>
        public IEnumerator ShowDialog(PlayerCharacter playerCharacter,
                                      MonsterInstance newMonster,
                                      bool requestNickname,
                                      bool alreadySeen)
        {
            yield return TransitionManager.BlackScreenFadeInRoutine();

            PlayerCharacter = playerCharacter;

            if (!configurationManager.GetConfiguration(out GameplayConfiguration configuration))
                Logger.Error("Couldn't retrieve gameplay configuration!");

            UIMonsterSprite.SetMonster(newMonster);

            Show();

            if (!newMonster.EggData.IsEgg)
                PlayerCharacter.PlayerDex.RegisterAsCaught(newMonster, false, alreadySeen, false);

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            if (configuration.ShowNicknameDialog && requestNickname) yield return NicknamePrompt(newMonster);

            yield return StorageOptionsPrompt(PlayerCharacter, newMonster, configuration.AutoSendToStorage);

            yield return TransitionManager.BlackScreenFadeInRoutine();

            Show(false);
        }

        /// <summary>
        /// Prompt the player to nickname the new monster.
        /// </summary>
        /// <param name="newMonster">Monster to nickname.</param>
        private IEnumerator NicknamePrompt(MonsterInstance newMonster)
        {
            if (newMonster.EggData.IsEgg) yield break;

            int option = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         choice => option = choice,
                                         onBackCallback: () => option = 1,
                                         showDialog: true,
                                         localizationKey: "Dialogs/StoringPrompt/WantToNickname",
                                         localizableModifiers: false,
                                         modifiers: newMonster.GetNameOrNickName(localizer));

            yield return new WaitWhile(() => option == -1);

            if (option != 0) yield break;

            yield return DialogManager.RequestTextInput(settings.MaxNicknameSize,
                                                        "Dialogs/TextInput/Nickname",
                                                        new[] {newMonster.GetNameOrNickName(localizer)},
                                                        newMonster.GetIcon(),
                                                        (entered, text) =>
                                                        {
                                                            if (entered) newMonster.Nickname = text;

                                                            newMonster.HasNickname = entered;
                                                        });

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Prompt the player with options to view the summary, add to the team or send the monster to storage.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="newMonster">Monster to add.</param>
        /// <param name="autoSendToStorage">Should the monster be auto sent to storage?</param>
        private IEnumerator StorageOptionsPrompt(PlayerCharacter playerCharacter,
                                                 MonsterInstance newMonster,
                                                 bool autoSendToStorage)
        {
            // The first monster will always be added to the roster.
            if (roster.RosterSize == 0)
                yield return AddToRoster(newMonster);
            else if (autoSendToStorage)
                yield return SendToStorage(newMonster);
            else
            {
                bool decisionMade = false;

                while (!decisionMade)
                {
                    int option = -1;

                    DialogManager.ShowChoiceMenu(new List<string>
                                                 {
                                                     "Dialogs/StoringPrompt/Upload",
                                                     "Dialogs/StoringPrompt/PlaceInRoster",
                                                     "Dialogs/StoringPrompt/Summary"
                                                 },
                                                 choice => option = choice,
                                                 onBackCallback: () => option = 0);

                    yield return new WaitWhile(() => option == -1);

                    switch (option)
                    {
                        case 0:
                            yield return SendToStorage(newMonster);
                            decisionMade = true;
                            break;
                        case 1:

                            if (roster.RosterSize < 6)
                                yield return AddToRoster(newMonster);
                            else
                            {
                                bool replacementChosen = false;
                                MonsterInstance monsterToReplace = null;

                                DialogManager.ShowPlayerRosterMenu(playerCharacter,
                                                                   false,
                                                                   false,
                                                                   isChoosingDialog: true,
                                                                   isChoosingToSwapDialog: true,
                                                                   onBackCallback:
                                                                   (_, chosenMonster, _) =>
                                                                   {
                                                                       replacementChosen = true;
                                                                       monsterToReplace = chosenMonster;
                                                                   });

                                yield return new WaitUntil(() => replacementChosen);

                                monsterToReplace =
                                    roster.ReplaceMonsterAt(roster.RosterData.IndexOf(monsterToReplace), newMonster);

                                yield return
                                    DialogManager.ShowDialogAndWait("Dialogs/StoringPrompt/PlaceInRoster/Dialog",
                                                                    localizableModifiers: false,
                                                                    modifiers: newMonster.GetNameOrNickName(localizer));

                                yield return SendToStorage(monsterToReplace);
                            }

                            decisionMade = true;
                            break;
                        case 2:
                            bool summaryOpen = true;

                            DialogManager.ShowMonsterSummary(new List<MonsterInstance> {newMonster},
                                                             0,
                                                             null,
                                                             PlayerCharacter,
                                                             _ => summaryOpen = false);

                            yield return new WaitWhile(() => summaryOpen);

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Send a monster to storage.
        /// </summary>
        /// <param name="monster">Monster to send.</param>
        private IEnumerator SendToStorage(MonsterInstance monster)
        {
            storage.AddMonster(monster);

            yield return DialogManager.ShowDialogAndWait("Dialogs/StoringPrompt/Upload/Dialog",
                                                         localizableModifiers: false,
                                                         modifiers: monster.GetNameOrNickName(localizer));
        }

        /// <summary>
        /// Add a monster to the roster.
        /// </summary>
        /// <param name="monster">Monster to add.</param>
        private IEnumerator AddToRoster(MonsterInstance monster)
        {
            roster.AddMonster(monster);

            yield return DialogManager.ShowDialogAndWait("Dialogs/StoringPrompt/PlaceInRoster/Dialog",
                                                         localizableModifiers: false,
                                                         modifiers: monster.GetNameOrNickName(localizer));
        }
    }
}