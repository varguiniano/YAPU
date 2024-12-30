using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Natures
{
    /// <summary>
    /// Object that represents a monster nature.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Nature", fileName = "Nature")]
    public class Nature : LocalizableMonsterDatabaseScriptable<Nature>
    {
        /// <summary>
        /// All natures should start with Natures/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Natures/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Stat favored by this nature.
        /// </summary>
        [FoldoutGroup("Stats")]
        public Stat FavoredStat;

        /// <summary>
        /// Stat unfavored by this nature.
        /// </summary>
        [FoldoutGroup("Stats")]
        public Stat UnfavoredStat;

        /// <summary>
        /// Get the stat multiplier for the given stat.
        /// </summary>
        /// <param name="stat">Stat to check.</param>
        /// <returns>The multiplier this nature gives to that stat.</returns>
        public float GetStatMultiplier(Stat stat)
        {
            if (FavoredStat == UnfavoredStat) return 1;

            if (FavoredStat == stat) return 1.1f;
            if (UnfavoredStat == stat) return .9f;

            return 1;
        }

        /// <summary>
        /// Get the liked flavour for this nature.
        /// </summary>
        public Flavour GetLikedFlavour() => GetFlavourForStat(FavoredStat);

        /// <summary>
        /// Get the disliked flavour for this nature.
        /// </summary>
        public Flavour GetDislikedFlavour() => GetFlavourForStat(UnfavoredStat);

        /// <summary>
        /// Get the flavour for a certain stat.
        /// It will return neutral if favored and unfavored stats match.
        /// </summary>
        /// <param name="stat">Stat to check.</param>
        /// <returns>The corresponding flavour.</returns>
        private Flavour GetFlavourForStat(Stat stat)
        {
            if (FavoredStat == UnfavoredStat) return Flavour.Neutral;

            return stat switch
            {
                Stat.Attack => Flavour.Spicy,
                Stat.Defense => Flavour.Sour,
                Stat.SpecialAttack => Flavour.Dry,
                Stat.SpecialDefense => Flavour.Bitter,
                Stat.Speed => Flavour.Sweet,
                _ => Flavour.Neutral
            };
        }
    }
}