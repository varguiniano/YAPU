using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Class representing an actor command to give monsters to the player.
    /// </summary>
    [Serializable]
    public class GiveMonsters : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Mode to give it.
        /// </summary>
        [SerializeField]
        private Mode GiveMode;

        /// <summary>
        /// Species in case of random.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        [SerializeField]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Random")]
        private MonsterEntry Species;

        /// <summary>
        /// Form in case of random.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        [SerializeField]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Random")]
        private Form Form;

        /// <summary>
        /// Level in case of random.
        /// </summary>
        [SerializeField]
        [PropertyRange(1, 100)]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Random")]
        private byte Level = 1;

        /// <summary>
        /// Does the random mon have a chance of being shinny?
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Random")]
        private bool CanBeShinny = true;

        /// <summary>
        /// Origin data of the random mon.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Random")]
        private OriginData OriginData;

        /// <summary>
        /// Monster to give.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(GiveMode) + " == Mode.Set")]
        private Roster Roster;

        /// <summary>
        /// Obtained sound.
        /// </summary>
        [SerializeField]
        private AudioReference ObtainedSound;

        /// <summary>
        /// Give the monsters to the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            Roster rosterToUse = ScriptableObject.CreateInstance<Roster>();

            switch (GiveMode)
            {
                case Mode.Random:

                    Form formToUse = Form;

                    if (formToUse.HasShinyVersion && CanBeShinny)
                    {
                        float shinnyRoll = Random.value;

                        Logger.Info("Shiny roll: "
                                  + shinnyRoll
                                  + " of "
                                  + parameterData.YAPUSettings.WildShinyChance
                                  + ".");

                        if (shinnyRoll <= parameterData.YAPUSettings.WildShinyChance)
                            formToUse = formToUse.ShinyVersion;
                    }

                    rosterToUse.Settings = parameterData.YAPUSettings;
                    rosterToUse.Database = parameterData.MonsterDatabase;

                    rosterToUse.AddMonster(Species,
                                           formToUse,
                                           Level,
                                           originalTrainer: OriginData.Trainer,
                                           originRegion: OriginData.Region,
                                           originType: OriginData.OriginType,
                                           originLocation: OriginData.Location,
                                           captureBall: OriginData.Ball);

                    break;
                case Mode.Set: rosterToUse.CopyFrom(Roster); break;
                default: throw new ArgumentOutOfRangeException();
            }

            foreach (MonsterInstance newMonster in rosterToUse)
            {
                if (newMonster == null || newMonster.IsNullEntry) continue;

                AudioManager.Instance.PlayAudio(ObtainedSound);

                yield return DialogManager.ShowDialogAndWait("Dialogs/Monsters/Received",
                                                             localizableModifiers: false,
                                                             modifiers: newMonster.GetNameOrNickName(parameterData
                                                                .Localizer));

                newMonster.CurrentTrainer = parameterData.PlayerCharacter.CharacterController.GetCharacterData()
                                                         .LocalizableName;

                yield return DialogManager.ShowNewMonsterDialog(parameterData.PlayerCharacter, newMonster, false, true);

                yield return TransitionManager.BlackScreenFadeOutRoutine();
            }
        }

        /// <summary>
        /// Modes this command can work on.
        /// </summary>
        private enum Mode
        {
            Random,
            Set
        }
    }
}