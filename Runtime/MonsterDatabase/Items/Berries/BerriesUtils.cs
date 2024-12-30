using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Utilities class for berry related stuff.
    /// </summary>
    public static class BerriesUtils
    {
        /// <summary>
        /// Cook a dish with the given berries and return the dish's stats.
        /// This is a simplified version of: https://bulbapedia.bulbagarden.net/wiki/Poffin#Flavors
        /// and https://bulbapedia.bulbagarden.net/wiki/Poffin#Smoothness
        /// </summary>
        /// <param name="berries">Berries to use for the cooking.</param>
        /// <param name="logger">Logger to use.</param>
        /// <returns>The dishes stats.</returns>
        public static (SerializableDictionary<Flavour, int> flavours, uint smoothness) CookDish(List<Berry> berries,
            ILog logger)
        {
            SerializableDictionary<Flavour, int> flavours = new();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Berry berry in berries)
            {
                foreach (KeyValuePair<Flavour, byte> datum in berry.FlavourData)
                    if (datum.Value != 0)
                        if (flavours.ContainsKey(datum.Key))
                            flavours[datum.Key] += datum.Value;
                        else
                            flavours.Add(datum.Key, datum.Value);
            }

            SerializableDictionary<Flavour, int> tempFlavours = new();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<Flavour, int> pair in flavours)
            {
                int weakenFactor = flavours.ContainsKey(pair.Key.GetWeakeningFlavour())
                                       ? flavours[pair.Key.GetWeakeningFlavour()]
                                       : 0;

                tempFlavours.Add(pair.Key, pair.Value - weakenFactor);
            }

            flavours = tempFlavours.ShallowClone();

            int negativeSubtraction = flavours.Count<ObjectPair<Flavour, int>>(pair => pair.Value < 0);

            tempFlavours.Clear();

            foreach (Flavour key in flavours.Keys)
            {
                int value = flavours[key] - negativeSubtraction;
                if (value > 0) tempFlavours[key] = value;
            }

            flavours = tempFlavours.ShallowClone();

            tempFlavours.Clear();

            foreach (KeyValuePair<Flavour, int> pair in flavours)
                tempFlavours[pair.Key] = pair.Value > 100 ? 100 : pair.Value;

            flavours = tempFlavours.ShallowClone();

            uint smoothness =
                (uint) Mathf.Max(Mathf.RoundToInt((float) berries.Average(berry => berry.Smoothness)
                                                - 2 * berries.Count),
                                 0);

            StringBuilder debugText = new("Created a dish with ");

            foreach (KeyValuePair<Flavour, int> pair in flavours)
            {
                debugText.Append(pair.Key);
                debugText.Append(": ");
                debugText.Append(pair.Value);
                debugText.Append(", ");
            }

            debugText.Append("Smoothness: ");
            debugText.Append(smoothness);
            debugText.Append(".");

            logger.Info(debugText);

            return (flavours, smoothness);
        }

        /// <summary>
        /// Get the localized description of a liked flavour.
        /// </summary>
        /// <param name="flavour">Flavour to get.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The description for the monster summary.</returns>
        public static string GetLikedFlavourLocalizedDescription(this Flavour flavour, ILocalizer localizer) =>
            localizer["Flavour/" + flavour + "/LikedSummary"];

        /// <summary>
        /// Get the localized description of a disliked flavour.
        /// </summary>
        /// <param name="flavour">Flavour to get.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The description for the monster summary.</returns>
        public static string GetDislikedFlavourLocalizedDescription(this Flavour flavour, ILocalizer localizer) =>
            localizer["Flavour/" + flavour + "/DislikedSummary"];

        /// <summary>
        /// Get the condition boosted by a flavour.
        /// </summary>
        /// <param name="flavour">Flavour to check.</param>
        /// <returns>Condition boosted.</returns>
        public static Condition GetBoostedCondition(this Flavour flavour) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            flavour switch
            {
                Flavour.Spicy => Condition.Cool,
                Flavour.Dry => Condition.Beautiful,
                Flavour.Sweet => Condition.Cute,
                Flavour.Bitter => Condition.Clever,
                Flavour.Sour => Condition.Tough,
                _ => throw new ArgumentOutOfRangeException(nameof(flavour), flavour, null)
            };

        /// <summary>
        /// Get the flavour that weakens the given flavour when cooking a dish.
        /// </summary>
        /// <param name="flavour">Flavour to check.</param>
        /// <returns>Weakener flavour.</returns>
        public static Flavour GetWeakeningFlavour(this Flavour flavour) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            flavour switch
            {
                Flavour.Spicy => Flavour.Dry,
                Flavour.Dry => Flavour.Sweet,
                Flavour.Sweet => Flavour.Bitter,
                Flavour.Bitter => Flavour.Sour,
                Flavour.Sour => Flavour.Spicy,
                _ => throw new ArgumentOutOfRangeException(nameof(flavour), flavour, null)
            };

        /// <summary>
        /// Get the localizable string of a flavour.
        /// </summary>
        /// <param name="flavour">Flavour to get.</param>
        /// <returns>Its localizable key.</returns>
        public static string ToLocalizableString(this Flavour flavour) => "Flavour/" + flavour;

        /// <summary>
        /// Get the localizable string of a firmness.
        /// </summary>
        /// <param name="firmness">Firmness to get.</param>
        /// <returns>Its localizable key.</returns>
        public static string ToLocalizableString(this Firmness firmness) => "Firmness/" + firmness;
    }
}