using System.Globalization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Menu item that represents a move button.
    /// </summary>
    public class MoveButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the left section of the button.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image LeftSection;

        /// <summary>
        /// Reference to the type icon image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image TypeIcon;

        /// <summary>
        /// Reference to the name of the move.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro MoveName;

        /// <summary>
        /// Reference to the name of the move.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform MoveNameTransform;

        /// <summary>
        /// Reference to the effectiveness of the move (when applicable).
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Effectiveness;

        /// <summary>
        /// Reference to the power of the move.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Power;

        /// <summary>
        /// Reference to the accuracy of the move.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Accuracy;

        /// <summary>
        /// Reference to the category icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoveCategoryIcon Category;

        /// <summary>
        /// Reference to the PP of the move.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text PP;

        /// <summary>
        /// Y position of the name when there is no effectiveness.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float NoEffectivenessNameY;

        /// <summary>
        /// Y position of the name when there is effectiveness shown.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float ShownEffectivenessNameY = 15;

        /// <summary>
        /// Flag to know if the move is disabled.
        /// </summary>
        [ReadOnly]
        public bool IsDisabled;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Move of this button.
        /// </summary>
        public MoveSlot Move { get; private set; }

        /// <summary>
        /// Set the move for this button.
        /// </summary>
        /// <param name="move">Move that it represents.</param>
        /// <param name="owner">Move owner.</param>
        /// <param name="useEffectiveness">Show effectiveness?</param>
        /// <param name="effectiveness">Effectiveness.</param>
        /// <param name="showDisabledIfCantUse">Make the move look disabled if it can't be used.</param>
        /// <param name="canBeUsed">Can the move be used?</param>
        /// <param name="inBattle">Are we in battle?</param>
        /// <param name="battleOwner">Owner in battle.</param>
        /// <param name="battleTarget">Target of this move in battle.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void SetMove(MoveSlot move,
                            MonsterInstance owner,
                            bool useEffectiveness = true,
                            float effectiveness = 1,
                            bool showDisabledIfCantUse = false,
                            bool canBeUsed = true,
                            bool inBattle = false,
                            Battler battleOwner = null,
                            Battler battleTarget = null,
                            BattleManager battleManager = null)
        {
            if (move.Move == null)
            {
                Show(false);
                return;
            }

            Move = move;

            if (inBattle)
            {
                LeftSection.color = Move.Move.GetMoveTypeInBattle(battleOwner, battleManager).Color;
                TypeIcon.sprite = Move.Move.GetMoveTypeInBattle(battleOwner, battleManager).IconOverColor;
            }
            else
            {
                LeftSection.color = Move.Move.GetMoveType(owner, settings).Color;
                TypeIcon.sprite = Move.Move.GetMoveType(owner, settings).IconOverColor;
            }

            MoveName.SetValue(Move.Move.LocalizableName);
            Category.SetIcon(Move.Move.GetMoveCategory(battleOwner, battleTarget, false, battleManager));
            PP.SetText(Move.CurrentPP + "/" + Move.MaxPP);

            Effectiveness.enabled = useEffectiveness;

            Vector3 namePosition = MoveNameTransform.localPosition;

            if (useEffectiveness)
            {
                namePosition.y = ShownEffectivenessNameY;
                Effectiveness.SetValue(MoveUtils.EffectivenessToLocalizableKey(effectiveness));
            }
            else
            {
                namePosition.y = NoEffectivenessNameY;
                Effectiveness.Text.SetText("");
            }

            MoveNameTransform.localPosition = namePosition;

            if (inBattle)
                Accuracy.SetText(Move.Move.HasInfiniteAccuracy(battleOwner, battleTarget, battleManager)
                                     ? "∞"
                                     : Move.Move.GetPreStageAccuracy(battleOwner, battleTarget, false, battleManager)
                                           .ToString());
            else
                Accuracy.SetText(Move.Move.HasInfiniteAccuracy() ? "∞" : Move.Move.GetOutOfBattleAccuracy().ToString());

            if (inBattle)
            {
                DamageMove damageMove = Move.Move.GetDamageMove(battleOwner, battleTarget, battleManager);

                if (damageMove != null
                 && damageMove.GetMovePowerInBattle(battleManager, battleOwner, battleTarget, false) > -1)
                    Power.SetText(damageMove.GetMovePowerInBattle(battleManager, battleOwner, battleTarget, false)
                                            .ToString(CultureInfo.InvariantCulture));
                else
                    Power.SetText("");
            }
            else
            {
                if (Move.Move is DamageMove damageMove && damageMove.GetMovePower(owner) > -1)
                    Power.SetText(damageMove.GetMovePower(owner).ToString(CultureInfo.InvariantCulture));
                else
                    Power.SetText("");
            }

            IsDisabled = false;
            Button.interactable = true;

            Show(true);

            if (!showDisabledIfCantUse) return;
            if (canBeUsed) return;

            Button.interactable = false;
            IsDisabled = true;
        }

        /// <summary>
        /// A much simple version of the set move method that allows for displaying moves without a slot or owner.
        /// </summary>
        /// <param name="move">Move to set.</param>
        public void SetMove(Move move) => SetMove(new MoveSlot(move), null, useEffectiveness: false);

        /// <summary>
        /// Factory for dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<MoveButton>
        {
        }
    }
}