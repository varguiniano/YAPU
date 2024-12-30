using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs.MoveTutor;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Behaviour to control a panel that shows move info.
    /// </summary>
    public class MoveInfoPanel : HidableUiElement<MoveInfoPanel>
    {
        /// <summary>
        /// Reference to the type badge.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TypeBadge TypeBadge;

        /// <summary>
        /// Reference to the move name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro MoveName;

        /// <summary>
        /// Reference to the PP text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text PPText;

        /// <summary>
        /// Reference to the category icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoveCategoryIcon CategoryIcon;

        /// <summary>
        /// Reference to the power text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Power;

        /// <summary>
        /// Reference to the accuracy text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Accuracy;

        /// <summary>
        /// Reference to the description text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Position when shown.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        [ShowIf(nameof(AnimateMovingShowing))]
        private Transform ShownPosition;

        /// <summary>
        /// Position when hidden.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        [ShowIf(nameof(AnimateMovingShowing))]
        private Transform HiddenPosition;

        /// <summary>
        /// Should auto subscribe to a menu?
        /// </summary>
        [FoldoutGroup("AutoMenu")]
        [SerializeField]
        private bool AutoSubscribeToMenu = true;

        /// <summary>
        /// Reference to the move selector menu.
        /// </summary>
        [FoldoutGroup("AutoMenu")]
        [SerializeField]
        [ShowIf(nameof(AutoSubscribeToMenu))]
        private MenuSelector MoveSelector;

        /// <summary>
        /// Animate being shown?
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private bool AnimateMovingShowing = true;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        [ShowIf(nameof(AnimateMovingShowing))]
        private float AnimationDuration;

        /// <summary>
        /// Are we in battle?
        /// </summary>
        private bool inBattle;

        /// <summary>
        /// Owner of the move in battle.
        /// </summary>
        private Battler moveOwner;

        /// <summary>
        /// Target of the move in battle.
        /// </summary>
        private Battler moveTarget;

        /// <summary>
        /// Owner of the move out of battle.
        /// </summary>
        private MonsterInstance outOfBattleOwner;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Subscribe to the hover event.
        /// </summary>
        private void OnEnable()
        {
            if (AutoSubscribeToMenu) MoveSelector.OnHovered += OnMoveHovered;
        }

        /// <summary>
        /// Unsubscribe from the hover event.
        /// </summary>
        private void OnDisable()
        {
            if (AutoSubscribeToMenu) MoveSelector.OnHovered -= OnMoveHovered;
        }

        /// <summary>
        /// Show the move out of battle.
        /// </summary>
        /// <param name="owner">Owner of the move when out of battle.</param>
        /// <param name="show">Show or hide?</param>
        public void ShowOutOfBattle(MonsterInstance owner = null, bool show = true)
        {
            inBattle = false;
            moveOwner = null;
            moveTarget = null;
            outOfBattleOwner = owner;
            battleManager = null;
            Show(show);
        }

        /// <summary>
        /// Show the move in battle.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="target">Target of the move if there is.</param>
        /// <param name="manager">Battle manager.</param>
        public void ShowInBattle(bool show, Battler owner, Battler target, BattleManager manager)
        {
            inBattle = true;
            moveOwner = owner;
            moveTarget = target;
            outOfBattleOwner = moveOwner;
            battleManager = manager;
            Show(show);
        }

        /// <summary>
        /// Show or hide the panel.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        public override void Show(bool show = true) => StartCoroutine(ShowRoutine(show));

        /// <summary>
        /// Show or hide the panel.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        private IEnumerator ShowRoutine(bool show)
        {
            // Make sure it stays in place.
            Transform transformReference = transform;
            transformReference.DOKill();

            yield return WaitAFrame;

            if (AnimateMovingShowing)
                transformReference.position =
                    Shown ? ShownPosition.position : HiddenPosition.position;

            if (show)
            {
                // ReSharper disable once BaseMethodCallWithDefaultParameter
                base.Show();

                if (AnimateMovingShowing)
                    transform.DOMove(ShownPosition.position, AnimationDuration)
                             .SetEase(Ease.OutBack);
            }
            else if (AnimateMovingShowing)
                transform.DOMove(HiddenPosition.position, AnimationDuration)
                         .SetEase(Ease.InBack)
                         .OnComplete(() => base.Show(false));
        }

        /// <summary>
        /// Called when a move is hovered on the move selector.
        /// </summary>
        /// <param name="index">Index of the hovered move.</param>
        private void OnMoveHovered(int index) => SetMove(RetrieveDataFromMenu(index));

        /// <summary>
        /// Set the move to display.
        /// </summary>
        /// <param name="move">Move to display.</param>
        public void SetMove(Move move) => SetMove(new MoveSlot(move));

        /// <summary>
        /// Set the move to display.
        /// </summary>
        /// <param name="move">Move to display.</param>
        public void SetMove(MoveSlot move)
        {
            TypeBadge.SetType(inBattle
                                  ? move.Move.GetMoveTypeInBattle(moveOwner, battleManager)
                                  : move.Move.GetMoveType(outOfBattleOwner, settings));

            MoveName.SetValue(move.Move.LocalizableName);
            PPText.SetText(move.CurrentPP + "/" + move.MaxPP);

            CategoryIcon.SetIcon(move.Move.GetMoveCategory(moveOwner, moveTarget, false, battleManager));

            if (inBattle)
                Accuracy.SetText(move.Move.HasInfiniteAccuracy(moveOwner, moveTarget, battleManager)
                                     ? "∞"
                                     : move.Move.GetPreStageAccuracy(moveOwner, moveTarget, false, battleManager)
                                           .ToString());
            else
                Accuracy.SetText(move.Move.HasInfiniteAccuracy()
                                     ? "∞"
                                     : move.Move.GetOutOfBattleAccuracy().ToString());

            if (move.Move is DamageMove damageMove
             && (inBattle
                     ? damageMove.GetMovePowerInBattle(battleManager, moveOwner, moveTarget, false)
                     : damageMove.GetMovePower(outOfBattleOwner))
              > -1)
                Power.SetText(inBattle
                                  ? damageMove.GetMovePowerInBattle(battleManager, moveOwner, moveTarget, false)
                                              .ToString()
                                  : damageMove.GetMovePower(outOfBattleOwner).ToString());
            else
                Power.SetText("-");

            Description.SetValue(move.Move.LocalizableDescription);
        }

        /// <summary>
        /// Retrieve the data from the menu.
        /// </summary>
        /// <param name="index">Index to retrieve.</param>
        /// <returns>The data</returns>
        private MoveSlot RetrieveDataFromMenu(int index)
        {
            MoveSelectionMenu moveMenu = MoveSelector as MoveSelectionMenu;

            return moveMenu == null
                       ? ((MoveButton) MoveSelector.MenuOptions[index]).Move
                       : new MoveSlot(moveMenu.Data[index]);
        }
    }
}