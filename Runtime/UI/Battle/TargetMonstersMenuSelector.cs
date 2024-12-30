using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Monsters;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Menu selector for when the player needs to select the targets for a move.
    /// </summary>
    public class TargetMonstersMenuSelector : MenuSelector
    {
        /// <summary>
        /// Set the targets of the menu.
        /// </summary>
        /// <param name="targets">Targets to set.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="valid">Whether the monster is a valid target.</param>
        /// <param name="effectiveness">Effectiveness of the move.</param>
        /// <param name="showEffectiveness">Show a move's effectiveness?</param>
        public void SetMonsters(Battler[] targets,
                                BattleManager battleManager,
                                bool[] valid,
                                float[] effectiveness,
                                bool showEffectiveness = true)
        {
            List<bool> enabledButtons = new();

            for (int i = 0; i < targets.Length; ++i)
            {
                if (valid[i])
                {
                    MenuOptions[i].Show();

                    (BattlerType targetType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(targets[i]);

                    ((TargetMonsterButton)MenuOptions[i]).SetButton(targets[i],
                                                                    effectiveness[i],
                                                                    i == 3 ? 1 : 0,
                                                                    showEffectiveness,
                                                                    targetType == BattlerType.Enemy
                                                                 && battleManager.EnemyType == EnemyType.Wild);
                }
                else
                    MenuOptions[i].Hide();

                enabledButtons.Add(valid[i]);
            }

            UpdateLayout(enabledButtons);
        }
    }
}