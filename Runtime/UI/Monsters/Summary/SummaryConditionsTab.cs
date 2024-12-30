using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Summary tab for the monster conditions.
    /// </summary>
    public class SummaryConditionsTab : SummaryTab
    {
        /// <summary>
        /// Reference to the cool field.
        /// </summary>
        [SerializeField]
        private TMP_Text Cool;

        /// <summary>
        /// Reference to the cool bar.
        /// </summary>
        [SerializeField]
        private Slider CoolBar;
        
        /// <summary>
        /// Reference to the beautiful field.
        /// </summary>
        [SerializeField]
        private TMP_Text Beautiful;
        
        /// <summary>
        /// Reference to the Beautiful bar.
        /// </summary>
        [SerializeField]
        private Slider BeautifulBar;
        
        /// <summary>
        /// Reference to the cute field.
        /// </summary>
        [SerializeField]
        private TMP_Text Cute;
        
        /// <summary>
        /// Reference to the Cute bar.
        /// </summary>
        [SerializeField]
        private Slider CuteBar;
        
        /// <summary>
        /// Reference to the clever field.
        /// </summary>
        [SerializeField]
        private TMP_Text Clever;
        
        /// <summary>
        /// Reference to the Clever bar.
        /// </summary>
        [SerializeField]
        private Slider CleverBar;
        
        /// <summary>
        /// Reference to the tough field.
        /// </summary>
        [SerializeField]
        private TMP_Text Tough;
        
        /// <summary>
        /// Reference to the Tough bar.
        /// </summary>
        [SerializeField]
        private Slider ToughBar;
        
        /// <summary>
        /// Reference to the Sheen field.
        /// </summary>
        [SerializeField]
        private TMP_Text Sheen;
        
        /// <summary>
        /// Reference to the Sheen bar.
        /// </summary>
        [SerializeField]
        private Slider SheenBar;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;
        
        /// <summary>
        /// Set the monster data.
        /// </summary>
        /// <param name="monster">Reference to the monster.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            Cool.SetText(monster.Conditions[Condition.Cool].ToString());
            CoolBar.value = monster.Conditions[Condition.Cool];
            Beautiful.SetText(monster.Conditions[Condition.Beautiful].ToString());
            BeautifulBar.value = monster.Conditions[Condition.Beautiful];
            Cute.SetText(monster.Conditions[Condition.Cute].ToString());
            CuteBar.value = monster.Conditions[Condition.Cute];
            Clever.SetText(monster.Conditions[Condition.Clever].ToString());
            CleverBar.value = monster.Conditions[Condition.Clever];
            Tough.SetText(monster.Conditions[Condition.Tough].ToString());
            ToughBar.value = monster.Conditions[Condition.Tough];
            
            Sheen.SetText(monster.Sheen.ToString());
            SheenBar.maxValue = settings.MaxSheen;
            SheenBar.value = monster.Sheen;
        }
    }
}