using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Behaviour of a button that represents a monster.
    /// </summary>
    [RequireComponent(typeof(MonsterPanel))]
    public class MonsterButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the attached monster panel.
        /// </summary>
        public MonsterPanel Panel => GetCachedComponent<MonsterPanel>();

        /// <summary>
        /// Position where to put the context menu when opened.
        /// </summary>
        [FoldoutGroup("References")]
        public Transform ContextMenuPosition;

        /// <summary>
        /// Reference to this button's monster.
        /// </summary>
        public MonsterInstance Monster => Panel.GetMonster();
    }
}