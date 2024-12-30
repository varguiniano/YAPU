using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Base data class for all berry items.
    /// </summary>
    public abstract class Berry : Item
    {
        /// <summary>
        /// Call berries can stack.
        /// </summary>
        public override bool CanStack => true;

        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// Flavour data in a similar structure as in gen IV.
        /// </summary>
        [FoldoutGroup("Berry")]
        [ListDrawerSettings(IsReadOnly = true)]
        public SerializableDictionary<Flavour, byte> FlavourData;

        /// <summary>
        /// Growth data, Growth represents the stage and the byte represents the hours that must pass to reach that stage.
        /// </summary>
        [FoldoutGroup("Berry")]
        [SerializeField]
        [ListDrawerSettings(IsReadOnly = true)]
        private SerializableDictionary<Growth, byte> GrowthData;

        /// <summary>
        /// Smoothness of the berry.
        /// </summary>
        [FoldoutGroup("Berry")]
        public byte Smoothness;

        /// <summary>
        /// Size of the berry in cm.
        /// </summary>
        [FoldoutGroup("Berry")]
        public float Size;

        /// <summary>
        /// Firmness of the berry.
        /// </summary>
        [FoldoutGroup("Berry")]
        public Firmness Firmness;

        /// <summary>
        /// Type to be used when using the natural gift move.
        /// </summary>
        [FoldoutGroup("Berry/Natural Gift")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType NaturalGiftType;

        /// <summary>
        /// Power to be used when using the natural gift move.
        /// </summary>
        [FoldoutGroup("Berry/Natural Gift")]
        [SerializeField]
        private byte NaturalGiftPower;

        /// <summary>
        /// Effects for cud chewing the berry in battle.
        /// </summary>
        [FoldoutGroup("Holding")]
        [SerializeField]
        private List<UseOnTargetItemEffect> CudChewEffects = new();

        /// <summary>
        /// Get a localized string with all the berry's stat information.
        /// </summary>
        /// <param name="localizer">Localizer to use.</param>
        /// <returns>A localized string with all the stat information.</returns>
        public string GetLocalizedStatInformation(ILocalizer localizer)
        {
            StringBuilder stats = new();

            bool atLeastOneFlavourAdded = false;

            foreach (KeyValuePair<Flavour, byte> datum in FlavourData)
            {
                if (datum.Value <= 0) continue;

                if (atLeastOneFlavourAdded) stats.Append(", ");

                stats.Append(localizer[datum.Key.ToLocalizableString()]);
                stats.Append(": ");
                stats.Append(datum.Value.ToString());

                atLeastOneFlavourAdded = true;
            }

            if (!atLeastOneFlavourAdded) stats.Append(localizer["Dialogs/Berry/NeutralFlavour"]);

            stats.Append(".\n");
            stats.Append(localizer["Dialogs/Berry/Smoothness"]);
            stats.Append(": ");
            stats.Append(Smoothness);
            stats.Append(", ");
            stats.Append(localizer["Dialogs/Berry/Firmness"]);
            stats.Append(": ");
            stats.Append(localizer[Firmness.ToLocalizableString()]);
            stats.Append(", ");
            stats.Append(localizer["Dialogs/Berry/Size"]);
            stats.Append(": ");
            stats.Append(Size.ToString("##.##"));
            stats.Append(" cm.");

            return stats.ToString();
        }

        /// <summary>
        /// Berry eaten callback.
        /// </summary>
        public override IEnumerator FlingOnTarget(Battler battler,
                                                  BattleManager battleManager,
                                                  YAPUSettings settings,
                                                  ExperienceLookupTable experienceLookupTable,
                                                  ILocalizer localizer)
        {
            yield return base.FlingOnTarget(battler, battleManager, settings, experienceLookupTable, localizer);

            yield return battler.OnBerryEaten(this, battleManager);
        }

        /// <summary>
        /// Berry eaten callback.
        /// </summary>
        public override IEnumerator AfterAction(Battler battler,
                                                BattleAction action,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            bool consume = false;

            yield return base.AfterAction(battler,
                                          action,
                                          battleManager,
                                          localizer,
                                          shouldConsume => consume = shouldConsume);

            if (consume) yield return battler.OnBerryEaten(this, battleManager);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Berry eaten callback.
        /// </summary>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoreAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            bool consume = false;

            yield return base.AfterHitByMove(move,
                                             effectiveness,
                                             battler,
                                             user,
                                             substituteTookHit,
                                             ignoreAbilities,
                                             battleManager,
                                             localizer,
                                             shouldConsume => consume = shouldConsume);

            if (consume) yield return battler.OnBerryEaten(this, battleManager);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Berry eaten callback.
        /// </summary>
        public override IEnumerator AfterHitByMultihitMove(DamageMove move,
                                                           float effectiveness,
                                                           Battler battler,
                                                           Battler user,
                                                           bool substituteTookHit,
                                                           bool ignoreAbilities,
                                                           BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           Action<bool> finished)
        {
            bool consume = false;

            yield return base.AfterHitByMultihitMove(move,
                                                     effectiveness,
                                                     battler,
                                                     user,
                                                     substituteTookHit,
                                                     ignoreAbilities,
                                                     battleManager,
                                                     localizer,
                                                     shouldConsume => consume = shouldConsume);

            if (consume) yield return battler.OnBerryEaten(this, battleManager);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called to cud chew the berry.
        /// </summary>
        /// <param name="owner">Monster cud chewing the berry.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator CudChewBerry(Battler owner, BattleManager battleManager)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (UseOnTargetItemEffect effect in CudChewEffects)
                yield return effect.UseOnBattler(this,
                                                 owner,
                                                 battleManager,
                                                 battleManager.YAPUSettings,
                                                 battleManager.ExperienceLookupTable,
                                                 battleManager.Localizer,
                                                 _ =>
                                                 {
                                                 });
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Called on inspector init.
        /// </summary>
        [OnInspectorInit]
        protected override void InspectorInit()
        {
            base.InspectorInit();
            PopulateFlavour();
            PopulateGrowth();
        }

        /// <summary>
        /// Populate the flavour is it is empty.
        /// </summary>
        private void PopulateFlavour()
        {
            if (FlavourData is {Count: > 0}) return;

            FlavourData = new SerializableDictionary<Flavour, byte>();

            foreach (Flavour flavour in Utils.GetAllItems<Flavour>()) FlavourData[flavour] = 0;
        }

        /// <summary>
        /// Populate the growth is it is empty.
        /// </summary>
        private void PopulateGrowth()
        {
            if (GrowthData is {Count: > 0}) return;

            GrowthData = new SerializableDictionary<Growth, byte>();

            foreach (Growth growth in Utils.GetAllItems<Growth>()) GrowthData[growth] = 0;
        }

        #endif
    }
}