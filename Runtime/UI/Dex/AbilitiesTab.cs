using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the dex tab that displays a monster's abilities.
    /// </summary>
    public class AbilitiesTab : MonsterDexTab
    {
        /// <summary>
        /// Audio to play when entering the menu.
        /// </summary>
        [SerializeField]
        private AudioReference EnterMenuAudio;

        /// <summary>
        /// Menu controller that allows viewing the abilities info.
        /// </summary>
        [SerializeField]
        private MenuSelector AbilitiesSelector;

        /// <summary>
        /// Reference to the tips shower.
        /// </summary>
        [SerializeField]
        private SingleDexTipsShower TipsShower;

        /// <summary>
        /// Reference to the ability info panel.
        /// </summary>
        [SerializeField]
        private DexGenericInfoPanel AbilityInfoPanel;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Subscribe to menu events.
        /// </summary>
        private void OnEnable()
        {
            AbilitiesSelector.OnHovered += OnAbilityHovered;
            AbilitiesSelector.OnBackSelected += OnBackSelected;
        }

        /// <summary>
        /// Unsubscribe from menu events.
        /// </summary>
        private void OnDisable()
        {
            AbilitiesSelector.OnHovered -= OnAbilityHovered;
            AbilitiesSelector.OnBackSelected -= OnBackSelected;
        }

        /// <summary>
        /// Set the data from this monster into the tab.
        /// Mons with more that 3 abilities will only show their first three.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            DataByFormEntry data = entry.Species[formEntry.Form];

            bool[] buttonsToDisplay = new bool[6];

            for (int i = 0; i < 3; i++)
            {
                buttonsToDisplay[i] = data.Abilities.Count > i;

                if (buttonsToDisplay[i])
                    ((DexAbilityButton)AbilitiesSelector.MenuOptions[i]).SetAbility(data.Abilities[i]);
            }

            for (int i = 0; i < 3; i++)
            {
                buttonsToDisplay[i + 3] = data.HiddenAbilities.Count > i;

                if (buttonsToDisplay[i + 3])
                    ((DexAbilityButton)AbilitiesSelector.MenuOptions[i + 3]).SetAbility(data.HiddenAbilities[i]);
            }

            AbilitiesSelector.UpdateLayout(buttonsToDisplay.ToList());
        }

        /// <summary>
        /// Enter the abilities menu when the select button is pressed.
        /// </summary>
        public override void OnSelectPressedOnParentScreen()
        {
            if (AbilitiesSelector.MenuOptions.Count == 0) return;

            audioManager.PlayAudio(EnterMenuAudio);
            AbilityInfoPanel.Open();
            AbilitiesSelector.RequestInput();
            TipsShower.SwitchToSubmenu();
        }

        /// <summary>
        /// Called when an ability is hovered.
        /// </summary>
        /// <param name="index">Index hovered.</param>
        private void OnAbilityHovered(int index)
        {
            Ability ability = ((DexAbilityButton)AbilitiesSelector.MenuOptions[index]).GetAbility();
            AbilityInfoPanel.SetTexts(ability.LocalizableName, ability.LocalizableDescription);
        }

        /// <summary>
        /// Called when back is selected in the menu.
        /// </summary>
        private void OnBackSelected()
        {
            AbilitiesSelector.ReleaseInput();
            AbilityInfoPanel.Close();
            TipsShower.SwitchToGeneral();
        }
    }
}