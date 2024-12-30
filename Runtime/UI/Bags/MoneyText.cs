using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    /// <summary>
    /// Behaviour to display the money of a bag into a text.
    /// Adds a money symbol depending on the language. 
    /// </summary>
    public class MoneyText : LocalizedTextMeshPro
    {
        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Set the money to be displayed on the text.
        /// </summary>
        /// <param name="bag">Bag to get the money from.</param>
        public void SetMoney(Bag bag) => SetMoney(bag.Money);

        /// <summary>
        /// Set the money to be displayed on the text.
        /// </summary>
        /// <param name="amount">Amount to display.</param>
        public void SetMoney(uint amount) => UpdateText(MoneyHelper.BuildMoneyString(amount, settings, Localizer));
    }
}