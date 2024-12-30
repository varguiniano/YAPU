using System.Text;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.MoveMachines
{
    /// <summary>
    /// Class representing a machine that can teach a move.
    /// </summary>
    public class MoveMachine : Item
    {
        /// <summary>
        /// Is the machine spent when used?
        /// </summary>
        [FoldoutGroup("Move Machine", -1)]
        public bool IsSpentOnUse;

        /// <summary>
        /// Move that this machine teaches.
        /// </summary>
        [FoldoutGroup("Move Machine")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        public Move Move;

        /// <summary>
        /// The machine can stack is it's spent on use.
        /// </summary>
        public override bool CanStack => IsSpentOnUse;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsedOnTarget => true;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// Can only be used on a monster out of battle.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;

        /// <summary>
        /// The name is the name of the machine plus the name of the move.
        /// </summary>
        public override string GetName(ILocalizer localizer) =>
            base.GetName(localizer) + " " + localizer[Move.LocalizableName];

        /// <summary>
        /// The description is the description of the move plus some stats.
        /// </summary>
        public override string GetDescription(ILocalizer localizer, PlayerSettings playerSettings)
        {
            StringBuilder description = new();

            if (Move is DamageMove damageMove)
            {
                description.Append(localizer["Moves/Power"]);
                description.Append(": ");
                description.Append(damageMove.GetMovePower());
                description.Append(" - ");
                description.Append(localizer["Stats/Accuracy"]);
                description.Append(": ");
                description.Append(damageMove.GetOutOfBattleAccuracy());
                description.Append(" - ");
                description.Append("PP");
                description.Append(": ");
                description.AppendLine(damageMove.MaxPowerPoints.ToString());
            }

            description.Append(localizer[Move.LocalizableDescription]);

            return description.ToString();
        }
    }
}