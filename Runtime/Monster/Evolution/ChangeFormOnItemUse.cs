using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Data class representing an item effect that triggers a form change when used.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/ChangeForm",
                     fileName = "ChangeFormOnItemUseEffect")]
    public class ChangeFormOnItemUse : UseOnTargetItemEffect
    {
        /// <summary>
        /// Compatible monster to use this effect on.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        private MonsterEntry CompatibleMonster;

        /// <summary>
        /// Compatible form to use this effect on.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private Form CompatibleForm;

        /// <summary>
        /// Check gender compatibility?
        /// </summary>
        [SerializeField]
        private bool CheckGender;

        /// <summary>
        /// Gender this effect is compatible with.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(CheckGender))]
        private MonsterGender CompatibleGender;

        /// <summary>
        /// Are shiny forms compatible?
        /// </summary>
        [SerializeField]
        private bool ShiniesAreCompatible = true;

        /// <summary>
        /// Form to change to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private Form TargetForm;

        /// <summary>
        /// Keep shiny if it was?
        /// </summary>
        [SerializeField]
        private bool KeepShiny = true;

        /// <summary>
        /// Check if the effect can be used on a monster.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item">Item being used.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            monsterInstance.Species == CompatibleMonster
         && (monsterInstance.Form == CompatibleForm
          || (ShiniesAreCompatible && monsterInstance.Form == CompatibleForm.ShinyVersion))
         && (!CheckGender || monsterInstance.PhysicalData.Gender == CompatibleGender);

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager">Reference to the evolution manager.</param>
        /// <param name="inputManager">Reference to the input manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                         Item item,
                                                         YAPUSettings settings,
                                                         ExperienceLookupTable experienceLookupTable,
                                                         PlayerCharacter playerCharacter,
                                                         TimeManager timeManager,
                                                         EvolutionManager evolutionManager,
                                                         IInputManager inputManager,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            if (!IsCompatible(settings, timeManager, monsterInstance, item, playerCharacter))
            {
                finished.Invoke(false);
                yield break;
            }

            Form targetForm = TargetForm;

            if (monsterInstance.Form.IsShiny && KeepShiny) targetForm = targetForm.ShinyVersion;

            monsterInstance.ChangeForm(targetForm);

            yield return DialogManager.ShowDialogAndWait("Dialogs/ChangedForm",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monsterInstance.GetNameOrNickName(localizer),
                                                                        localizer[targetForm.LocalizableName]
                                                                    });

            playerCharacter.PlayerDex.RegisterAsCaught(monsterInstance, true, true, false);

            finished.Invoke(true);
        }
    }
}