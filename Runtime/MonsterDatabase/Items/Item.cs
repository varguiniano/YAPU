using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetIcons;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterUsingMove;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.Breeding;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.ForceSurviveEffect;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAccuracyWhenTargeted;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAccuracyWhenUsing;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAttackMultiplierOfTypesWhenDefending;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateCriticalChance;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateDrainHPMultiplier;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateEncounterChance;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateMoveDamage;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculatePriceMoney;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateSideStatusEffectDuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateTerrainEffectDuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateVolatileStatusEffectDuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateWeatherEffectDuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateXPYield;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCheckGrounding;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCheckRunAway;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnDeterminePriorityInsideBracket;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnEvolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnHitByMove;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnItemMovedEffect;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnMonsterEnteredBattleEffects;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnStatCalculation;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnTerrainSet;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnVolatileStatusAdded;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnYield;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostTurn;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnMove;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OutOfBattle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using Move = Varguiniano.YAPU.Runtime.MonsterDatabase.Moves.Move;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items
{
    /// <summary>
    /// Database object that represents an item.
    /// </summary>
    public abstract class Item : DocumentableMonsterDatabaseScriptable<Item>, ICommandParameter
    {
        /// <summary>
        /// All item localization keys will start with Items/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Items/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => true;

        /// <summary>
        /// This item´s category.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItemCategories))]
        #endif
        public ItemCategory ItemCategory;

        /// <summary>
        /// Icon sprite for this item.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [PreviewField(100)]
        public Sprite Icon;

        /// <summary>
        /// Icon to show on the editor.
        /// </summary>
        [AssetIcon]
        [UsedImplicitly]
        public Sprite EditorIcon => Icon;

        /// <summary>
        /// Default price this item would have at a shop.
        /// </summary>
        [FoldoutGroup("Economy")]
        public uint DefaultPrice;

        /// <summary>
        /// Can this item be sold by the player to a shop?
        /// </summary>
        [FoldoutGroup("Economy")]
        public bool CanBeSold;

        /// <summary>
        /// Percentage of the price that the player gets paid when sold.
        /// </summary>
        [FoldoutGroup("Economy")]
        [ShowIf(nameof(CanBeSold))]
        [PropertyRange(0, 1)]
        public float SellPercentage = .5f;

        /// <summary>
        /// Price at which it is commonly sold.
        /// </summary>
        [FoldoutGroup("Economy")]
        [ShowIf(nameof(CanBeSold))]
        [ShowInInspector]
        public uint SellPrice => (uint) (DefaultPrice * SellPercentage);

        /// <summary>
        /// Can this item be held by monsters?
        /// </summary>
        [FoldoutGroup("Holding")]
        public bool CanBeHeld;

        /// <summary>
        /// Power when using fling.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        public int FlingPower = 30;

        /// <summary>
        /// Power when using fling.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf("@" + nameof(CanBeHeld) + " && " + nameof(FlingPower) + " > 0")]
        [SerializeField]
        private bool FlingHasAdditionalEffects;

        /// <summary>
        /// Effects for using it in battle.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf("@" + nameof(CanBeHeld) + " && " + nameof(FlingPower) + " > 0 && " + nameof(FlingHasAdditionalEffects))]
        [SerializeField]
        private List<UseOnTargetItemEffect> FlingEffects = new();

        /// <summary>
        /// Does this item make the holder gain shared XP in battle?
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        public bool SharesXP;

        /// <summary>
        /// Effects to call on item moving.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnItemMovedItemEffect> OnItemMovedItemEffects = new();

        /// <summary>
        /// Effects to call on stat calculation.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnStatCalculationItemEffect> OnStatCalculationEffects = new();

        /// <summary>
        /// Effects that modify the critical chance when using a move.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateCriticalChanceItemEffect> OnCalculateCriticalChanceItemEffects = new();

        /// <summary>
        /// Effects to call to modify the attack multiplier of two types.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffect>
            OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffects = new();

        /// <summary>
        /// Callbacks after an action is performed in battle.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnMonsterEnteredBattleEffect> OnMonsterEnteredBattleEffects = new();

        /// <summary>
        /// Callbacks after an action is performed in battle.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<PostActionCallbackItemEffect> AfterActionCallbacks = new();

        /// <summary>
        /// Callbacks after a terrain has been set.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnTerrainSetItemEffect> OnTerrainSetItemEffects = new();

        /// <summary>
        /// Callbacks once after each turn.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<PostTurnCallbackItemEffect> PostTurnCallbackItemEffects = new();

        /// <summary>
        /// Callbacks when calculating move accuracy and the holder is the user.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateAccuracyWhenUsingItemEffect> OnCalculateAccuracyWhenUsingItemEffects = new();

        /// <summary>
        /// Callbacks when calculating move accuracy and targeting the holder.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateAccuracyWhenTargetedItemEffect> OnCalculateAccuracyWhenTargetedItemEffects = new();

        /// <summary>
        /// Callbacks when checking the battler grounding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCheckGroundingItemEffect> OnCheckGroundingItemEffects = new();

        /// <summary>
        /// Callbacks when checking the if the battler can run away.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<CheckRunAwayItemEffect> OnCheckRunAwayItemEffects = new();

        /// <summary>
        /// Callbacks when calculating move damage.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateMoveDamageItemEffect> OnCalculateMoveDamageItemEffects = new();

        /// <summary>
        /// Callbacks when the holder is hit by a move.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<HitByMoveItemEffect> HitByMoveCallbacks = new();

        /// <summary>
        /// Callbacks after the holder is hit by a move.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<AfterHitByMoveItemEffect> AfterHitByMoveCallbacks = new();

        /// <summary>
        /// Callbacks after the holder is hit by a multihit move.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<AfterHitByMoveItemEffect> AfterHitByMultihitMoveCallbacks = new();

        /// <summary>
        /// Callbacks that allow the holder to force survive HP loses.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<ForceSurviveItemEffect> ForceSurviveItemEffects = new();

        /// <summary>
        /// Callbacks after the holder is hit by a move.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<AfterUsingMoveItemEffect> AfterUsingMoveCallbacks = new();

        /// <summary>
        /// Callbacks when determining the priority inside its bracket.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnDeterminePriorityInsideBracketItemEffect> OnDeterminePriorityInsideBracketCallbacks = new();

        /// <summary>
        /// Effects to call when calculating HP to drain.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateDrainHPMultiplierItemEffect> OnCalculateDrainHPMultiplierEffects = new();

        /// <summary>
        /// Effects to call when calculating XP to yield.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateXPYieldItemEffect> OnCalculateXPYieldEffects = new();

        /// <summary>
        /// Effects to call when yielding XP and EV.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnYieldCallbackItemEffect> OnYieldEffects = new();

        /// <summary>
        /// Effects to call when calculating price money.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculatePriceMoneyItemEffect> OnCalculatePriceMoneyItemEffects = new();

        /// <summary>
        /// Effects that modify the duration of a weather effect.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateWeatherEffectDurationItemEffect> OnCalculateWeatherEffectDurationItemEffects = new();

        /// <summary>
        /// Effects that modify the duration of a side status effect.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateSideStatusEffectDurationItemEffect> OnCalculateSideStatusEffectDurationItemEffects =
            new();

        /// <summary>
        /// Effects that modify the duration of a volatile status effect.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateVolatileStatusEffectDurationItemEffect>
            OnCalculateVolatileStatusEffectDurationItemEffects =
                new();

        /// <summary>
        /// Effects that modify the duration of a terrain effect.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateTerrainEffectDurationItemEffect> OnCalculateTerrainEffectDurationItemEffects = new();

        /// <summary>
        /// Effects called when the holder is added a volatile status.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnVolatileStatusAddedItemEffect> OnVolatileStatusAddedItemEffects = new();

        /// <summary>
        /// Effects that modify the wild encounter chances.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateEncounterChanceItemEffect> OnCalculateEncounterChanceItemEffects = new();

        /// <summary>
        /// Effects that can prevent a monster from evolving.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<AllowEvolutionItemEffect> AllowEvolutionItemEffects = new();

        /// <summary>
        /// Effects that modify the number of IVs to pass on breeding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateNumberOfIVsToPassOnBreedingItemEffect>
            OnCalculateNumberOfIVsToPassOnBreedingItemEffects = new();

        /// <summary>
        /// Effects that allow to pass specific IVs on breeding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateSpecificIVsToPassOnBreedingItemEffect>
            OnCalculateSpecificIVsToPassOnBreedingItemEffects = new();

        /// <summary>
        /// Effects that allow to force pass the parent nature when breeding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateNatureToPassOnBreedingItemEffect>
            OnCalculateNatureToPassOnBreedingItemEffects = new();

        /// <summary>
        /// Effects that allow to force pass the parent's form when breeding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnCalculateFormToPassOnBreedingItemEffect>
            OnCalculateFormToPassOnBreedingItemEffects = new();

        /// <summary>
        /// Effects that allow to add extra moves to pass on breeding.
        /// </summary>
        [FoldoutGroup("Holding")]
        [ShowIf(nameof(CanBeHeld))]
        [SerializeField]
        private List<OnAddExtraMovesToPassOnBreedingItemEffect>
            OnAddExtraMovesToPassOnBreedingItemEffects = new();

        /// <summary>
        /// Can this item stack on the bag?
        /// To be implemented by specific item categories or types.
        /// </summary>
        public abstract bool CanStack { get; }

        /// <summary>
        /// Can this item be used without a target?
        /// </summary>
        public abstract bool CanBeUsed { get; }

        /// <summary>
        /// Can this item be registered to be used on quick access?
        /// </summary>
        public abstract bool CanBeRegistered { get; }

        /// <summary>
        /// Can this item be used with a monster instance as a target?
        /// </summary>
        public abstract bool CanBeUsedOnTarget { get; }

        /// <summary>
        /// Can this item be used with a move slot in a monster instance as a target?
        /// </summary>
        public abstract bool CanBeUsedOnTargetMove { get; }

        /// <summary>
        /// Can this item be used in battle without a target?
        /// </summary>
        public abstract bool CanBeUsedInBattle { get; }

        /// <summary>
        /// Can this item be used with a battler as a target?
        /// </summary>
        public abstract bool CanBeUsedInBattleOnTarget { get; }

        /// <summary>
        /// Can this item be used with a move slot in a battler as a target?
        /// </summary>
        public abstract bool CanBeUsedInBattleOnTargetMove { get; }

        /// <summary>
        /// Does this item need a monster panel animation when used in the bag screen?
        /// </summary>
        [FoldoutGroup("Animation")]
        [ShowIf("@("
              + nameof(CanBeUsed)
              + "||"
              + nameof(CanBeUsedOnTarget)
              + ") && !"
              + nameof(RequiresUpdatingTheEntirePartyInBagScreen))]
        public bool NeedsPanelAnimationInBagScreen;

        /// <summary>
        /// Does this item need updating the entire party in the bag screen?
        /// </summary>
        [FoldoutGroup("Animation")]
        [ShowIf("@" + nameof(CanBeUsed) + "||" + nameof(CanBeUsedOnTarget))]
        public bool RequiresUpdatingTheEntirePartyInBagScreen;

        /// <summary>
        /// Effects for using it on a target.
        /// </summary>
        [ShowIf("@CanBeUsedOnTarget || CanBeUsedInBattleOnTarget")]
        [SerializeField]
        internal List<UseOnTargetItemEffect> UseOnTargetEffects = new();

        /// <summary>
        /// Effects for using it on a target move slot.
        /// </summary>
        [ShowIf("@CanBeUsedOnTargetMove || CanBeUsedInBattleOnTargetMove")]
        [SerializeField]
        private List<UseOnTargetMoveItemEffect> UseOnTargetMoveEffects = new();

        /// <summary>
        /// Effects for using it out of battle.
        /// </summary>
        [ShowIf(nameof(CanBeUsed))]
        [SerializeField]
        private List<UseOutOfBattleItemEffect> UseOutOfBattleEffects = new();

        /// <summary>
        /// Effects for using it in battle.
        /// </summary>
        [ShowIf(nameof(CanBeUsedInBattle))]
        [SerializeField]
        private List<UseInBattleItemEffect> UseInBattleEffects = new();

        /// <summary>
        /// Get the name of this item.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>Its localized name.</returns>
        public virtual string GetName(ILocalizer localizer) => localizer[LocalizableName];

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => GetName(localizer);

        /// <summary>
        /// Get the description of this item.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <returns>Its localized description.</returns>
        public virtual string GetDescription(ILocalizer localizer, PlayerSettings playerSettings) =>
            localizer[LocalizableDescription];

        /// <summary>
        /// Show a notification with this ability's name and the icon of its owner.
        /// </summary>
        /// <param name="user">User of the item</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public void ShowItemNotification(MonsterInstance user, ILocalizer localizer) =>
            DialogManager.Notifications.QueueIconTextNotification(user.GetIcon(), GetName(localizer));

        /// <summary>
        /// Check if the item can be used on a monster.
        /// If a single effect is compatible, the item is compatible.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsCompatible(YAPUSettings settings,
                                 TimeManager timeManager,
                                 PlayerCharacter playerCharacter,
                                 MonsterInstance monsterInstance)
        {
            if (!CanBeUsedOnTarget) return false;

            bool compatible = false;

            foreach (UseOnTargetItemEffect _ in
                     UseOnTargetEffects.Where(effect => effect.IsCompatible(settings,
                                                                            timeManager,
                                                                            monsterInstance,
                                                                            this,
                                                                            playerCharacter)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager">Reference to the evolution manager.</param>
        /// <param name="inputManager">Reference to the input manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseOnTarget(MonsterInstance monsterInstance,
                                       YAPUSettings settings,
                                       ExperienceLookupTable experienceLookupTable,
                                       PlayerCharacter playerCharacter,
                                       TimeManager timeManager,
                                       EvolutionManager evolutionManager,
                                       IInputManager inputManager,
                                       ILocalizer localizer,
                                       Action<bool> finished)
        {
            bool consume = false;

            foreach (UseOnTargetItemEffect effect in UseOnTargetEffects)
                yield return effect.UseOnMonsterInstance(monsterInstance,
                                                         this,
                                                         settings,
                                                         experienceLookupTable,
                                                         playerCharacter,
                                                         timeManager,
                                                         evolutionManager,
                                                         inputManager,
                                                         localizer,
                                                         shouldConsume =>
                                                         {
                                                             if (shouldConsume) consume = true;
                                                         });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Check if the item can be used on a monster in battle.
        /// If a single effect is compatible, the item is compatible.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsCompatible(BattleManager battleManager, Battler battler)
        {
            if (!CanBeUsedInBattleOnTarget) return false;

            bool compatible = false;

            foreach (UseOnTargetItemEffect _ in
                     UseOnTargetEffects.Where(effect => effect.IsCompatible(battleManager, battler, this)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Use on a battler.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseOnTarget(Battler battler,
                                       BattleManager battleManager,
                                       YAPUSettings settings,
                                       ExperienceLookupTable experienceLookupTable,
                                       ILocalizer localizer,
                                       Action<bool> finished)
        {
            bool consume = false;

            foreach (UseOnTargetItemEffect effect in UseOnTargetEffects)
                yield return effect.UseOnBattler(this,
                                                 battler,
                                                 battleManager,
                                                 settings,
                                                 experienceLookupTable,
                                                 localizer,
                                                 shouldConsume =>
                                                 {
                                                     if (shouldConsume) consume = true;
                                                 });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Called when the item is flung on a target.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        public virtual IEnumerator FlingOnTarget(Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer)
        {
            if (!FlingHasAdditionalEffects) yield break;

            foreach (UseOnTargetItemEffect effect in FlingEffects)
                yield return effect.UseOnBattler(this,
                                                 battler,
                                                 battleManager,
                                                 settings,
                                                 experienceLookupTable,
                                                 localizer,
                                                 _ =>
                                                 {
                                                 },
                                                 true);
        }

        /// <summary>
        /// Use out of battle.
        /// </summary>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseOutOfBattle(PlayerSettings playerSettings,
                                          PlayerCharacter playerCharacter,
                                          ILocalizer localizer,
                                          Action<bool> finished)
        {
            bool consume = false;

            foreach (UseOutOfBattleItemEffect effect in UseOutOfBattleEffects)
                yield return effect.Use(this,
                                        playerSettings,
                                        playerCharacter,
                                        localizer,
                                        shouldConsume =>
                                        {
                                            if (shouldConsume) consume = true;
                                        });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Check if the item can be used in this moment.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user">User of the item.</param>
        /// <returns>True if it can be used in this moment.</returns>
        public bool CanBeUsedInBattleRightNow(BattleManager battleManager, Battler user) =>
            UseInBattleEffects.Any(effect => effect.CanBeUsed(battleManager, user));

        /// <summary>
        /// Use in battle.
        /// </summary>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseInBattle(BattlerType userType,
                                       int userIndex,
                                       BattleManager battleManager,
                                       ILocalizer localizer,
                                       Action<bool> finished)
        {
            bool consume = false;

            foreach (UseInBattleItemEffect effect in UseInBattleEffects)
                yield return effect.Use(this,
                                        userType,
                                        userIndex,
                                        battleManager,
                                        shouldConsume =>
                                        {
                                            if (shouldConsume) consume = true;
                                        });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Check if the item can be used on any move slot of a monster.
        /// If a single effect is compatible on a single or multiple move slots, the item is compatible.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsMonsterCompatibleForMoveTargeting(MonsterInstance monsterInstance)
        {
            if (!CanBeUsedOnTargetMove) return false;

            bool compatible = false;

            foreach (UseOnTargetMoveItemEffect _ in
                     UseOnTargetMoveEffects.Where(effect => effect.IsCompatible(monsterInstance)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Check if the item can be used on any move slot of a monster.
        /// If a single effect is compatible on a single or multiple move slots, the item is compatible.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="index">Index of the move slot.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsMoveCompatible(MonsterInstance monsterInstance, int index)
        {
            if (!CanBeUsedOnTargetMove) return false;

            bool compatible = false;

            foreach (UseOnTargetMoveItemEffect _ in
                     UseOnTargetMoveEffects.Where(effect => effect.IsMoveCompatible(monsterInstance, index)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="index">Move slot index.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseOnTargetMove(MonsterInstance monsterInstance,
                                           int index,
                                           PlayerCharacter playerCharacter,
                                           ILocalizer localizer,
                                           Action<bool> finished)
        {
            bool consume = false;

            foreach (UseOnTargetMoveItemEffect effect in UseOnTargetMoveEffects)
                yield return effect.UseOnMonsterInstance(monsterInstance,
                                                         index,
                                                         playerCharacter,
                                                         localizer,
                                                         shouldConsume =>
                                                         {
                                                             if (shouldConsume) consume = true;
                                                         });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Check if the item can be used on any move slot of a monster.
        /// If a single effect is compatible on a single or multiple move slots, the item is compatible.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsMonsterCompatibleForMoveTargeting(BattleManager battleManager, Battler battler)
        {
            if (!CanBeUsedInBattleOnTargetMove) return false;

            bool compatible = false;

            foreach (UseOnTargetMoveItemEffect _ in
                     UseOnTargetMoveEffects.Where(effect => effect.IsCompatible(battleManager, battler)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Check if the item can be used on any move slot of a monster.
        /// If a single effect is compatible on a single or multiple move slots, the item is compatible.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="index">Index of the move slot.</param>
        /// <returns>True if it can be used.</returns>
        public bool IsMoveCompatible(BattleManager battleManager, Battler battler, int index)
        {
            if (!CanBeUsedInBattleOnTargetMove) return false;

            bool compatible = false;

            foreach (UseOnTargetMoveItemEffect _ in
                     UseOnTargetMoveEffects.Where(effect => effect.IsMoveCompatible(battleManager, battler, index)))
                compatible = true;

            return compatible;
        }

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to target.</param>
        /// <param name="index">Index of the move slot.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator UseOnTargetMove(BattleManager battleManager,
                                           Battler battler,
                                           int index,
                                           ILocalizer localizer,
                                           Action<bool> finished)
        {
            bool consume = false;

            foreach (UseOnTargetMoveItemEffect effect in UseOnTargetMoveEffects)
                yield return effect.UseOnBattler(battleManager,
                                                 battler,
                                                 index,
                                                 localizer,
                                                 shouldConsume =>
                                                 {
                                                     if (shouldConsume) consume = true;
                                                 });

            finished?.Invoke(consume);
        }

        /// <summary>
        /// Called when the item effect is negated.
        /// </summary>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnEffectNegated(Battler battler, BattleManager battleManager) =>
            OnItemMovedItemEffects.Select(effect => effect.OnEffectNegated(this, battler, battleManager))
                                   // ReSharper disable once NotDisposedResourceIsReturned
                                  .GetEnumerator();

        /// <summary>
        /// Called when the item effect is negated.
        /// </summary>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnEffectReEnabled(Battler battler, BattleManager battleManager) =>
            OnItemMovedItemEffects.Select(effect => effect.OnEffectReEnabled(this, battler, battleManager))
                                   // ReSharper disable once NotDisposedResourceIsReturned
                                  .GetEnumerator();

        /// <summary>
        /// Called when the item gets stolen.
        /// </summary>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemStolen(Battler battler, BattleManager battleManager) =>
            // ReSharper disable once NotDisposedResourceIsReturned
            OnItemMovedItemEffects.Select(effect => effect.OnItemStolen(this, battler, battleManager)).GetEnumerator();

        /// <summary>
        /// Called when the holder received the item in battle.
        /// </summary>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemReceivedInBattle(Battler battler, BattleManager battleManager) =>
            OnItemMovedItemEffects.Select(effect => effect.OnItemReceivedInBattle(this, battler, battleManager))
                                   // ReSharper disable once NotDisposedResourceIsReturned
                                  .GetEnumerator();

        /// <summary>
        /// Called when the item is consumed in battle.
        /// </summary>
        /// <param name="battler">Reference to the owner.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public virtual IEnumerator OnItemConsumedInBattle(Battler battler, BattleManager battleManager) =>
            OnItemMovedItemEffects.Select(effect => effect.OnItemConsumedInBattle(this, battler, battleManager))
                                   // ReSharper disable once NotDisposedResourceIsReturned
                                  .GetEnumerator();

        /// <summary>
        /// Called when the item is consumed.
        /// </summary>
        public virtual IEnumerator OnItemConsumed() =>
            // ReSharper disable once NotDisposedResourceIsReturned
            OnItemMovedItemEffects.Select(effect => effect.OnItemConsumed(this)).GetEnumerator();

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if the item should be consumed.</param>
        public IEnumerator OnMonsterEnteredBattle(Battler battler,
                                                  BattleManager battleManager,
                                                  Action<bool> finished)
        {
            bool consume = false;

            foreach (OnMonsterEnteredBattleEffect effect in OnMonsterEnteredBattleEffects)
                yield return effect.OnMonsterEnteredBattle(this,
                                                           battler,
                                                           battleManager,
                                                           shouldConsume => consume = shouldConsume);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public virtual IEnumerator AfterAction(Battler battler,
                                               BattleAction action,
                                               BattleManager battleManager,
                                               ILocalizer localizer,
                                               Action<bool> finished)
        {
            bool consume = false;

            foreach (PostActionCallbackItemEffect callback in AfterActionCallbacks)
                yield return callback.AfterAction(this,
                                                  battler,
                                                  action,
                                                  battleManager,
                                                  localizer,
                                                  shouldConsume =>
                                                  {
                                                      if (shouldConsume) consume = true;
                                                  });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called when a terrain is set on the battlefield.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="terrain">Terrain that has been set.</param>
        /// <param name="holder">Holder of this item.</param>
        /// <param name="finished">Should the item be consumed?</param>
        public IEnumerator OnTerrainSet(BattleManager battleManager,
                                        Terrain terrain,
                                        Battler holder,
                                        Action<bool> finished)
        {
            bool consume = false;

            foreach (OnTerrainSetItemEffect effect in OnTerrainSetItemEffects)
                yield return effect.OnTerrainSet(battleManager,
                                                 terrain,
                                                 holder,
                                                 this,
                                                 shouldConsume =>
                                                 {
                                                     if (shouldConsume) consume = true;
                                                 });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator AfterTurnPreStatus(Battler battler,
                                              BattleManager battleManager,
                                              ILocalizer localizer,
                                              Action<bool> finished)
        {
            bool consume = false;

            foreach (PostTurnCallbackItemEffect callback in PostTurnCallbackItemEffects)
                yield return callback.AfterTurnPreStatus(this,
                                                         battler,
                                                         battleManager,
                                                         localizer,
                                                         shouldConsume =>
                                                         {
                                                             if (shouldConsume) consume = true;
                                                         });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator AfterTurnPostStatus(Battler battler,
                                               BattleManager battleManager,
                                               ILocalizer localizer,
                                               Action<bool> finished)
        {
            bool consume = false;

            foreach (PostTurnCallbackItemEffect callback in PostTurnCallbackItemEffects)
                yield return callback.AfterTurnPostStatus(this,
                                                          battler,
                                                          battleManager,
                                                          localizer,
                                                          shouldConsume =>
                                                          {
                                                              if (shouldConsume) consume = true;
                                                          });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called when the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed and the new effectiveness.</param>
        public IEnumerator OnHitByMove(DamageMove move,
                                       float effectiveness,
                                       Battler battler,
                                       BattleManager battleManager,
                                       Battler moveUser,
                                       ILocalizer localizer,
                                       Action<bool, float> finished)
        {
            bool consume = false;

            foreach (HitByMoveItemEffect callback in HitByMoveCallbacks)
                yield return callback.OnHitByMove(this,
                                                  move,
                                                  effectiveness,
                                                  battler,
                                                  battleManager,
                                                  moveUser,
                                                  localizer,
                                                  (shouldConsume, newEffectiveness) =>
                                                  {
                                                      if (shouldConsume) consume = true;

                                                      effectiveness = newEffectiveness;
                                                  });

            finished.Invoke(consume, effectiveness);
        }

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoreAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public virtual IEnumerator AfterHitByMove(DamageMove move,
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

            foreach (AfterHitByMoveItemEffect callback in AfterHitByMoveCallbacks)
                yield return callback.AfterHitByMove(this,
                                                     move,
                                                     effectiveness,
                                                     battler,
                                                     user,
                                                     substituteTookHit,
                                                     ignoreAbilities,
                                                     battleManager,
                                                     localizer,
                                                     shouldConsume =>
                                                     {
                                                         if (shouldConsume) consume = true;
                                                     });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called after the holder is hit by a multihit move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoreAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public virtual IEnumerator AfterHitByMultihitMove(DamageMove move,
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

            foreach (AfterHitByMoveItemEffect callback in AfterHitByMultihitMoveCallbacks)
                yield return callback.AfterHitByMove(this,
                                                     move,
                                                     effectiveness,
                                                     battler,
                                                     user,
                                                     substituteTookHit,
                                                     ignoreAbilities,
                                                     battleManager,
                                                     localizer,
                                                     shouldConsume =>
                                                     {
                                                         if (shouldConsume) consume = true;
                                                     });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Check if this item should trigger force survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">Amount the HP is going to change. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        /// <returns>True if force survive should be triggered.</returns>
        public bool ShouldForceSurvive(Battler owner,
                                       int amount,
                                       BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       bool isSecondaryDamage,
                                       Move userMove = null) =>
            ForceSurviveItemEffects.Aggregate(false,
                                              (current, effect) => current
                                                                || effect.ShouldForceSurvive(this,
                                                                       owner,
                                                                       amount,
                                                                       battleManager,
                                                                       userType,
                                                                       userIndex,
                                                                       isSecondaryDamage,
                                                                       userMove));

        /// <summary>
        /// Called after the monster has survived, if it was this item the one that made it survive.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="amount">The actual amount of Hp that was changed. Negative if losing.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect that triggered the HP loss.</param>
        /// <param name="userIndex">User of the effect that triggered the HP loss.</param>
        /// <param name="isSecondaryDamage">Is it secondary damage?</param>
        /// <param name="finished">Callback stating true if the item should be consumed.</param>
        /// <param name="userMove">Move that is making the damage, if any.</param>
        public IEnumerator OnForceSurvive(Battler owner,
                                          int amount,
                                          BattleManager battleManager,
                                          BattlerType userType,
                                          int userIndex,
                                          bool isSecondaryDamage,
                                          Action<bool> finished,
                                          Move userMove = null)
        {
            bool shouldConsume = false;

            foreach (ForceSurviveItemEffect effect in ForceSurviveItemEffects)
                yield return effect.OnForceSurvive(this,
                                                   owner,
                                                   amount,
                                                   battleManager,
                                                   userType,
                                                   userIndex,
                                                   isSecondaryDamage,
                                                   consumeItem =>
                                                   {
                                                       if (consumeItem) shouldConsume = true;
                                                   },
                                                   userMove);

            finished.Invoke(shouldConsume);
        }

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public IEnumerator AfterHittingWithMove(Move move,
                                                Battler user,
                                                List<(BattlerType Type, int Index)> targets,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            bool consume = false;

            foreach (AfterUsingMoveItemEffect callback in AfterUsingMoveCallbacks)
                yield return callback.AfterHittingWithMove(this,
                                                           move,
                                                           user,
                                                           targets,
                                                           battleManager,
                                                           localizer,
                                                           shouldConsume =>
                                                           {
                                                               if (shouldConsume) consume = true;
                                                           });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public IEnumerator AfterUsingMove(Move move,
                                          Battler user,
                                          List<(BattlerType Type, int Index)> targets,
                                          BattleManager battleManager,
                                          ILocalizer localizer,
                                          Action<bool> finished)
        {
            bool consume = false;

            foreach (AfterUsingMoveItemEffect callback in AfterUsingMoveCallbacks)
                yield return callback.AfterUsingMove(this,
                                                     move,
                                                     user,
                                                     targets,
                                                     battleManager,
                                                     localizer,
                                                     shouldConsume =>
                                                     {
                                                         if (shouldConsume) consume = true;
                                                     });

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called to determine the priority of the battler inside its priority bracket.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed and true if it should go first (1), last (-1) or normal (0).</param>
        public IEnumerator OnDeterminePriority(Battler battler,
                                               BattleManager battleManager,
                                               ILocalizer localizer,
                                               Action<bool, int> finished)
        {
            bool consume = false;
            int priorityChange = 0;

            foreach (OnDeterminePriorityInsideBracketItemEffect callback in OnDeterminePriorityInsideBracketCallbacks)
                yield return callback.OnDeterminePriority(this,
                                                          battler,
                                                          battleManager,
                                                          localizer,
                                                          (shouldConsume, newPriorityChange) =>
                                                          {
                                                              if (shouldConsume) consume = true;
                                                              priorityChange += newPriorityChange;
                                                          });

            finished.Invoke(consume, Mathf.Clamp(priorityChange, -1, 1));
        }

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>If it can run away, and if it overrides all other effects preventing run away.</returns>
        public (bool, bool) CanRunAway(Battler battler, BattleManager battleManager, bool showMessages)
        {
            bool canRun = true;
            bool overrideAll = false;

            foreach (CheckRunAwayItemEffect effect in OnCheckRunAwayItemEffects)
            {
                (bool effectAllowsRunning, bool effectOverrides) =
                    effect.CheckCanRunAway(this, battler, battleManager, showMessages);

                if (!effectAllowsRunning) canRun = false;
                if (effectOverrides) overrideAll = true;
            }

            return (canRun, overrideAll);
        }

        /// <summary>
        /// Called when the battler runs away.
        /// </summary>
        public IEnumerator OnRunAway(Battler owner, BattleManager battleManager) =>
            // ReSharper disable once NotDisposedResourceIsReturned
            OnCheckRunAwayItemEffects.Select(effect => effect.OnRunAway(owner, this, battleManager)).GetEnumerator();

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="modifier">Critical stage modifier to use.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <param name="shouldConsume">Should the item be consumed upon use?</param>
        /// <returns>Has the chance been changed?</returns>
        public bool OnCalculateCriticalChance(Battler owner,
                                              Battler target,
                                              BattleManager battleManager,
                                              Move move,
                                              ref float multiplier,
                                              ref byte modifier,
                                              ref bool alwaysHit,
                                              out bool shouldConsume)
        {
            bool modified = false;
            shouldConsume = false;

            foreach (OnCalculateCriticalChanceItemEffect effect in OnCalculateCriticalChanceItemEffects)
            {
                modified |= effect.OnCalculateCriticalChance(owner,
                                                             this,
                                                             target,
                                                             battleManager,
                                                             move,
                                                             ref multiplier,
                                                             ref modifier,
                                                             ref alwaysHit,
                                                             out bool consume);

                shouldConsume |= consume;
            }

            return modified;
        }

        /// <summary>
        /// Called when calculating the move's accuracy and the holder is the user.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public float OnCalculateAccuracyWhenUsed(Move move,
                                                 Battler user,
                                                 Battler target,
                                                 BattleManager battleManager) =>
            OnCalculateAccuracyWhenUsingItemEffects.Aggregate(1f,
                                                              (current, effect) =>
                                                                  current
                                                                * effect.GetMoveAccuracyMultiplierWhenUsed(this,
                                                                      battleManager,
                                                                      target,
                                                                      user,
                                                                      move));

        /// <summary>
        /// Called when calculating the move's accuracy and targeting the holder.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public float OnCalculateAccuracyWhenTargeted(Move move,
                                                     Battler user,
                                                     Battler target,
                                                     BattleManager battleManager) =>
            OnCalculateAccuracyWhenTargetedItemEffects.Aggregate(1f,
                                                                 (current, effect) =>
                                                                     current
                                                                   * effect.OnCalculateMoveAccuracyWhenTargeted(move,
                                                                         this,
                                                                         user,
                                                                         target,
                                                                         battleManager));

        /// <summary>
        /// Check if this item prevents or forces grounding.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="preventsGrounding">Does this item prevent grounding?</param>
        /// <param name="forcesGrounding">Does this item force grounding?</param>
        public void OnCheckGrounding(BattleManager battleManager,
                                     Battler holder,
                                     out bool preventsGrounding,
                                     out bool forcesGrounding)
        {
            preventsGrounding = false;
            forcesGrounding = false;

            foreach (OnCheckGroundingItemEffect effect in OnCheckGroundingItemEffects)
            {
                if (effect.PreventsGrounding) preventsGrounding = true;
                if (effect.ForcesGrounding) forcesGrounding = true;
            }
        }

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier"></param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed and the new multiplier.</param>
        public IEnumerator OnCalculateMoveDamageWhenUsing(DamageMove move,
                                                          float multiplier,
                                                          Battler user,
                                                          Battler target,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Action<bool, float> finished)
        {
            bool consume = false;

            foreach (OnCalculateMoveDamageItemEffect callback in OnCalculateMoveDamageItemEffects)
                yield return callback.OnCalculateMoveDamage(move,
                                                            this,
                                                            multiplier,
                                                            user,
                                                            target,
                                                            battleManager,
                                                            localizer,
                                                            (shouldConsume, newMultiplier) =>
                                                            {
                                                                if (shouldConsume) consume = true;
                                                                multiplier = newMultiplier;
                                                            });

            finished.Invoke(consume, multiplier);
        }

        /// <summary>
        /// Calculate the multiplier to use when draining HP.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">Battler draining.</param>
        /// <param name="target">Target to drain from, can be null.</param>
        /// <returns>The multiplier to apply.</returns>
        public float CalculateDrainHPMultiplier(BattleManager battleManager, Battler user, Battler target) =>
            OnCalculateDrainHPMultiplierEffects.Aggregate<OnCalculateDrainHPMultiplierItemEffect, float>(1,
                (current, effect) =>
                    current * effect.CalculateDrainHPMultiplier(battleManager, this, user, target));

        /// <summary>
        /// Calculate the object modifier to apply to the XP yield.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="enemyType">The type of enemy it was.</param>
        /// <param name="faintedBattler">Reference to the fainted battler.</param>
        /// <param name="receiver">Reference to the receiver battler.</param>
        public float CalculateXPYieldModifier(YAPUSettings settings,
                                              PlayerSettings playerSettings,
                                              EnemyType enemyType,
                                              Battler faintedBattler,
                                              Battler receiver) =>
            OnCalculateXPYieldEffects.Aggregate<OnCalculateXPYieldItemEffect, float>(1,
                (current, callback) =>
                    current
                  * callback.CalculateXPYieldModifier(settings,
                                                      playerSettings,
                                                      enemyType,
                                                      faintedBattler,
                                                      receiver));

        /// <summary>
        /// Called when yielding xp and ev.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public IEnumerator OnYield(Battler battler,
                                   YAPUSettings settings,
                                   BattleManager battleManager,
                                   ILocalizer localizer,
                                   Action<bool> finished)
        {
            bool consume = false;

            foreach (OnYieldCallbackItemEffect callback in OnYieldEffects)
                yield return callback.OnYield(this,
                                              battler,
                                              settings,
                                              battleManager,
                                              localizer,
                                              shouldConsume => consume = shouldConsume);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        public float OnCalculateStat(MonsterInstance monster,
                                     Stat stat) =>
            OnStatCalculationEffects.Aggregate<OnStatCalculationItemEffect, float>(1,
                (current, callback) => current
                                     * callback.OnCalculateStat(this,
                                                                monster,
                                                                stat));

        /// <summary>
        /// Called when calculating the attack multiplier of a type attacking this monster.
        /// </summary>
        /// <param name="monster">Monster being attacked.</param>
        /// <param name="attackerType">Type attacking.</param>
        /// <param name="userType">Type of the defender to calculate the multiplier from.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier, -1 if unchanged.</returns>
        public float OnCalculateAttackMultiplierOfTypesWhenDefending(Battler monster,
                                                                     MonsterType attackerType,
                                                                     MonsterType userType,
                                                                     BattleManager battleManager)
        {
            foreach (float multiplierOverride in OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffects
                                                .Select(effect =>
                                                            effect
                                                               .CalculateAttackMultiplierOfTypesWhenDefending(this,
                                                                    monster,
                                                                    attackerType,
                                                                    userType,
                                                                    battleManager))
                                                .Where(multiplierOverride => multiplierOverride >= 0))
                return multiplierOverride;

            return -1;
        }

        /// <summary>
        /// Called when calculating price money.
        /// </summary>
        public float GetMultiplierForPriceMoney() =>
            OnCalculatePriceMoneyItemEffects.Aggregate<OnCalculatePriceMoneyItemEffect, float>(1,
                (current, callback) => current * callback.GetMultiplierForPriceMoney());

        /// <summary>
        /// Calculate the duration of a new weather effect created by the holder.
        /// </summary>
        /// <param name="weather">Weather to be created.</param>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateWeatherDuration(Weather weather, Battler owner, BattleManager battleManager)
        {
            // The first effect will be the last to be used.
            foreach (OnCalculateWeatherEffectDurationItemEffect effect in OnCalculateWeatherEffectDurationItemEffects)
                return effect.CalculateWeatherDuration(this, owner, weather, battleManager);

            return -2;
        }

        /// <summary>
        /// Calculate the duration of a new side status effect created by the holder.
        /// </summary>
        /// <param name="statusToAdd">Status to add.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="inBattleIndex">In battle index of the affected roster. Only used for dialogs.</param>
        /// <param name="holder">Holder of the item and the one setting the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateSideStatusDuration(SideStatus statusToAdd,
                                               BattlerType side,
                                               int inBattleIndex,
                                               Battler holder,
                                               BattleManager battleManager)
        {
            // The first effect will be the last to be used.
            foreach (OnCalculateSideStatusEffectDurationItemEffect effect in
                     OnCalculateSideStatusEffectDurationItemEffects)
                return effect.CalculateSideStatusDuration(statusToAdd,
                                                          side,
                                                          inBattleIndex,
                                                          holder,
                                                          this,
                                                          battleManager);

            return -2;
        }

        /// <summary>
        /// Calculate the random countdown of a volatile status to inflict on a target.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="status">Status to inflict.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateRandomCountdownOfVolatileStatus(BattleManager battleManager,
                                                            VolatileStatus status,
                                                            Battler holder,
                                                            BattlerType targetType,
                                                            int targetIndex)
        {
            // The first effect will be the last to be used.
            foreach (OnCalculateVolatileStatusEffectDurationItemEffect effect in
                     OnCalculateVolatileStatusEffectDurationItemEffects)
                return effect.CalculateRandomCountdownOfVolatileStatus(battleManager,
                                                                       status,
                                                                       holder,
                                                                       this,
                                                                       targetType,
                                                                       targetIndex);

            return -2;
        }

        /// <summary>
        /// Calculate the duration of a new Terrain effect created by the holder.
        /// </summary>
        /// <param name="user">Battler setting the terrain.</param>
        /// <param name="terrain">terrain.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateTerrainDuration(Battler user, Terrain terrain, BattleManager battleManager)
        {
            // The first effect will be the last to be used.
            foreach (OnCalculateTerrainEffectDurationItemEffect effect in OnCalculateTerrainEffectDurationItemEffects)
                return effect.CalculateTerrainDuration(this, user, terrain, battleManager);

            return -2;
        }

        /// <summary>
        /// Called when a volatile status is added to the holder.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="userType">Type of the one that added the status.</param>
        /// <param name="userIndex">Index of the one that added the status.</param>
        /// <param name="status">Status added.</param>
        /// <param name="countdown">Countdown established.</param>
        /// <param name="finished">Callback establishing if the item should be consumed.</param>
        public IEnumerator OnVolatileStatusAdded(BattleManager battleManager,
                                                 Battler holder,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 VolatileStatus status,
                                                 int countdown,
                                                 Action<bool> finished)
        {
            bool consume = false;

            foreach (OnVolatileStatusAddedItemEffect effect in OnVolatileStatusAddedItemEffects)
                yield return effect.OnVolatileStatusAdded(battleManager,
                                                          this,
                                                          holder,
                                                          userType,
                                                          userIndex,
                                                          status,
                                                          countdown,
                                                          shouldConsume => consume = shouldConsume);

            finished.Invoke(consume);
        }

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public float OnCalculateEncounterChance(EncounterType encounterType) =>
            OnCalculateEncounterChanceItemEffects.Aggregate<OnCalculateEncounterChanceItemEffect, float>(1,
                (current, effect) => current * effect.OnCalculateEncounterChance(encounterType));

        /// <summary>
        /// Allow the monster to evolve?
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="evolutionData">Evolution data to use.</param>
        /// <returns>True if it allows evolution.</returns>
        public bool AllowEvolution(MonsterInstance holder, EvolutionData evolutionData) =>
            AllowEvolutionItemEffects.All(effect => effect.AllowEvolution(this, holder, evolutionData));

        /// <summary>
        /// Calculate the number of IVs to pass on breeding.
        /// Only the first in the list is taken into account.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The number to pass. -1 if unchanged.</returns>
        public int OnCalculateNumberOfIVsToPassOnBreeding(MonsterInstance holder,
                                                          MonsterInstance otherParent,
                                                          MonsterEntry species,
                                                          Form form,
                                                          bool isMother)
        {
            foreach (int result in OnCalculateNumberOfIVsToPassOnBreedingItemEffects
                                  .Select(effect => effect.OnCalculateNumberOfIVsToPassOnBreeding(holder,
                                              otherParent,
                                              species,
                                              form,
                                              isMother))
                                  .Where(result => result != -1))
                return result;

            return -1;
        }

        /// <summary>
        /// Calculate the specific IVs to pass on breeding.
        /// Only the first in the list is taken into account.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The specific IVs that should be passed.</returns>
        public SerializableDictionary<Stat, byte> OnCalculateSpecificIVsToPassOnBreeding(MonsterInstance holder,
            MonsterInstance otherParent,
            MonsterEntry species,
            Form form,
            bool isMother)
        {
            foreach (OnCalculateSpecificIVsToPassOnBreedingItemEffect effect in
                     OnCalculateSpecificIVsToPassOnBreedingItemEffects)
                return effect.OnCalculateSpecificIVsToPassOnBreeding(holder, otherParent, species, form, isMother);

            return new SerializableDictionary<Stat, byte>();
        }

        /// <summary>
        /// Calculate the nature to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's nature?</returns>
        public bool OnCalculateNatureToPassOnBreeding(MonsterInstance holder,
                                                      MonsterInstance otherParent,
                                                      MonsterEntry species,
                                                      Form form,
                                                      bool isMother) =>
            OnCalculateNatureToPassOnBreedingItemEffects.Any(effect =>
                                                                 effect.OnCalculateNatureToPassOnBreeding(holder,
                                                                     otherParent,
                                                                     species,
                                                                     form,
                                                                     isMother));

        /// <summary>
        /// Calculate the nature to pass on breeding.
        /// </summary>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's form if it is of the same species?</returns>
        public bool OnCalculateFormToPassOnBreeding(MonsterInstance holder,
                                                    MonsterInstance otherParent,
                                                    bool isMother) =>
            OnCalculateFormToPassOnBreedingItemEffects.Any(effect =>
                                                               effect.OnCalculateFormToPassOnBreeding(holder,
                                                                   otherParent,
                                                                   isMother));

        /// <summary>
        /// Callback that allows the monster to add extra moves to the baby mon when breeding.
        /// this is useful for effects like Light Ball.
        /// </summary>
        /// <param name="owner">Owner of the item.</param>
        /// <param name="isMother">Is this mon the mother?</param>
        /// <param name="otherParent">Reference to the other parent.</param>
        /// <param name="babySpecies">Species of the baby.</param>
        /// <param name="babyForm">Form of the baby.</param>
        /// <returns>A list of the moves to include.</returns>
        public IEnumerable<Move> AddExtraLearntMovesWhenBreeding(MonsterInstance owner,
                                                                 bool isMother,
                                                                 MonsterInstance otherParent,
                                                                 MonsterEntry babySpecies,
                                                                 Form babyForm)
        {
            List<Move> moves = new();

            foreach (Move move in OnAddExtraMovesToPassOnBreedingItemEffects.SelectMany(effect => effect
                        .AddExtraLearntMovesWhenBreeding(owner,
                                                         this,
                                                         isMother,
                                                         otherParent,
                                                         babySpecies,
                                                         babyForm)
                        .Where(move => !moves.Contains(move))))
                moves.Add(move);

            return moves;
        }
    }
}