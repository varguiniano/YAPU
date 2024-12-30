using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Base class for menu selectors that use virtualization of their elements.
    /// It uses a recyclable scroll view system based on
    /// https://github.com/sinbad/UnityRecyclingListView/blob/master/Source/RecyclingListView.cs
    /// but heavily modified to work with dependency injection and the same methods as the MenuSelector class.
    /// </summary>
    /// <typeparam name="TData">Data class that will be represented in each of the elements of the menu.</typeparam>
    /// <typeparam name="TButtonController">Behaviour controlling each of the buttons to be used in this menu.</typeparam>
    /// <typeparam name="TButtonFactory">Factory that will instantiate the buttons.</typeparam>
    public abstract class VirtualizedMenuSelector<TData, TButtonController, TButtonFactory> : MenuSelector
        where TButtonController : VirtualizedMenuItem where TButtonFactory : GameObjectFactory<TButtonController>
    {
        /// <summary>
        /// Is the scroll in the same object?
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        private bool ScrollInSameObject = true;

        /// <summary>
        /// Reference to the scroll in case it's not on the same object.
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        [HideIf(nameof(ScrollInSameObject))]
        private ScrollRect ScrollReference;

        /// <summary>
        /// Content in which to place the buttons.
        /// </summary>
        [FoldoutGroup("Scroll")]
        [SerializeField]
        private RectTransform Content;

        /// <summary>
        /// Reference to the prefab for data retrieving, not instantiation.
        /// </summary>
        [FoldoutGroup("Pooling")]
        [SerializeField]
        private TButtonController ChildPrefab;

        /// <summary>
        /// Unused pool of buttons.
        /// </summary>
        [FoldoutGroup("Pooling")]
        [ShowInInspector]
        [ReadOnly]
        private TButtonController[] childItems;

        /// <summary>
        /// The amount of vertical padding to add between items.
        /// </summary>
        public float RowPadding = 15f;

        /// <summary>
        /// Minimum height to pre-allocate list items for. Use to prevent allocations on resizing.
        /// </summary>
        public float PreAllocHeight;

        /// <summary>
        /// Reference to the scroll.
        /// </summary>
        private ScrollRect Scroll => ScrollInSameObject ? GetCachedComponent<ScrollRect>() : ScrollReference;

        /// <summary>
        /// Backfield for Scroll.
        /// </summary>
        private ScrollRect scroll;

        /// <summary>
        /// Set the vertical normalized scroll position. 0 is bottom, 1 is top (as with ScrollRect) 
        /// </summary>
        public float VerticalNormalizedPosition
        {
            get => Scroll.verticalNormalizedPosition;
            set
            {
                Scroll.verticalNormalizedPosition = value;
                OnScrollChanged(Scroll.normalizedPosition);
            }
        }

        /// <summary>
        /// Get / set the number of rows in the list. If changed, will cause a rebuild of
        /// the contents of the list. Call Refresh() instead to update contents without changing
        /// length.
        /// </summary>
        public int RowCount
        {
            get => rowCount;
            private set
            {
                if (rowCount == value) return;

                rowCount = value;
                // avoid triggering double refresh due to scroll change from height change
                ignoreScrollChange = true;
                UpdateContentHeight();
                ignoreScrollChange = false;
                Refresh();
            }
        }

        /// <summary>
        /// Backfield for RowCount.
        /// </summary>
        private int rowCount;

        /// <summary>
        /// Access to the current selected button.
        /// </summary>
        public override MenuItem CurrentSelectedButton => GetRowItem(CurrentSelection);

        /// <summary>
        /// Get the height of an individual row.
        /// </summary>
        /// <returns>Height in pixels.</returns>
        private float RowHeight => RowPadding + ChildPrefab.GetCachedComponent<RectTransform>().rect.height;

        /// <summary>
        /// Get the height of the entire viewport.
        /// </summary>
        /// <returns>Height in pixels.</returns>
        private float ViewportHeight => Scroll.viewport.rect.height;

        /// <summary>
        /// The current start index of the circular buffer.
        /// </summary>
        private int childBufferStart;

        /// <summary>
        /// The index into source data which childBufferStart refers to.
        /// </summary>
        private int sourceDataRowStart;

        /// <summary>
        /// Flag to ignore when the scroll is changing.
        /// </summary>
        private bool ignoreScrollChange;

        /// <summary>
        /// Cached value of the previous height of the content.
        /// </summary>
        private float previousBuildHeight;

        /// <summary>
        /// Number of rows to have on the pool above and below the virtualized buttons.
        /// </summary>
        private const int RowsAboveBelow = 1;

        /// <summary>
        /// Cached copy of the data to display.
        /// </summary>
        public List<TData> Data { get; private set; }

        /// <summary>
        /// Button factory.
        /// </summary>
        [Inject]
        private TButtonFactory buttonFactory;

        /// <summary>
        /// Set the default as selected.
        /// </summary>
        public override void OnStateEnter()
        {
            Scroll.onValueChanged.AddListener(OnScrollChanged);
            ignoreScrollChange = false;

            ReceivingInput = true;

            if (DefaultSelected >= Data.Count) DefaultSelected = Mathf.Max(Data.Count - 1, 0);
            if (CurrentSelection >= Data.Count) CurrentSelection = Mathf.Max(Data.Count - 1, 0);

            if (ShowSelectorArrow && SelectorArrow == null)
            {
                SelectorArrow = Instantiate(SelectorArrowPrefab, transform).transform;
                SelectorArrowShower = SelectorArrow.GetComponent<HidableUiElement>();
                SelectorArrowShower.Show(false);
            }

            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

            StartCoroutine(SelectDefaultAfterAFrame());

            HoldNavigationRoutineReference ??= StartCoroutine(HoldNavigation());
        }

        /// <summary>
        /// Deselect.
        /// </summary>
        public override void OnStateExit()
        {
            Scroll.onValueChanged.RemoveListener(OnScrollChanged);

            if (HoldNavigationRoutineReference != null)
            {
                StopCoroutine(HoldNavigationRoutineReference);
                HoldNavigationRoutineReference = null;
            }

            ReceivingInput = false;
            Holding = false;

            Deselect(CurrentSelection);

            if (SelectorArrow == null) return;
            SelectorArrow.DOKill();
            Destroy(SelectorArrow.gameObject);
        }

        /// <summary>
        /// Routine to select the default selection after a frame.
        /// </summary>
        protected override IEnumerator SelectDefaultAfterAFrame()
        {
            yield return WaitAFrame;

            if (Data.Count == 0) yield break;

            if (DefaultSelected < 0 || DefaultSelected >= Data.Count) DefaultSelected = 0;

            if (ReceivingInput) Select(DefaultSelected, false);
        }

        /// <summary>
        /// Reorder stuff to first scroll and then update the arrow position.
        /// </summary>
        /// <param name="index">Index of the item to select.</param>
        /// <param name="playAudio">Play the navigation audio?</param>
        /// <param name="updateArrow">Should update the arrow when selecting?</param>
        /// <param name="force">Force reselection?</param>
        public override void Select(int index, bool playAudio = true, bool updateArrow = true, bool force = false)
        {
            if (index < 0 || index >= Data.Count) return;

            if (CurrentSelection != index || force)
            {
                Deselect(CurrentSelection);
                if (playAudio) AudioManager.PlayAudio(NavigationAudio);
            }

            CurrentSelection = index;

            if (SyncDefaultWithCurrent) DefaultSelected = CurrentSelection;

            UpdateScroll();

            StartCoroutine(FinishSelectionAfterAFrame(index, updateArrow));
        }

        /// <summary>
        /// Finish selecting after a frame has passed and the scroll is updated.
        /// </summary>
        private IEnumerator FinishSelectionAfterAFrame(int index, bool updateArrow)
        {
            if (Data.Count == 0) yield break;

            yield return WaitAFrame;

            OnHovered?.Invoke(CurrentSelection);

            TButtonController item = GetRowItem(index);
            if (item != null) item.OnSelect();

            if (updateArrow) UpdateSelectorArrowPosition();
        }

        /// <summary>
        /// Deselect an item of the menu.
        /// </summary>
        /// <param name="index">Index of the item to deselect.</param>
        protected override void Deselect(int index)
        {
            if (index < 0 || Data == null || index >= Data.Count) return;

            TButtonController item = GetRowItem(index);
            if (item != null) item.OnDeselect();
        }

        /// <summary>
        /// Update the layout of this menu.
        /// </summary>
        /// <param name="enabledButtons">List of enabled buttons.</param>
        [Button]
        public override void UpdateLayout(List<bool> enabledButtons)
        {
            // ALl buttons will always be enabled when using this kind of pooling.
        }

        /// <summary>
        /// Set the buttons to display.
        /// </summary>
        /// <param name="newData">New data to display.</param>
        /// <param name="clearPrevious">Clear the previous buttons?</param>
        public void SetButtons(List<TData> newData, bool clearPrevious = true)
        {
            Data = newData;

            RowCount = Data.Count;

            Refresh(clearPrevious);
        }

        /// <summary>
        /// Remove all the buttons.
        /// </summary>
        public void ClearButtons(bool destroyButtonObjects = false)
        {
            MenuOptions.Clear();
            Clear();
            Data?.Clear();

            if (!destroyButtonObjects) return;

            foreach (TButtonController item in childItems) Destroy(item.gameObject);

            childItems = null;
        }

        /// <summary>
        /// Perform the navigation.
        /// </summary>
        /// <param name="input"></param>
        protected override void Navigate(float input)
        {
            if (Data.Count <= 1) return;

            if (InvertedControls) input *= -1;

            bool validSelection = false;
            int newSelection = CurrentSelection;

            while (!validSelection)
            {
                switch (input)
                {
                    case > 0:
                        if (newSelection == Data.Count - 1)
                            newSelection = 0;
                        else
                            newSelection++;

                        break;
                    case < 0:
                        if (newSelection == 0)
                            newSelection = Data.Count - 1;
                        else
                            newSelection--;

                        break;
                }

                if (newSelection >= 0
                 && newSelection < Data.Count)
                    validSelection = true;
            }

            Select(newSelection);
        }

        /// <summary>
        /// Update the position of the selector arrow.
        /// </summary>
        protected override void UpdateSelectorArrowPosition()
        {
            if (SelectorArrow == null) return;
            if (Data.Count == 0) return;

            if (SelectorArrowShower.Shown)
                SelectorArrow.DOMove(GetRowItem(CurrentSelection).ArrowSelectorPosition.position, .1f);
            else
            {
                SelectorArrow.position = GetRowItem(CurrentSelection).ArrowSelectorPosition.position;
                SelectorArrowShower.Show();
            }
        }

        /// <summary>
        /// Trigger the refreshing of the list content (e.g. if you've changed some values).
        /// Use this if the number of rows hasn't changed but you want to update the contents
        /// for some other reason. All active items will have the ItemCallback invoked. 
        /// </summary>
        public void Refresh(bool clearContents = true)
        {
            ReorganiseContent(clearContents);
            if (!clearContents) SyncData();
        }

        /// <summary>
        /// Quick way of clearing all the content from the list (alias for RowCount = 0)
        /// </summary>
        private void Clear() => RowCount = 0;

        /// <summary>
        /// Scroll the viewport so that a given row is in view, preferably centred vertically.
        /// </summary>
        /// <param name="row"></param>
        [Button]
        [HideInEditorMode]
        [FoldoutGroup("Pooling")]
        public void ScrollToRow(int row) => VerticalNormalizedPosition = GetRowScrollPosition(row);

        /// <summary>
        /// Get the normalised vertical scroll position which would centre the given row in the view,
        /// as best as possible without scrolling outside the bounds of the content.
        /// Use this instead of ScrollToRow if you want to control the actual scrolling yourself.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private float GetRowScrollPosition(int row)
        {
            float rowCentre = (row + 0.5f) * RowHeight;
            float vpHeight = ViewportHeight;
            float halfVpHeight = vpHeight * 0.5f;
            // Clamp to top of content
            float vpTop = Mathf.Max(0, rowCentre - halfVpHeight);
            float vpBottom = vpTop + vpHeight;
            float contentHeight = Scroll.content.sizeDelta.y;

            // clamp to bottom of content
            if (vpBottom > contentHeight) // if content is shorter than vp always stop at 0
                vpTop = Mathf.Max(0, vpTop - (vpBottom - contentHeight));

            // Range for our purposes is between top (0) and top of vp when scrolled to bottom (contentHeight - vpHeight)
            // ScrollRect normalised position is 0 at bottom, 1 at top
            // so inverted range because 0 is bottom and our calc is top-down
            return Mathf.InverseLerp(contentHeight - vpHeight, 0, vpTop);
        }

        /// <summary>
        /// Retrieve the item instance for a given row, IF it is currently allocated for the view.
        /// Because these items are recycled as the view moves, you should not hold on to this item beyond
        /// the site of the call to this method.
        /// </summary>
        /// <param name="row">The row number</param>
        /// <returns>The list view item assigned to this row IF it's within the window the list currently has
        /// allocated for viewing. If row is outside this range, returns null.</returns>
        private TButtonController GetRowItem(int row)
        {
            if (childItems != null
             && row >= sourceDataRowStart
             && row < sourceDataRowStart + childItems.Length
             && // within window 
                row < rowCount)
                // within overall range
                return childItems[WrapChildIndex(childBufferStart + row - sourceDataRowStart)];

            Logger.Warn("No item found for row " + row + ", returning null.");
            return null;
        }

        /// <summary>
        /// Update the data in all the displayed items.
        /// </summary>
        private void SyncData()
        {
            foreach (TButtonController item in childItems)
            {
                int row = item.CurrentRow;

                if (Data.Count > row) PopulateChildData(item, Data[row]);
            }
        }

        /// <summary>
        /// Check the currently virtualized buttons and rebuild them if they are not updated.
        /// </summary>
        /// <returns>True if they were rebuilt.</returns>
        private bool CheckChildItems()
        {
            float vpHeight = ViewportHeight;
            float buildHeight = Mathf.Max(vpHeight, PreAllocHeight);
            bool rebuild = childItems == null || buildHeight > previousBuildHeight;

            if (!rebuild) return false;

            // create a fixed number of children, we'll re-use them when scrolling
            // figure out how many we need, round up
            int childCount = Mathf.RoundToInt(0.5f + buildHeight / RowHeight);
            childCount += RowsAboveBelow * 2; // X before, X after

            if (childItems == null)
                childItems = new TButtonController[childCount];
            else if (childCount > childItems.Length) Array.Resize(ref childItems, childCount);

            for (int i = 0; i < childItems.Length; ++i)
            {
                if (childItems[i] == null) childItems[i] = buttonFactory.CreateUiGameObject(Content);

                childItems[i].GetComponent<RectTransform>().SetParent(Content, false);
                childItems[i].gameObject.SetActive(false);
            }

            previousBuildHeight = buildHeight;

            return true;
        }

        /// <summary>
        /// Called each time the scroll is changed.
        /// </summary>
        /// <param name="normalisedPos">New scroll position.</param>
        private void OnScrollChanged(Vector2 normalisedPos)
        {
            // This is called when scroll bar is moved *and* when viewport changes size
            if (!ignoreScrollChange) ReorganiseContent(false);
        }

        /// <summary>
        /// Called to refresh and reorganize the content in the list.
        /// </summary>
        /// <param name="clearContents">Clear the entire contents?</param>
        protected virtual void ReorganiseContent(bool clearContents)
        {
            if (clearContents)
            {
                Scroll.StopMovement();
                Scroll.verticalNormalizedPosition = 1; // 1 == top
            }

            bool childrenChanged = CheckChildItems();
            bool populateAll = childrenChanged || clearContents;

            // Figure out which is the first virtual slot visible
            float yMin = Scroll.content.localPosition.y;

            // round down to find first visible
            int firstVisibleIndex = (int) (yMin / RowHeight);

            // we always want to start our buffer before
            int newRowStart = firstVisibleIndex - RowsAboveBelow;

            // If we've moved too far to be able to reuse anything, same as init case
            int diff = newRowStart - sourceDataRowStart;

            if (populateAll || Mathf.Abs(diff) >= childItems.Length)
            {
                sourceDataRowStart = newRowStart;
                childBufferStart = 0;
                int rowIdx = newRowStart;

                foreach (TButtonController item in childItems) UpdateChild(item, rowIdx++);
            }
            else if (diff != 0)
            {
                // we scrolled forwards or backwards within the tolerance that we can re-use some of what we have
                // Move our window so that we just re-use from back and place in front
                // children which were already there and contain correct data won't need changing
                int newBufferStart = (childBufferStart + diff) % childItems.Length;

                if (diff < 0)
                {
                    // window moved backwards
                    for (int i = 1; i <= -diff; ++i)
                    {
                        int bufI = WrapChildIndex(childBufferStart - i);
                        int rowIdx = sourceDataRowStart - i;
                        UpdateChild(childItems[bufI], rowIdx);
                    }
                }
                else
                {
                    // window moved forwards
                    int prevLastBufIdx = childBufferStart + childItems.Length - 1;
                    int prevLastRowIdx = sourceDataRowStart + childItems.Length - 1;

                    for (int i = 1; i <= diff; ++i)
                    {
                        int bufI = WrapChildIndex(prevLastBufIdx + i);
                        int rowIdx = prevLastRowIdx + i;
                        UpdateChild(childItems[bufI], rowIdx);
                    }
                }

                sourceDataRowStart = newRowStart;
                childBufferStart = newBufferStart;
            }
        }

        /// <summary>
        /// Get the index in the virtualized buttons of an element by its global index.
        /// </summary>
        /// <param name="idx">Global index.</param>
        /// <returns>Index in the virtualized buttons.</returns>
        private int WrapChildIndex(int idx)
        {
            while (idx < 0) idx += childItems.Length;

            return idx % childItems.Length;
        }

        /// <summary>
        /// Update the value of a single virtualized child.
        /// </summary>
        /// <param name="child">Child to update.</param>
        /// <param name="rowIdx">Its row index.</param>
        private void UpdateChild(TButtonController child, int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= rowCount)
                // Out of range of data, can happen
                child.gameObject.SetActive(false);
            else
            {
                // Move to correct location
                Rect childRect = ChildPrefab.GetCachedComponent<RectTransform>().rect;
                Vector2 pivot = ChildPrefab.GetCachedComponent<RectTransform>().pivot;
                float yTopPos = RowHeight * rowIdx;
                float yPos = yTopPos + (1f - pivot.y) * childRect.height;
                float xPos = 0 + pivot.x * childRect.width;
                child.GetCachedComponent<RectTransform>().anchoredPosition = new Vector2(xPos, -yPos);
                child.NotifyCurrentAssignment(rowIdx);

                PopulateChildData(child, Data[rowIdx]);

                child.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Populate a child with its corresponding data.
        /// </summary>
        /// <param name="child">Child to populate.</param>
        /// <param name="childData">Data to populate.</param>
        protected abstract void PopulateChildData(TButtonController child, TData childData);

        /// <summary>
        /// Update the height of the content object.
        /// </summary>
        private void UpdateContentHeight()
        {
            float height = ChildPrefab.GetCachedComponent<RectTransform>().rect.height * rowCount
                         + (rowCount - 1) * RowPadding;

            // apparently 'sizeDelta' is the way to set w / h 
            Vector2 sz = Scroll.content.sizeDelta;
            Scroll.content.sizeDelta = new Vector2(sz.x, height);
        }

        /// <summary>
        /// Update the scroll position based on the current button.
        /// </summary>
        private void UpdateScroll()
        {
            if (Data.Count < 2) return;
            ScrollToRow(CurrentSelection);
        }
    }
}