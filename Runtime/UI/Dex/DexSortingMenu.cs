using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dex.Filters;
using Varguiniano.YAPU.Runtime.UI.Sorting;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Menu in charge of sorting the dex monster list.
    /// </summary>
    public class DexSortingMenu : MenuSelector, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the sorting panel open position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform SortingPanelOpenPosition;

        /// <summary>
        /// Reference to the sorting panel closed position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform SortingPanelClosedPosition;

        /// <summary>
        /// Audio to play when opening and closing.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference OpenCloseAudio;

        /// <summary>
        /// Event raised when the menu is closed.
        /// </summary>
        public Action OnMenuClosed;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the dex.
        /// </summary>
        [Inject]
        private MonsterDex.Dex dex;

        /// <summary>
        /// Make sure it's not in the way.
        /// </summary>
        private void OnEnable()
        {
            GetCachedComponent<Transform>().localPosition = SortingPanelClosedPosition.localPosition;
            ResetFilters();
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        public void Open()
        {
            inputManager.BlockInput();

            audioManager.PlayAudio(OpenCloseAudio);

            GetCachedComponent<Transform>()
               .DOLocalMove(SortingPanelOpenPosition.localPosition, .25f)
               .SetEase(Ease.OutBack)
               .OnComplete(() =>
                           {
                               OnButtonSelected += OnButtonSelectedCallback;

                               inputManager.BlockInput(false);
                               inputManager.RequestInput(this);

                               ReselectAfterFrames();
                           });
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        private void Close()
        {
            OnMenuClosed?.Invoke();

            inputManager.BlockInput();

            audioManager.PlayAudio(OpenCloseAudio);

            GetCachedComponent<Transform>()
               .DOLocalMove(SortingPanelClosedPosition.localPosition, .25f)
               .SetEase(Ease.InBack)
               .OnComplete(() =>
                           {
                               OnButtonSelected -= OnButtonSelectedCallback;

                               inputManager.BlockInput(false);
                               inputManager.ReleaseInput(this);
                           });
        }

        /// <summary>
        /// Filter the storage with the corresponding filters.
        /// </summary>
        /// <param name="databaseMonsters">All monsters in the database.</param>
        /// <param name="callback">Callback with the new list.</param>
        public IEnumerator FilterDex(List<MonsterEntry> databaseMonsters,
                                     Action<List<(MonsterDexEntry, FormDexEntry, MonsterGender)>> callback)
        {
            List<(MonsterDexEntry, FormDexEntry, MonsterGender)> dexEntries = new();
            List<MonsterDexEntry> allDexMonsterEntries = null;

            yield return dex.GetEntries(databaseMonsters,
                                        entriesRetrieves => allDexMonsterEntries = entriesRetrieves);

            // Create an entry for each form and gender so that filtering catches everything that has been seen.
            foreach (MonsterDexEntry entry in allDexMonsterEntries)
            {
                foreach (FormDexEntry formEntry in entry.FormEntries)
                {
                    DataByFormEntry data = entry.Species[formEntry.Form];

                    if (data.HasBinaryGender)
                    {
                        if (data.HasGenderVariations)
                        {
                            if (data.FemaleRatio > 0) dexEntries.Add((entry, formEntry, MonsterGender.Female));

                            if (data.MaleRatio > 0) dexEntries.Add((entry, formEntry, MonsterGender.Male));
                        }
                        else
                            dexEntries.Add((entry, formEntry, MonsterGender.Female));
                    }
                    else
                        dexEntries.Add((entry, formEntry, MonsterGender.NonBinary));
                }
            }

            IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> filteredEntries =
                new List<(MonsterDexEntry, FormDexEntry, MonsterGender)>(dexEntries);

            // Inverted filtering so that sorting (the first button) should be the last one to be applied.
            // This is because sorting depends on the forms and genders selected by the filters.
            // It's also the most expensive operation so having it when the fewer entries are left is better.
            for (int i = MenuOptions.Count - 1; i >= 0; i--)
            {
                MenuItem item = MenuOptions[i];
                FilterButton filterButton = item as FilterButton;
                if (filterButton == null) continue;
                filteredEntries = filterButton.ApplyFilter(filteredEntries);
            }

            List<(MonsterDexEntry, FormDexEntry, MonsterGender)> formFilteredEntries = new();

            // For entries in which one at least one form has been seen, remove the ones that haven't been seen.
            // This applies for dex number sorting, in which all forms are sorted and not seen ones are not removed.
            // This also deduplicates monster entries making sure only one of each has been seen.
            foreach ((MonsterDexEntry Monster, FormDexEntry Form, MonsterGender Gender) entry in filteredEntries)
            {
                if (!entry.Monster.HasMonsterBeenSeen
                 && formFilteredEntries.All(addedEntry => addedEntry.Item1 != entry.Monster))
                {
                    formFilteredEntries.Add(entry);
                    continue;
                }

                List<FormDexEntry> formsSeen = entry.Monster.GetAllFormsSeen();

                if (formFilteredEntries.Any(addedEntry => addedEntry.Item1 == entry.Monster)
                 || !formsSeen.Contains(entry.Form)
                 || (entry.Monster.Species[entry.Form.Form].HasGenderVariations
                  && !entry.Form.GendersSeen[entry.Gender]))
                    continue;

                formFilteredEntries.Add(entry);
            }

            callback.Invoke(formFilteredEntries.ToList());
        }

        /// <summary>
        /// Called when a button is selected.
        /// </summary>
        private void OnButtonSelectedCallback(int index) => ((FilterButton) MenuOptions[index]).OnButtonSelected();

        /// <summary>
        /// Close the menu.
        /// </summary>
        public override void OnBack(InputAction.CallbackContext context)
        {
            base.OnBack(context);

            if (context.phase != InputActionPhase.Started) return;

            Close();
        }

        /// <summary>
        /// Disable all filters.
        /// </summary>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            base.OnExtra1(context);

            if (context.phase != InputActionPhase.Started) return;

            ResetFilters();
        }

        /// <summary>
        /// Reset all filters.
        /// </summary>
        private void ResetFilters()
        {
            DexSortingFilterButton sortingButton = MenuOptions[0] as DexSortingFilterButton;

            if (sortingButton != null)
                sortingButton.SetOptionAndMode(SortOption.DexNumber,
                                               SortMode.Ascending);

            foreach (ToggableFilterButton filterButton in MenuOptions.OfType<ToggableFilterButton>())
                filterButton.DisableFilter();
        }
    }
}