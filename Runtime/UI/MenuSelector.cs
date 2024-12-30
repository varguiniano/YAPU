using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Class representing a menu selector.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MenuSelector : HidableUiElement<MenuSelector>, IInputReceiver
    {
        /// <summary>
        /// Event raised when a button is selected.
        /// </summary>
        public Action<int> OnButtonSelected;

        /// <summary>
        /// Event raised when a button is hovered.
        /// </summary>
        public Action<int> OnHovered;

        /// <summary>
        /// Event raised when the back button is pressed.
        /// </summary>
        public Action OnBackSelected;

        /// <summary>
        /// Default selected item.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [PropertyRange(0, "@MenuOptions.Count - 1")]
        [SerializeField]
        [HideIf("@MenuOptions.Count == 0")]
        protected int DefaultSelected;

        /// <summary>
        /// Should the menu request input when shown?
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        private bool RequestInputOnShow = true;

        /// <summary>
        /// Sync the default value with the current value.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        [HideIf(nameof(NoChoosing))]
        protected bool SyncDefaultWithCurrent;

        /// <summary>
        /// Keep the last selected item after submitting.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        [HideIf(nameof(NoChoosing))]
        private bool KeepSelectionWhenSubmitting;

        /// <summary>
        /// Navigation of this menu.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        protected Navigation MenuNavigation = Navigation.Vertical;

        /// <summary>
        /// Should the controls be inverted.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        protected bool InvertedControls = true;

        /// <summary>
        /// Hide the menu when chosen?
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        [HideIf(nameof(NoChoosing))]
        private bool HideWhenChosen = true;

        /// <summary>
        /// Deselect when choosing?
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        [HideIf(nameof(NoChoosing))]
        private bool DeselectWhenChoosing = true;

        /// <summary>
        /// Used when you don't want anything to be chosen.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        private bool NoChoosing;

        /// <summary>
        /// Used when you don't want the suer to be able to go back.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        public bool NoGoingBack;

        /// <summary>
        /// List of the available menu options.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        public List<MenuItem> MenuOptions = new();

        /// <summary>
        /// Show a selector arrow on this menu?
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        public bool ShowSelectorArrow = true;

        /// <summary>
        /// Prefab for the selection arrow.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        [ShowIf(nameof(ShowSelectorArrow))]
        protected GameObject SelectorArrowPrefab;

        /// <summary>
        /// Audio to play when navigating.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        protected AudioReference NavigationAudio;

        /// <summary>
        /// Audio to play when selecting.
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [SerializeField]
        protected AudioReference SelectAudio;

        /// <summary>
        /// Play select audio when going back?
        /// </summary>
        [FoldoutGroup("Menu Configuration")]
        [HideIf(nameof(NoGoingBack))]
        public bool PlayAudioOnBack = true;

        /// <summary>
        /// Can the navigation button be held to scroll faster?
        /// </summary>
        [FoldoutGroup("Input")]
        [SerializeField]
        public bool CanHoldToScrollFaster;

        /// <summary>
        /// Interval to navigate when holding.
        /// </summary>
        [FoldoutGroup("Input")]
        [SerializeField]
        public float HoldNavigationInterval = 0.05f;

        /// <summary>
        /// Currently selected item.
        /// </summary>
        [ReadOnly]
        public int CurrentSelection;

        /// <summary>
        /// Access to the current selected button.
        /// </summary>
        public virtual MenuItem CurrentSelectedButton => MenuOptions[CurrentSelection];

        /// <summary>
        /// Flag to know if we are currently active.
        /// </summary>
        protected bool Active;

        /// <summary>
        /// Reference to the instantiated selector arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [ShowInInspector]
        [ReadOnly]
        protected Transform SelectorArrow;

        /// <summary>
        /// Reference to the instantiated selector arrow.
        /// </summary>
        [FoldoutGroup("References")]
        [ShowInInspector]
        [ReadOnly]
        protected HidableUiElement SelectorArrowShower;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        protected IInputManager InputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        protected IAudioManager AudioManager;

        /// <summary>
        /// Flag to know if it is receiving input.
        /// </summary>
        protected bool ReceivingInput;

        /// <summary>
        /// Flag to know if the navigation is currently being held down.
        /// </summary>
        protected bool Holding;

        /// <summary>
        /// Value of the last input.
        /// </summary>
        protected float LastInput;

        /// <summary>
        /// Routine to handle hold navigation.
        /// </summary>
        protected Coroutine HoldNavigationRoutineReference;

        /// <summary>
        /// Request input on show.
        /// </summary>
        /// <param name="show"></param>
        public override void Show(bool show = true)
        {
            base.Show(show);

            if (InputManager == null) return;

            if (RequestInputOnShow)
                if (show)
                    RequestInput();
                else
                    ReleaseInput();

            Active = show;
        }

        /// <summary>
        /// Set the default as selected.
        /// </summary>
        public virtual void OnStateEnter()
        {
            ReceivingInput = true;

            if (DefaultSelected >= MenuOptions.Count) DefaultSelected = Mathf.Max(MenuOptions.Count - 1, 0);
            if (CurrentSelection >= MenuOptions.Count) CurrentSelection = Mathf.Max(MenuOptions.Count - 1, 0);

            if (ShowSelectorArrow && SelectorArrow == null)
            {
                SelectorArrow = Instantiate(SelectorArrowPrefab, transform).transform;
                SelectorArrowShower = SelectorArrow.GetComponent<HidableUiElement>();
                SelectorArrowShower.Show(false);
            }

            EventSystem.current.SetSelectedGameObject(null);

            StartCoroutine(SelectDefaultAfterAFrame());

            HoldNavigationRoutineReference ??= StartCoroutine(HoldNavigation());
        }

        /// <summary>
        /// Deselect.
        /// </summary>
        public virtual void OnStateExit()
        {
            if (HoldNavigationRoutineReference != null)
            {
                StopCoroutine(HoldNavigationRoutineReference);
                HoldNavigationRoutineReference = null;
            }

            ReceivingInput = false;
            Holding = false;

            Deselect(CurrentSelection);

            EventSystem.current.SetSelectedGameObject(null);

            if (SelectorArrow == null) return;
            SelectorArrow.DOKill();
            Destroy(SelectorArrow.gameObject);
        }

        /// <summary>
        /// Manually request input for this menu.
        /// </summary>
        public void RequestInput() => InputManager.RequestInput(this);

        /// <summary>
        /// Manually release input for this menu.
        /// </summary>
        public void ReleaseInput() => InputManager.ReleaseInput(this);

        /// <summary>
        /// Reselect the currently selected item.
        /// </summary>
        public void ReselectAfterFrames(int frames = 1) => StartCoroutine(ReselectAfterFramesRoutine(frames));

        /// <summary>
        /// Reselect the currently selected item.
        /// </summary>
        private IEnumerator ReselectAfterFramesRoutine(int frames = 1)
        {
            for (int i = 0; i < frames; ++i) yield return WaitAFrame;

            Select(CurrentSelection, false, force: true);
        }

        /// <summary>
        /// Routine to select the default selection after a frame.
        /// </summary>
        protected virtual IEnumerator SelectDefaultAfterAFrame()
        {
            yield return WaitAFrame;

            if (DefaultSelected < 0 || DefaultSelected >= MenuOptions.Count) DefaultSelected = 0;

            if (ReceivingInput) Select(DefaultSelected, false);
        }

        /// <summary>
        /// Select an item of the menu.
        /// </summary>
        /// <param name="index">Index of the item to select.</param>
        /// <param name="playAudio">Play the navigation audio?</param>
        /// <param name="updateArrow">Should update the arrow when selecting?</param>
        /// <param name="force">Force reselection?</param>
        public virtual void Select(int index, bool playAudio = true, bool updateArrow = true, bool force = false)
        {
            if (index < 0 || index >= MenuOptions.Count) return;

            while (MenuOptions[index].Button.interactable == false)
            {
                index++;

                if (index >= MenuOptions.Count) index = 0;
            }

            if (CurrentSelection != index)
            {
                Deselect(CurrentSelection);
                if (playAudio) AudioManager.PlayAudio(NavigationAudio);
            }

            CurrentSelection = index;

            if (SyncDefaultWithCurrent) DefaultSelected = CurrentSelection;

            if (updateArrow) UpdateSelectorArrowPosition();

            OnHovered?.Invoke(CurrentSelection);

            MenuOptions[CurrentSelection].OnSelect();
        }

        /// <summary>
        /// Deselect an item of the menu.
        /// </summary>
        /// <param name="index">Index of the item to deselect.</param>
        protected virtual void Deselect(int index)
        {
            if (index >= 0 && index < MenuOptions.Count) MenuOptions[index].OnDeselect();
        }

        /// <summary>
        /// Update the layout of this menu.
        /// </summary>
        /// <param name="enabledButtons">List of enabled buttons.</param>
        [Button]
        public virtual void UpdateLayout(List<bool> enabledButtons)
        {
            for (int i = 0; i < enabledButtons.Count; ++i) MenuOptions[i].Show(enabledButtons[i]);

            if (enabledButtons.Any(isEnabled => isEnabled))
            {
                if (enabledButtons.Count <= DefaultSelected) DefaultSelected = 0;

                while (!enabledButtons[DefaultSelected])
                    if (DefaultSelected >= enabledButtons.Count - 1)
                        DefaultSelected = 0;
                    else
                        DefaultSelected++;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetCachedComponent<RectTransform>());

            if (Active) Select(CurrentSelection, false);
        }

        /// <summary>
        /// Move through the menu.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnNavigation(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    LastInput = MenuNavigation == Navigation.Vertical
                                    ? context.ReadValue<Vector2>().y
                                    : context.ReadValue<Vector2>().x;

                    Navigate(LastInput);
                    break;
                case { phase: InputActionPhase.Performed } when CanHoldToScrollFaster:
                    Holding = true;
                    break;
                case { phase: InputActionPhase.Canceled } when CanHoldToScrollFaster:
                    Holding = false;
                    break;
            }
        }

        /// <summary>
        /// Navigation when holding.
        /// </summary>
        protected IEnumerator HoldNavigation()
        {
            while (true)
            {
                if (Holding) Navigate(LastInput);

                yield return new WaitForSeconds(HoldNavigationInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Perform the navigation.
        /// </summary>
        /// <param name="input"></param>
        protected virtual void Navigate(float input)
        {
            if (MenuOptions.Count <= 1) return;

            if (InvertedControls) input *= -1;

            bool validSelection = false;
            int newSelection = CurrentSelection;

            while (!validSelection)
            {
                switch (input)
                {
                    case > 0:
                        if (newSelection == MenuOptions.Count - 1)
                            newSelection = 0;
                        else
                            newSelection++;

                        break;
                    case < 0:
                        if (newSelection == 0)
                            newSelection = MenuOptions.Count - 1;
                        else
                            newSelection--;

                        break;
                }

                if (newSelection >= 0
                 && newSelection < MenuOptions.Count
                 && MenuOptions[newSelection].Button.interactable)
                    validSelection = true;
            }

            Select(newSelection);
        }

        /// <summary>
        /// Select the corresponding button.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnSelect(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (NoChoosing) return;

            AudioManager.PlayAudio(SelectAudio);

            Deselect(CurrentSelection);

            OnButtonSelected?.Invoke(CurrentSelection);

            if (HideWhenChosen) Show(false);

            if (KeepSelectionWhenSubmitting) DefaultSelected = CurrentSelection;

            if (DeselectWhenChoosing)
                EventSystem.current.SetSelectedGameObject(null);
            else
                StartCoroutine(ReselectNextFrameRoutine());
        }

        /// <summary>
        /// Reselect after a frame has passed.
        /// </summary>
        protected IEnumerator ReselectNextFrameRoutine()
        {
            yield return WaitAFrame;

            Select(CurrentSelection);
        }

        /// <summary>
        /// Go back to the previous menu.
        /// To be implemented by inheritors.
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnBack(InputAction.CallbackContext context)
        {
            if (NoGoingBack) return;

            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (PlayAudioOnBack) AudioManager.PlayAudio(SelectAudio);

            Deselect(CurrentSelection);

            EventSystem.current.SetSelectedGameObject(null);

            OnBackSelected?.Invoke();
        }

        /// <summary>
        /// Update the position of the selector arrow after a frame.
        /// </summary>
        protected void UpdateSelectorArrowPositionAfterAFrame() =>
            StartCoroutine(UpdateSelectorArrowPositionAfterAFrameRoutine());

        /// <summary>
        /// Update the position of the selector arrow after a frame.
        /// </summary>
        private IEnumerator UpdateSelectorArrowPositionAfterAFrameRoutine()
        {
            yield return WaitAFrame;
            UpdateSelectorArrowPosition();
        }

        /// <summary>
        /// Update the position of the selector arrow.
        /// </summary>
        protected virtual void UpdateSelectorArrowPosition()
        {
            if (!ShowSelectorArrow) return;

            if (SelectorArrow == null)
            {
                SelectorArrow = Instantiate(SelectorArrowPrefab, transform).transform;
                SelectorArrowShower = SelectorArrow.GetComponent<HidableUiElement>();
                SelectorArrowShower.Show(false);
            }

            if (MenuOptions.Count == 0) return;

            if (SelectorArrowShower.Shown)
                SelectorArrow.DOMove(MenuOptions[CurrentSelection].ArrowSelectorPosition.position, .1f);
            else
            {
                SelectorArrow.position = MenuOptions[CurrentSelection].ArrowSelectorPosition.position;
                SelectorArrowShower.Show();
            }
        }

        /// <summary>
        /// This receiver is of type UI.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name used for input debugging.
        /// </summary>
        /// <returns></returns>
        public virtual string GetDebugName() => name;

        #region Unused input callbacks

        public virtual void OnMovement(InputAction.CallbackContext context)
        {
        }

        public virtual void OnInteract(InputAction.CallbackContext context)
        {
        }

        public virtual void OnMenu(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
        {
        }

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public virtual void OnExtra1(InputAction.CallbackContext context)
        {
        }

        public virtual void OnExtra2(InputAction.CallbackContext context)
        {
        }

        public virtual void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public virtual void OnCancel(InputAction.CallbackContext context)
        {
        }

        public virtual void OnPoint(InputAction.CallbackContext context)
        {
        }

        public virtual void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTextBackspace(InputAction.CallbackContext context)
        {
        }

        public void OnTextSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnTextCancel(InputAction.CallbackContext context)
        {
        }

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        #endregion

        /// <summary>
        /// Enumeration that sets the navigation of this menu.
        /// </summary>
        protected enum Navigation
        {
            Vertical,
            Horizontal
        }
    }
}