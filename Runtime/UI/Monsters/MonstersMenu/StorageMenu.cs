using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu
{
    /// <summary>
    /// Controller for the storage menu.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class StorageMenu : VirtualizedMenuSelector<MonsterInstance, StorageMonsterButton,
        StorageMonsterButton.Factory>
    {
        /// <summary>
        /// Reference to the warning saying the cloud is empty.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement EmptyWarning;

        /// <summary>
        /// Is this a choosing dialog?
        /// </summary>
        [HideInInspector]
        public bool IsChoosingDialog;

        /// <summary>
        /// Compatibility checker to use when showing the monster's compatibility on a choosing dialog.
        /// </summary>
        [HideInInspector]
        public MonsterCompatibilityChecker CompatibilityChecker;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Show the empty warning before reorganising if there are no monsters in storage.
        /// </summary>
        /// <param name="clearContents">Clear the entire contents?</param>
        protected override void ReorganiseContent(bool clearContents)
        {
            EmptyWarning.Show(RowCount == 0);

            base.ReorganiseContent(clearContents);
        }

        /// <summary>
        /// Populate the data of a button.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to populate.</param>
        protected override void PopulateChildData(StorageMonsterButton child, MonsterInstance childData)
        {
            child.Panel.SetMonster(childData);
            child.SetCompatibility(!IsChoosingDialog || CompatibilityChecker.IsMonsterCompatible(childData, settings));

            if (IsChoosingDialog)
                child.SetCompatibilityText(CompatibilityChecker.GetNotCompatibleLocalizationKey(childData));
        }
    }
}