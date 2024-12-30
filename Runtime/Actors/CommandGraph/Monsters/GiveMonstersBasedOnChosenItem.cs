using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.TwoDAudio.Runtime;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// A command very similar to give monsters, but the given roster depends on the chosen item.
    /// </summary>
    [Serializable]
    public class GiveMonstersBasedOnChosenItem : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Monsters to give depending on the item.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Item, ObjectPair<MonsterEntry, Form>> MonstersPerItem;

        /// <summary>
        /// Level in case of random.
        /// </summary>
        [SerializeField]
        [PropertyRange(1, 100)]
        private byte Level = 1;

        /// <summary>
        /// Does the random mon have a chance of being shinny?
        /// </summary>
        [SerializeField]
        private bool CanBeShinny = true;

        /// <summary>
        /// Ball to put the monsters in.
        /// </summary>
        [SerializeField]
        private Ball Ball;

        /// <summary>
        /// Obtained sound.
        /// </summary>
        [SerializeField]
        private AudioReference ObtainedSound;

        /// <summary>
        /// Change the amount.
        /// </summary>">
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            Item item = (Item) parameterData.ExtraParams[0];
            Roster rosterToUse = ScriptableObject.CreateInstance<Roster>();
            (MonsterEntry species, Form form) = (MonstersPerItem[item].Key, MonstersPerItem[item].Value);

            if (form.HasShinyVersion && CanBeShinny)
            {
                float shinnyRoll = Random.value;

                Logger.Info("Shiny roll: " + shinnyRoll + " of " + parameterData.YAPUSettings.WildShinyChance + ".");

                if (shinnyRoll <= parameterData.YAPUSettings.WildShinyChance) form = form.ShinyVersion;
            }

            rosterToUse.Settings = parameterData.YAPUSettings;
            rosterToUse.Database = parameterData.MonsterDatabase;

            rosterToUse.AddMonster(species,
                                   form,
                                   Level,
                                   originalTrainer: parameterData.PlayerCharacter.CharacterController
                                                                 .GetLocalizedName(),
                                   originRegion: parameterData.Localizer[parameterData.PlayerCharacter.Region
                                                                            .LocalizableName],
                                   originType: OriginData.Type.Resurrected,
                                   originLocation: parameterData.Localizer[parameterData.PlayerCharacter.Scene
                                                                              .LocalizableNameKey],
                                   captureBall: Ball);

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
    }
}