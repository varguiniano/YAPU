using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for a storage filter button that discriminates whether the monster is holding an item.
    /// </summary>
    public class IsHoldingItemFilterButton : ToggableFilterButton
    {
        /// <summary>
        /// Holding or not holding filter.
        /// </summary>
        private bool holdingItem;

        /// <summary>
        /// Called when the button is selected.
        /// </summary>
        public override void OnButtonSelected() =>
            DialogManager.ShowChoiceMenu(new List<string>()
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         OnOptionChosen);

        /// <summary>
        /// Called when an option is chosen.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private void OnOptionChosen(int index)
        {
            FilterEnabled = true;

            holdingItem = index == 0;

            UpdateText();
        }

        /// <summary>
        /// Update the button text.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(Localizer["Menu/Pokemon/Cloud/SortAndFilter/IsHoldingItem"]
                       + " "
                       + (FilterEnabled ? Localizer[holdingItem ? "Common/True" : "Common/False"] : "-"));

        /// <summary>
        /// Apply the filter.
        /// </summary>
        protected override IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original) =>
            original.Where(monster => holdingItem ? monster.HeldItem != null : monster.HeldItem == null);
    }
}