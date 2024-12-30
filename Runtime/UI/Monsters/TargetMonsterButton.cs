using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Button representing a monster in the choose targets menu.
    /// </summary>
    public class TargetMonsterButton : MonsterButton
    {
        /// <summary>
        /// Reference to the effectiveness text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Effectiveness;

        /// <summary>
        /// Mark if this button is for ally monsters.
        /// </summary>
        [SerializeField]
        private bool IsAllyButton;

        /// <summary>
        /// Reference to the in battle panel of this monster.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(IsAllyButton))]
        private Runtime.Battle.MonsterPanel InBattlePanel;

        /// <summary>
        /// Reference to the in battle panel of this monster.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(IsAllyButton))]
        private Runtime.Battle.MonsterPanel[] AllyPanels;

        /// <summary>
        /// Reference to the trainer panel.
        /// </summary>
        [SerializeField]
        private HidableUiElement TrainerPanel;

        /// <summary>
        /// Index of the current in battle index of this monster.
        /// </summary>
        private int inBattleIndex;

        /// <summary>
        /// Set the values in the button.
        /// </summary>
        /// <param name="target">Target monster.</param>
        /// <param name="effectiveness">Move effectiveness.</param>
        /// <param name="currentInBattleIndex">Index of the current in battle index of this monster.</param>
        /// <param name="showEffectiveness">Show effectiveness?</param>
        /// <param name="isWild">Is it a wild mon?</param>
        public void SetButton(MonsterInstance target,
                              float effectiveness,
                              int currentInBattleIndex,
                              bool showEffectiveness,
                              bool isWild)
        {
            inBattleIndex = currentInBattleIndex;
            Panel.SetMonster(target);
            Effectiveness.SetValue(showEffectiveness ? MoveUtils.EffectivenessToLocalizableKey(effectiveness) : "");
            TrainerPanel.Show(!isWild);
        }

        /// <summary>
        /// When selected, make the panel bounce.
        /// </summary>
        public override void OnSelect()
        {
            base.OnSelect();

            if (IsAllyButton)
                AllyPanels[inBattleIndex].StartBouncing();
            else
                InBattlePanel.StartBouncing();
        }

        /// <summary>
        /// When deselected, stop the panel bounce.
        /// </summary>
        public override void OnDeselect()
        {
            if (IsAllyButton)
                AllyPanels[inBattleIndex].StopBouncing();
            else
                InBattlePanel.StopBouncing();

            base.OnDeselect();
        }
    }
}