using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters;
using Varguiniano.YAPU.Runtime.UI.Sorting;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu
{
    /// <summary>
    /// Controller for the menu that manages sorting and filtering the storage.
    /// </summary>
    public class StorageSortingMenu : MenuSelector
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
        /// Flag to know if the menu is open.
        /// </summary>
        public bool IsOpen { get; private set; }

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
        /// Make sure it's not in the way.
        /// </summary>
        private void OnEnable() =>
            GetCachedComponent<Transform>().localPosition = SortingPanelClosedPosition.localPosition;

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
                               IsOpen = true;

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
                               IsOpen = false;
                           });
        }

        /// <summary>
        /// Filter the storage with the corresponding filters.
        /// </summary>
        /// <param name="original">Original storage list.</param>
        /// <returns>The new list.</returns>
        public List<MonsterInstance> FilterStorage(IEnumerable<MonsterInstance> original)
        {
            IEnumerable<MonsterInstance> filtered = new List<MonsterInstance>(original);

            foreach (FilterButton item in MenuOptions.OfType<FilterButton>()) filtered = item.ApplyFilter(filtered);

            return filtered.ToList();
        }

        /// <summary>
        /// Called when a button is selected.
        /// </summary>
        private void OnButtonSelectedCallback(int index) => ((FilterButton)MenuOptions[index]).OnButtonSelected();

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

            SortingFilterButton sortingButton = MenuOptions[0] as SortingFilterButton;

            if (sortingButton != null)
                sortingButton.SetOptionAndMode(SortOption.DateAdded,
                                               SortMode.Descending);

            foreach (ToggableFilterButton filterButton in MenuOptions.OfType<ToggableFilterButton>())
                filterButton.DisableFilter();
        }
    }
}