using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.UI.Ribbons;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Tab in the monster summary to display the ribbons.
    /// </summary>
    public class SummaryRibbonTab : SummaryTab
    {
        /// <summary>
        /// Prefab for the ribbon panel
        /// </summary>
        [SerializeField]
        private RibbonPanel PanelPrefab;

        /// <summary>
        /// Container for the panels.
        /// </summary>
        [SerializeField]
        private Transform Container;

        /// <summary>
        /// Reference to the instanced ribbons in scene.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private List<RibbonPanel> InstancedRibbons;

        /// <summary>
        /// Display the ribbons for this monster.
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager) =>
            StartCoroutine(SetDataRoutine(monster));

        /// <summary>
        /// Display the ribbons for this monster.
        /// </summary>
        /// <param name="monster"></param>
        private IEnumerator SetDataRoutine(MonsterInstance monster)
        {
            foreach (RibbonPanel ribbonPanel in InstancedRibbons) Destroy(ribbonPanel.gameObject);

            InstancedRibbons.Clear();

            yield return WaitAFrame;

            foreach (Ribbon ribbon in monster.Ribbons)
            {
                RibbonPanel panel = Instantiate(PanelPrefab, Container);
                panel.SetRibbon(ribbon);
                InstancedRibbons.Add(panel);
            }
        }
    }
}