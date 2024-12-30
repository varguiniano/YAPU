using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Data class that represents a monster battler inside the battle.
    /// </summary>
    [Serializable]
    public class Battler : MonsterInstance
    {
        /// <summary>
        /// In battle modifier to the stats.
        /// </summary>
        [ReadOnly]
        public SerializableDictionary<Stat, short> StatStage;

        /// <summary>
        /// In battle modifier to the battle specific stats.
        /// </summary>
        [ReadOnly]
        public SerializableDictionary<BattleStat, short> BattleStatStage;

        /// <summary>
        /// Stage for landing a critical hit.
        /// </summary>
        [ReadOnly]
        public byte CriticalStage;

        /// <summary>
        /// Data representing the substitute for this battler.
        /// </summary>
        [ReadOnly]
        public SubstituteData Substitute;

        /// <summary>
        /// List of moves that should be recovered when withdrawing from the field.
        /// </summary>
        private List<int> recoverOnWithdrawMoves;

        /// <summary>
        /// Dictionary that stores moves that may have been replaced during the battle.
        /// They will be recovered after it.
        /// Example: Mimic, Transform, etc.
        /// </summary>
        public Dictionary<int, MoveSlot> RecoverableMoves;

        /// <summary>
        /// Battlers this battler has fought.
        /// </summary>
        public List<Battler> BattlersFought;

        /// <summary>
        /// Dictionary with all the volatile statuses this battler has.
        /// </summary>
        public Dictionary<VolatileStatus, int> VolatileStatuses;

        /// <summary>
        /// Information about items consumed or stolen in battle.
        /// </summary>
        public ConsumedItemData ConsumedItemData = new();

        /// <summary>
        /// Flag to know if the battler entered the battle this turn.
        /// </summary>
        public bool EnteredBattleThisTurn;

        /// <summary>
        /// Information about the last performed action.
        /// </summary>
        public LastPerformedActionData LastPerformedAction;

        /// <summary>
        /// Last damage moved received and the damage it dealt.
        /// </summary>
        public LastDamageMoveReceivedData LastReceivedDamageMove;

        /// <summary>
        /// Moves the battler has used since it entered the battle.
        /// </summary>
        public List<Move> MovesUsedSinceEnteringBattle = new();

        /// <summary>
        /// Did the battler receive damage this turn?
        /// </summary>
        public bool ReceivedDamageThisTurn;

        /// <summary>
        /// Did this battler increase its stats this turn?
        /// </summary>
        public bool IncreasedStatsThisTurn;

        /// <summary>
        /// Did this battler decreased its stats this turn?
        /// </summary>
        public bool DecreasedStatsThisTurn;

        /// <summary>
        /// Has the monster eaten a berry during this battle?
        /// </summary>
        public bool HasEatenBerryThisBattle;

        /// <summary>
        /// Number of times this mon has been revived this battle.
        /// </summary>
        public int TimesRevivedThisBattle;

        /// <summary>
        /// Species it had when the battle started.
        /// </summary>
        public MonsterEntry OriginalSpecies;

        /// <summary>
        /// Form it had when the battle started.
        /// </summary>
        public Form OriginalForm;

        /// <summary>
        /// Ability it had when the battle started.
        /// </summary>
        public Ability OriginalAbility;

        /// <summary>
        /// Gender it had when the battle started.
        /// </summary>
        public MonsterGender OriginalGender;

        /// <summary>
        /// Don't allow constructions of the battler outside the class.
        /// Use the FromMonsterInstance() method.
        /// </summary>
        protected Battler()
        {
        }

        /// <summary>
        /// Don't allow constructions of the battler outside the class.
        /// Use the FromMonsterInstance() method.
        /// </summary>
        // ReSharper disable once FunctionComplexityOverflow
        protected Battler(YAPUSettings settings,
                          MonsterDatabaseInstance database,
                          MonsterEntry species,
                          Form form,
                          byte level,
                          Nature nature = null,
                          Ability ability = null,
                          MonsterGender gender = MonsterGender.NonBinary,
                          float heightModifierOverride = -1,
                          float weightModifierOverride = -1,
                          int friendshipModifier = 0,
                          Move[] moveRosterOverride = null,
                          SerializableDictionary<Stat, byte> ivOverride = null,
                          SerializableDictionary<Stat, byte> evOverride = null,
                          SerializableDictionary<Condition, byte> conditionsOverride = null,
                          byte maxFormLevel = 0,
                          string nickname = "",
                          string currentTrainer = "Varguiniano",
                          string originRegion = "YAPULand",
                          string originLocation = "Route 1",
                          string originalTrainer = "",
                          Ball captureBall = null,
                          OriginData.Type originType = OriginData.Type.Caught,
                          bool isAlpha = false,
                          bool hasVirus = false,
                          List<Ribbon> ribbons = null,
                          Item heldItem = null,
                          bool isEgg = false) : base(settings,
                                                     database,
                                                     species,
                                                     form,
                                                     level,
                                                     nature,
                                                     ability,
                                                     gender,
                                                     heightModifierOverride,
                                                     weightModifierOverride,
                                                     friendshipModifier,
                                                     moveRosterOverride,
                                                     null,
                                                     ivOverride,
                                                     evOverride,
                                                     conditionsOverride,
                                                     maxFormLevel,
                                                     nickname,
                                                     currentTrainer,
                                                     originRegion,
                                                     originLocation,
                                                     originalTrainer,
                                                     originType,
                                                     captureBall,
                                                     isAlpha,
                                                     hasVirus,
                                                     ribbons,
                                                     heldItem,
                                                     isEgg)
        {
        }

        /// <summary>
        /// Creates a new battler instance from a monster.
        /// </summary>
        /// <param name="monsterInstance"></param>
        /// <returns></returns>
        public static Battler FromMonsterInstance(MonsterInstance monsterInstance)
        {
            Battler battler = new()
                              {
                                  Species = monsterInstance.Species,
                                  Form = monsterInstance.Form,
                                  OriginData = monsterInstance.OriginData,
                                  PhysicalData = monsterInstance.PhysicalData,
                                  StatData = monsterInstance.StatData.DeepClone(),
                                  Ability = monsterInstance.GetAbility(),
                                  HasNickname = monsterInstance.HasNickname,
                                  Nickname = monsterInstance.Nickname,
                                  CurrentTrainer = monsterInstance.CurrentTrainer,
                                  Friendship = monsterInstance.Friendship,
                                  CurrentHP = monsterInstance.CurrentHP,
                                  Status = monsterInstance.GetStatus(),
                                  CurrentMoves = monsterInstance.CloneMoveSlots(),
                                  LearntMoves = monsterInstance.LearntMoves.ShallowClone(),
                                  Conditions = monsterInstance.Conditions.ShallowClone(),
                                  Sheen = monsterInstance.Sheen,
                                  MaxFormLevel = monsterInstance.MaxFormLevel,
                                  VirusData = monsterInstance.VirusData,
                                  Ribbons = monsterInstance.Ribbons.ShallowClone(),
                                  StatStage = new SerializableDictionary<Stat, short>(),
                                  BattleStatStage = new SerializableDictionary<BattleStat, short>(),
                                  BattlersFought = new List<Battler>(),
                                  HeldItem = monsterInstance.HeldItem,
                                  ExtraData = monsterInstance.ExtraData,
                                  EggData = monsterInstance.EggData
                              };

            foreach (KeyValuePair<Stat, byte> stat in battler.StatData.IndividualValues)
                battler.StatStage[stat.Key] = 0;

            foreach (BattleStat stat in Utils.GetAllItems<BattleStat>()) battler.BattleStatStage[stat] = 0;

            battler.VolatileStatuses = new Dictionary<VolatileStatus, int>();
            battler.OriginalSpecies = battler.Species;
            battler.OriginalForm = battler.Form;
            battler.OriginalAbility = battler.GetAbility();
            battler.OriginalGender = battler.PhysicalData.Gender;

            battler.recoverOnWithdrawMoves = new List<int>();
            battler.RecoverableMoves = new Dictionary<int, MoveSlot>();

            battler.LastPerformedAction = new LastPerformedActionData();

            return battler;
        }

        /// <summary>
        /// Creates a new battler instance from a monster.
        /// </summary>
        /// <returns>A monster instance created from this battler.</returns>
        public MonsterInstance ToMonsterInstance()
        {
            MonsterInstance monster = new()
                                      {
                                          Species = Species,
                                          Form = Form,
                                          OriginData = OriginData,
                                          PhysicalData = PhysicalData,
                                          StatData = StatData.DeepClone(),
                                          HasNickname = HasNickname,
                                          Nickname = Nickname,
                                          CurrentTrainer = CurrentTrainer,
                                          Friendship = Friendship,
                                          CurrentHP = CurrentHP,
                                          CurrentMoves = CloneMoveSlots(),
                                          LearntMoves = LearntMoves.ShallowClone(),
                                          Conditions = Conditions.ShallowClone(),
                                          Sheen = Sheen,
                                          MaxFormLevel = MaxFormLevel,
                                          VirusData = VirusData,
                                          Ribbons = Ribbons.ShallowClone(),
                                          HeldItem = HeldItem,
                                          EggData = EggData,
                                          ExtraData = ExtraData
                                      };

            monster.SetAbility(OriginalAbility);
            monster.SetStatus(Status);
            monster.PhysicalData.Gender = OriginalGender;

            return monster;
        }

        #region General Data

        /// <summary>
        /// Get the name or nickname of the monster.
        /// </summary>
        public override string GetNameOrNickName(ILocalizer localizer)
        {
            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                string candidate = status.GetMonsterName(this);

                if (!candidate.IsNullEmptyOrWhiteSpace()) return candidate;
            }

            return base.GetNameOrNickName(localizer);
        }

        /// <summary>
        /// Get the Weight of the monster in battle.
        /// </summary>
        /// <returns>The Weight of the monster.</returns>
        public float GetWeight(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            float weight = FormData.Weight * PhysicalData.WeightModifier;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
            {
                (float modifier, float multiplier) = GetAbility().GetMonsterWeightInBattle(this);

                weight = weight * multiplier + modifier;
            }

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                (float modifier, float weightOverride) = status.GetMonsterWeightInBattle(this);
                weight += modifier;

                if (weightOverride > 0) weight = weightOverride;
            }

            return Mathf.Max(weight, .1f);
        }

        /// <summary>
        /// Get the Height of the monster in battle.
        /// </summary>
        /// <returns>The Height of the monster.</returns>
        public float GetHeight(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            float height = FormData.Height * PhysicalData.HeightModifier;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                (float modifier, float weightOverride) = status.GetMonsterHeightInBattle(this);
                height += modifier;

                if (weightOverride > 0) height = weightOverride;
            }

            return Mathf.Max(height, .1f);
        }

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="stat">Stat to be calculated.</param>
        /// <param name="form">Form to calculate the stat with, this may not be the same form as the current one.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="overrideBaseValue">If > 0 override the base value with this value.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(Stat stat,
                                              Form form,
                                              BattleManager battleManager,
                                              out uint overrideBaseValue)
        {
            float multiplier = base.OnCalculateStat(stat, form, battleManager, out overrideBaseValue);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                multiplier *= status.OnCalculateStat(this, stat, out uint statusOverrideBaseValue);
                overrideBaseValue = statusOverrideBaseValue;
            }

            if (battleManager == null) return multiplier;

            foreach (SideStatus status in
                     battleManager.Statuses.GetSideStatuses(battleManager.Battlers.GetTypeAndIndexOfBattler(this)
                                                                         .Type)
                                  .Select(slot => slot.Key))
                multiplier *= status.OnCalculateStat(this, stat);

            if (battleManager.Scenario.GetWeather(out Weather weather))
                multiplier *= weather.OnCalculateStat(this, stat, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the statuses and items to change the modifier.
        /// </summary>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public IEnumerator OnStatChange(Stat stat,
                                        short modifier,
                                        BattlerType userType,
                                        int userIndex,
                                        bool effectIgnoresAbilities,
                                        BattleManager battleManager,
                                        Ability changingAbility,
                                        Action<short> callback)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .OnStatChange(this,
                                 stat,
                                 modifier,
                                 userType,
                                 userIndex,
                                 battleManager,
                                 changingAbility,
                                 newModifier => modifier = newModifier);

            callback.Invoke(modifier);
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the statuses and items to change the modifier.
        /// </summary>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public IEnumerator OnStatChange(BattleStat stat,
                                        short modifier,
                                        BattlerType userType,
                                        int userIndex,
                                        bool effectIgnoresAbilities,
                                        BattleManager battleManager,
                                        Action<short> callback)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .OnStatChange(this,
                                 stat,
                                 modifier,
                                 userType,
                                 userIndex,
                                 battleManager,
                                 newModifier => modifier = newModifier);

            callback.Invoke(modifier);
        }

        /// <summary>
        /// Callback after a stat stage has been changed.
        /// </summary>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator AfterStatChanged(Stat stat,
                                            short modifier,
                                            BattlerType userType,
                                            int userIndex,
                                            bool effectIgnoresAbilities,
                                            BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .AfterStatChanged(this,
                                     stat,
                                     modifier,
                                     userType,
                                     userIndex,
                                     battleManager);
        }

        /// <summary>
        /// Callback after a stat stage has been changed.
        /// </summary>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator AfterStatChanged(BattleStat stat,
                                            short modifier,
                                            BattlerType userType,
                                            int userIndex,
                                            bool effectIgnoresAbilities,
                                            BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .AfterStatChanged(this,
                                     stat,
                                     modifier,
                                     userType,
                                     userIndex,
                                     battleManager);
        }

        /// <summary>
        /// Retrieve the accuracy stage.
        /// </summary>
        public short GetAccuracyStage(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility().OnCalculateAccuracyStage(this, battleManager);

            return (short) Mathf.RoundToInt(BattleStatStage[BattleStat.Accuracy] * multiplier);
        }

        /// <summary>
        /// Retrieve the evasion stage.
        /// </summary>
        public short GetEvasionStage(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility().OnCalculateEvasionStage(this, battleManager);

            return (short) Mathf.RoundToInt(BattleStatStage[BattleStat.Evasion] * multiplier);
        }

        /// <summary>
        /// Can this battler be targeted by a move?
        /// </summary>
        /// <returns>True if it can.</returns>
        public bool CanBeTargeted() => CanBattle;

        /// <summary>
        /// Is this monster on the ground?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="showNotificationIfPrevented">Should the item or ability show a notification if grounding is prevented?</param>
        /// <returns>True if it is.</returns>
        public bool IsGrounded(BattleManager battleManager,
                               bool effectIgnoresAbilities,
                               bool showNotificationIfPrevented = false)
        {
            bool groundingPrevented = false;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
            {
                (bool forcesGrounding, bool preventsGrounding) =
                    GetAbility().IsGrounded(this, battleManager, showNotificationIfPrevented);

                if (forcesGrounding) return true;

                groundingPrevented |= preventsGrounding;
            }

            // If any status forces grounding, return true.
            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                (bool forcesGrounding, bool preventsGrounding) = status.IsGrounded(this);

                if (forcesGrounding) return true;

                groundingPrevented |= preventsGrounding;
            }

            if (battleManager.Statuses.GetGlobalStatuses().Any(status => status.Key.IsGrounded(this, battleManager)))
                return true;

            if (CanUseHeldItemInBattle(battleManager))
            {
                HeldItem.OnCheckGrounding(battleManager,
                                          this,
                                          out bool itemPreventsGrounding,
                                          out bool itemForcesGrounding);

                if (itemForcesGrounding) return true;

                if (itemPreventsGrounding) groundingPrevented = true;
            }

            (MonsterType type1, MonsterType type2) = GetTypes(battleManager.YAPUSettings);

            if (type1.PreventsGrounding || (type2 != null && type2.PreventsGrounding)) groundingPrevented = true;

            return !groundingPrevented;
        }

        /// <summary>
        /// Can this monster switch to its mega form?
        /// </summary>
        public bool CanSwitchToMegaForm(BattleManager battleManager)
        {
            if (!CanChangeForm(battleManager)) return false;

            if (!CanUseHeldItemInBattle(battleManager) || Species != OriginalSpecies || Form != OriginalForm)
                return false;

            foreach (KeyValuePair<Item, Form> pair in FormData.MegaEvolutions)
                if (pair.Key == HeldItem)
                    return true;

            return false;
        }

        /// <summary>
        /// Retrieve the types of this battler.
        /// They can be modified by volatile states.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The two types of the monster.</returns>
        public override (MonsterType, MonsterType) GetTypes(YAPUSettings settings)
        {
            if (EggData.IsEgg) return base.GetTypes(settings);

            (MonsterType firstCalculated, MonsterType secondCalculated) = (FormData.FirstType, FormData.SecondType);

            foreach (KeyValuePair<VolatileStatus, int> pair in VolatileStatuses)
                (firstCalculated, secondCalculated) =
                    pair.Key.OnCalculateTypes(this, firstCalculated, secondCalculated);

            return (firstCalculated, secondCalculated);
        }

        /// <summary>
        /// Get this monster's ability.
        /// </summary>
        /// <returns>The monster's ability.</returns>
        public override Ability GetAbility()
        {
            // If volatiles override ability, return that.
            foreach (Ability abilityOverride in VolatileStatuses.Select(status => status.Key.OnGetAbility(this))
                                                                .Where(abilityOverride => abilityOverride != null))
                return abilityOverride;

            return base.GetAbility();
        }

        /// <summary>
        /// Can this monster use its ability in battle?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="effectIgnoresAbilities">Does the effect being use ignore abilities?</param>
        /// <param name="dontCheckOtherMonstersAbilities">Flag to use when this is being called from another CanUseAbility method, in other to prevent a stack overflow.</param>
        /// <returns>True if it can.</returns>
        public bool CanUseAbility(BattleManager battleManager,
                                  bool effectIgnoresAbilities,
                                  bool dontCheckOtherMonstersAbilities = false)
        {
            if (effectIgnoresAbilities && GetAbility().IsIgnorable) return false;

            bool canUse = true;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                canUse &= status.CanUseAbility(this, battleManager);

            if (dontCheckOtherMonstersAbilities) return canUse;

            foreach (Battler other in battleManager.Battlers.GetBattlersFighting().Where(other => other != this))
                canUse &= other.CanOtherMonsterUseAbility(this, battleManager, effectIgnoresAbilities);

            return canUse;
        }

        /// <summary>
        /// Can another monster use their ability?
        /// </summary>
        /// <param name="other">Other monster.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="effectIgnoresAbilities">Does the effect being use ignore abilities?</param>
        /// <returns>True if it can.</returns>
        private bool CanOtherMonsterUseAbility(Battler other, BattleManager battleManager, bool effectIgnoresAbilities)
        {
            bool canUse = true;

            if (CanUseAbility(battleManager, effectIgnoresAbilities, true))
                canUse &= GetAbility().CanOtherMonsterUseAbility(this, other, battleManager);

            return canUse;
        }

        /// <summary>
        /// Does this monster ignore others' abilities when using a move?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <returns>True if it ignores other abilities.</returns>
        public virtual bool IgnoresOtherAbilities(BattleManager battleManager, Move move)
        {
            bool ignores = false;

            if (CanUseAbility(battleManager, false))
                ignores |= GetAbility().IgnoresOtherAbilities(battleManager, this, move);

            return ignores;
        }

        /// <summary>
        /// Can the battler change forms?
        /// </summary>
        public bool CanChangeForm(BattleManager battleManager)
        {
            bool can = true;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                can &= status.CanChangeForm(this, battleManager);

            return can;
        }

        /// <summary>
        /// Can the battler transform?
        /// </summary>
        public bool CanTransform(BattleManager battleManager)
        {
            bool can = CanChangeForm(battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                can &= status.CanTransform(this, battleManager);

            return can;
        }

        /// <summary>
        /// Mega form available for this battler.
        /// </summary>
        public Form GetAvailableMegaForm(BattleManager battleManager) =>
            CanSwitchToMegaForm(battleManager) ? FormData.MegaEvolutions[HeldItem] : null;

        /// <summary>
        /// Get the catch rate of the monster in battle.
        /// </summary>
        public byte GetCatchRateInBattle(BattleManager battleManager)
        {
            byte catchRate = FormData.CatchRate;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                catchRate = status.GetCatchRate(this, catchRate, battleManager);

            return catchRate;
        }

        /// <summary>
        /// Reset all the stages. Called when withdrawing.
        /// </summary>
        public void ResetStages()
        {
            ResetStatStages();
            BattleStatStage[BattleStat.Accuracy] = 0;
            BattleStatStage[BattleStat.Evasion] = 0;
            CriticalStage = 0;

            EnteredBattleThisTurn = false;
            LastReceivedDamageMove = null;
            ReceivedDamageThisTurn = false;
            IncreasedStatsThisTurn = false;
            DecreasedStatsThisTurn = false;
        }

        /// <summary>
        /// Reset the stages for the stats.
        /// </summary>
        public void ResetStatStages()
        {
            StatStage[Stat.Hp] = 0;
            StatStage[Stat.Attack] = 0;
            StatStage[Stat.Defense] = 0;
            StatStage[Stat.SpecialAttack] = 0;
            StatStage[Stat.SpecialDefense] = 0;
            StatStage[Stat.Speed] = 0;
        }

        /// <summary>
        /// Reset all stat stages that have been lowered.
        /// </summary>
        /// <returns>True if any stage was restored.</returns>
        public bool ResetLoweredStatStages()
        {
            bool anyRestored = false;

            foreach (Stat stat in Utils.GetAllItems<Stat>())
            {
                if (StatStage[stat] >= 0) continue;

                anyRestored = true;
                StatStage[stat] = 0;
            }

            return anyRestored;
        }

        /// <summary>
        /// Return to its original form.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="replaceHp">Replace the Hp using a percentage?</param>
        public IEnumerator ReturnToOriginalForm(BattleManager battleManager, bool replaceHp = true)
        {
            // Go back to the original form.
            if (Species == OriginalSpecies && Form == OriginalForm) yield break;

            (BattlerType ownType, int ownIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(this);

            yield return battleManager.Battlers.ChangeToSpeciesAndForm(ownType,
                                                                       ownIndex,
                                                                       OriginalSpecies,
                                                                       OriginalForm,
                                                                       false,
                                                                       replaceHp: replaceHp,
                                                                       overrideCanChange: true,
                                                                       registerOnDex: false);

            Ability = OriginalAbility;
            PhysicalData.Gender = OriginalGender;
        }

        #endregion

        #region Battle State Machine

        /// <summary>
        /// Called when the battle has started.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnBattleStarted(BattleManager battleManager)
        {
            yield return GetAbility().OnBattleStarted(battleManager, this);
        }

        /// <summary>
        /// Called when the battle has ended.
        /// </summary>
        public IEnumerator OnBattleEnded(BattleManager battleManager)
        {
            foreach (KeyValuePair<VolatileStatus, int> pair in VolatileStatuses)
                yield return pair.Key.OnBattleEnded(this);

            foreach (MoveSlot slot in CurrentMoves)
                if (slot.Move != null)
                    yield return slot.Move.OnBattleEnded(this, battleManager);

            foreach (Move move in LearntMoves) yield return move.OnBattleEnded(this, battleManager);

            if (Status != null) yield return Status.OnBattleEnded(this);

            yield return GetAbility().OnBattleEnded(this, battleManager);
        }

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnMonsterEnteredBattle(BattleManager battleManager)
        {
            EnteredBattleThisTurn = true;

            if (GetStatus() != null) yield return GetStatus().OnMonsterEnteredBattle(this, battleManager);

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().OnMonsterEnteredBattle(battleManager, this);

            if (!CanUseHeldItemInBattle(battleManager)) yield break;

            bool consumeItem = false;

            yield return HeldItem.OnMonsterEnteredBattle(this,
                                                         battleManager,
                                                         shouldConsume => consumeItem = shouldConsume);

            if (consumeItem) yield return ConsumeItemInBattle(battleManager);
        }

        /// <summary>
        /// Called when the monster is withdrawn from battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnMonsterLeavingBattle(BattleManager battleManager)
        {
            yield return RemoveAllVolatileStatusWithoutAnimation(battleManager);

            ResetStages();

            MovesUsedSinceEnteringBattle.Clear();

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().OnMonsterLeavingBattle(battleManager, this);

            foreach (int index in recoverOnWithdrawMoves)
            {
                CurrentMoves[index] = RecoverableMoves[index];
                RecoverableMoves.Remove(index);
            }

            recoverOnWithdrawMoves.Clear();
        }

        /// <summary>
        /// Called when another monster is withdrawn from battle.
        /// </summary>
        /// <param name="other">Monster leaving.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnOtherMonsterLeavingBattle(Battler other, BattleManager battleManager)
        {
            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                yield return status.OnOtherMonsterLeavingBattle(this, other, battleManager);
        }

        /// <summary>
        /// Request this battler for an action forced by its items or statuses.
        /// </summary>
        /// <param name="battleManager">Reference to the battler manager.</param>
        /// <param name="battleAction">Generated battle action.</param>
        public bool RequestForcedAction(BattleManager battleManager, out BattleAction battleAction)
        {
            foreach (KeyValuePair<VolatileStatus, int> status in VolatileStatuses)
                if (status.Key.RequestForcedAction(this, battleManager, out battleAction))
                    return true;

            battleAction = default;
            return false;
        }

        /// <summary>
        /// Called to determine the priority of the battler inside its priority bracket.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Finished callback stating if  it should go first (1), last (-1) or normal (0).</param>
        public IEnumerator OnDeterminePriority(BattleManager battleManager,
                                               Action<int> finished)
        {
            int priorityModifier = 0;

            if (CanUseAbility(battleManager, false))
                yield return GetAbility()
                   .OnDeterminePriority(this,
                                        battleManager,
                                        newPriorityModifier =>
                                        {
                                            priorityModifier = newPriorityModifier;
                                        });

            if (priorityModifier == 0 && CanUseHeldItemInBattle(battleManager))
            {
                bool consume = false;

                yield return HeldItem.OnDeterminePriority(this,
                                                          battleManager,
                                                          battleManager.Localizer,
                                                          (shouldConsume, newPriorityModifier) =>
                                                          {
                                                              if (shouldConsume) consume = true;

                                                              priorityModifier = newPriorityModifier;
                                                          });

                if (consume) yield return ConsumeItemInBattle(battleManager);
            }

            finished.Invoke(priorityModifier);
        }

        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="action">Action that was performed.</param>
        /// <param name="user">User of the action.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator AfterAction(BattleAction action,
                                       Battler user,
                                       BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility().AfterAction(this, action, user, battleManager);

            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consume = false;

                yield return HeldItem.AfterAction(this,
                                                  action,
                                                  battleManager,
                                                  battleManager.Localizer,
                                                  shouldConsume => consume = shouldConsume);

                if (consume) yield return ConsumeItemInBattle(battleManager);

                DialogManager.AcceptInput = true;

                yield return DialogManager.WaitForDialog;
            }

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
            {
                yield return status.AfterAction(this, action, user, battleManager);
            }
        }

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public IEnumerator AfterTurnPreStatus(BattleManager battleManager,
                                              ILocalizer localizer)
        {
            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consume = false;

                yield return HeldItem.AfterTurnPreStatus(this,
                                                         battleManager,
                                                         localizer,
                                                         shouldConsume => consume = shouldConsume);

                if (consume) yield return ConsumeItemInBattle(battleManager);

                yield return DialogManager.WaitForDialog;
            }

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().AfterTurnPreStatus(this, battleManager, localizer);
        }

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public IEnumerator AfterTurnPostStatus(BattleManager battleManager,
                                               ILocalizer localizer)
        {
            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consume = false;

                yield return HeldItem.AfterTurnPostStatus(this,
                                                          battleManager,
                                                          localizer,
                                                          shouldConsume => consume = shouldConsume);

                if (consume) yield return ConsumeItemInBattle(battleManager);

                yield return DialogManager.WaitForDialog;
            }

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().AfterTurnPostStatus(this, battleManager, localizer);

            foreach (Move move in CurrentMoves.Select(slot => slot.Move).Where(move => move != null))
                yield return move.AfterTurnPostStatus(this, battleManager);
        }

        /// <summary>
        /// Check if this battler can switch.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Display dialog messages stating why the battler Switch?</param>
        /// <returns>True if it can.</returns>
        public bool CanSwitch(BattleManager battleManager,
                              BattlerType userType,
                              int userIndex,
                              Move userMove,
                              bool effectIgnoresAbilities,
                              Item item,
                              bool itemBelongsToUser,
                              bool showMessages)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities)
             && !GetAbility()
                   .CanSwitch(this,
                              battleManager,
                              userType,
                              userIndex,
                              userMove,
                              item,
                              itemBelongsToUser,
                              showMessages))
                return false;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<VolatileStatus, int> pair in VolatileStatuses)
                if (!pair.Key.CanSwitch(this,
                                        battleManager,
                                        userType,
                                        userIndex,
                                        userMove,
                                        item,
                                        itemBelongsToUser,
                                        showMessages))
                    return false;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Battler opponent in battleManager.Battlers.GetBattlersFighting(userType == BattlerType.Ally
                         ? BattlerType.Enemy
                         : BattlerType.Ally))
                if (opponent.CanUseAbility(battleManager, effectIgnoresAbilities)
                 && !opponent.GetAbility()
                             .CanOpponentSwitch(opponent,
                                                this,
                                                battleManager,
                                                userType,
                                                userIndex,
                                                userMove,
                                                item,
                                                itemBelongsToUser,
                                                showMessages))
                    return false;

            return true;
        }

        /// <summary>
        /// Can this monster be caught by a ball?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can be caught.</returns>
        public bool CanBeCaught(BattleManager battleManager)
        {
            // TODO: Global flag in the battle manager for some story battles.

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<VolatileStatus, int> pair in VolatileStatuses)
                if (!pair.Key.CanBeCaught(this, battleManager))
                    return false;

            return true;
        }

        /// <summary>
        /// Check if this battler can run away.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="showMessages">Display dialog messages stating why the battler can't run?</param>
        /// <returns>True if it can.</returns>
        public bool CanRunAway(BattleManager battleManager, bool effectIgnoresAbilities, bool showMessages)
        {
            bool canRun = true;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<VolatileStatus, int> _ in
                     VolatileStatuses.Where(pair => !pair.Key.CanRunAway(this, battleManager, showMessages)))
                canRun = false;

            bool abilityAllowsRunning = true;
            bool abilityOverridesAndAlwaysCan = false;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                (abilityAllowsRunning, abilityOverridesAndAlwaysCan) =
                    GetAbility().CanRunAway(this, battleManager, showMessages);

            if (abilityOverridesAndAlwaysCan) return true;

            canRun &= abilityAllowsRunning;

            foreach (Battler opponent in
                     battleManager.Battlers.GetBattlersFighting(battleManager.Battlers.GetTypeAndIndexOfBattler(this)
                                                                             .Type
                                                             == BattlerType.Ally
                                                                    ? BattlerType.Enemy
                                                                    : BattlerType.Ally))
            {
                if (opponent.CanUseAbility(battleManager, false))
                    (abilityAllowsRunning, abilityOverridesAndAlwaysCan) =
                        opponent.GetAbility().CanOpponentMonsterRunAway(opponent, this, battleManager, showMessages);

                if (abilityOverridesAndAlwaysCan) return true;

                canRun &= abilityAllowsRunning;
            }

            // ReSharper disable once InvertIf
            if (CanUseHeldItemInBattle(battleManager))
            {
                (bool itemAllowsRunning, bool itemOverridesAndAlwaysCan) =
                    HeldItem.CanRunAway(this, battleManager, showMessages);

                if (itemOverridesAndAlwaysCan) return true;

                canRun &= itemAllowsRunning;
            }

            return canRun;
        }

        /// <summary>
        /// Called at the end of each turn to reset flags.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator ResetTurnFlags(BattleManager battleManager)
        {
            EnteredBattleThisTurn = false;
            ReceivedDamageThisTurn = false;
            IncreasedStatsThisTurn = false;
            DecreasedStatsThisTurn = false;
            yield break;
        }

        /// <summary>
        /// Called when the battler runs away.
        /// </summary>
        public IEnumerator OnRunAway(BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false)) yield return GetAbility().OnRunAway(this, battleManager);
            if (CanUseHeldItemInBattle(battleManager)) yield return HeldItem.OnRunAway(this, battleManager);
        }

        #endregion

        #region Health

        /// <summary>
        /// Change the HP of this monster.
        /// </summary>
        /// <param name="amount">The unclamped amount to change the HP to.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battlerType">Type of this battler.</param>
        /// <param name="battlerIndex">Index of this battler.</param>
        /// <param name="userType">Type of the origin of the hp change.</param>
        /// <param name="userIndex">Index of the origin of the hp change.</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="userMove">Move that inflicted the damage.</param>
        /// <param name="callback">Callback with the new HP, the previous HP and if the substitute took the hit.</param>
        /// <param name="bypassSubstitute">Does the effect bypass the substitute?</param>
        internal IEnumerator ChangeHPInBattle(int amount,
                                              BattleManager battleManager,
                                              BattlerType battlerType,
                                              int battlerIndex,
                                              BattlerType userType,
                                              int userIndex,
                                              bool isSecondaryDamage,
                                              bool effectIgnoresAbilities,
                                              Move userMove = null,
                                              Action<int, int, bool> callback = null,
                                              bool forceSurvive = false,
                                              bool bypassSubstitute = false)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (userMove != null && userMove.DamageBypassesSubstitute) bypassSubstitute = true;

            if (user != null
             && user.CanUseAbility(battleManager, effectIgnoresAbilities)
             && user.GetAbility()
                    .ByPassesSubstitute(battlerType, battlerIndex, battleManager, userType, userIndex, userMove))
                bypassSubstitute = true;

            // If there is a substitute and an enemy wants to hit, take life from the substitute.
            if (!bypassSubstitute
             && Substitute.SubstituteEnabled
             && amount <= 0
             && userIndex != -2
             && battlerType != userType)
                yield return MakeSubstituteLoseHP(amount, battleManager, callback);
            else
                yield return ChangeHPAfterSubstituteChecks(amount,
                                                           battleManager,
                                                           battlerType,
                                                           battlerIndex,
                                                           userType,
                                                           userIndex,
                                                           user,
                                                           isSecondaryDamage,
                                                           effectIgnoresAbilities,
                                                           userMove,
                                                           callback,
                                                           forceSurvive);
        }

        /// <summary>
        /// Make the substitute lose HP from a hit.
        /// </summary>
        /// <param name="amount">The unclamped amount to change the HP to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback with the new HP, the previous HP and if the substitute took the hit.</param>
        private IEnumerator MakeSubstituteLoseHP(int amount,
                                                 BattleManager battleManager,
                                                 Action<int, int, bool> callback = null)
        {
            int previousHP = (int) Substitute.CurrentHP;

            int uncappedDelta = (int) (Substitute.CurrentHP + amount);

            amount = Mathf.Min(uncappedDelta, (int) Substitute.MaxHP);

            amount = Mathf.Max(0, amount);

            Substitute.CurrentHP = (uint) amount;

            if (Substitute.CurrentHP == 0)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Broken",
                                                             localizableModifiers: false,
                                                             modifiers: GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                Substitute.SubstituteEnabled = false;

                yield return battleManager.GetMonsterSprite(this).HideSubstitute(battleManager);
            }

            callback?.Invoke((int) Substitute.CurrentHP, previousHP, true);
        }

        /// <summary>
        /// Change the HP of this monster after the substitute checks.
        /// </summary>
        /// <param name="amount">The unclamped amount to change the HP to.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battlerType">Type of this battler.</param>
        /// <param name="battlerIndex">Index of this battler.</param>
        /// <param name="userType">Type of the origin of the hp change.</param>
        /// <param name="userIndex">Index of the origin of the hp change.</param>
        /// <param name="user">Origin of the hp change.</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="userMove">Move that inflicted the damage.</param>
        /// <param name="callback">Callback with the new HP, the previous HP and if the substitute took the hit.</param>
        private IEnumerator ChangeHPAfterSubstituteChecks(int amount,
                                                          BattleManager battleManager,
                                                          BattlerType battlerType,
                                                          int battlerIndex,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          Battler user,
                                                          bool isSecondaryDamage,
                                                          bool effectIgnoresAbilities,
                                                          Move userMove = null,
                                                          Action<int, int, bool> callback = null,
                                                          bool forceSurvive = false)
        {
            int newHP = (int) CurrentHP;
            int prevHP = (int) CurrentHP;

            if (amount > 0
             || !isSecondaryDamage
             || (CanUseAbility(battleManager, effectIgnoresAbilities)
              && GetAbility().IsAffectedBySecondaryDamageEffects(this, user, amount, battleManager)))
            {
                bool abilityTriggeredForceSurvive = false;

                if (!forceSurvive)
                {
                    abilityTriggeredForceSurvive = GetAbility()
                       .ShouldForceSurvive(this,
                                           amount,
                                           battleManager,
                                           userType,
                                           userIndex,
                                           isSecondaryDamage,
                                           userMove);

                    forceSurvive |= abilityTriggeredForceSurvive;
                }

                bool itemTriggeredForceSurvive = false;

                if (!forceSurvive && CanUseHeldItemInBattle(battleManager))
                    itemTriggeredForceSurvive =
                        HeldItem.ShouldForceSurvive(this,
                                                    amount,
                                                    battleManager,
                                                    userType,
                                                    userIndex,
                                                    isSecondaryDamage,
                                                    userMove);

                forceSurvive |= itemTriggeredForceSurvive;

                bool triggeredForceSurvive;

                (newHP, prevHP, triggeredForceSurvive) = ChangeHP(amount, forceSurvive);

                if (triggeredForceSurvive && abilityTriggeredForceSurvive)
                    yield return GetAbility()
                       .OnForceSurvive(this,
                                       amount,
                                       battleManager,
                                       userType,
                                       userIndex,
                                       isSecondaryDamage,
                                       userMove);

                if (triggeredForceSurvive && itemTriggeredForceSurvive)
                {
                    bool consume = false;

                    yield return HeldItem.OnForceSurvive(this,
                                                         amount,
                                                         battleManager,
                                                         userType,
                                                         userIndex,
                                                         isSecondaryDamage,
                                                         shouldConsume => consume = shouldConsume,
                                                         userMove);

                    if (consume) yield return ConsumeItemInBattle(battleManager);
                }

                if (userMove != null)
                {
                    LastReceivedDamageMove =
                        new LastDamageMoveReceivedData(userMove,
                                                       prevHP - newHP,
                                                       battleManager.Battlers.GetBattlerFromBattleIndex(userType,
                                                           userIndex));

                    ReceivedDamageThisTurn = true;
                }
            }

            if (newHP == 0 && user != this && user != null)
            {
                yield return OnKnockedOutByBattler(userType, userIndex, userMove, battleManager);

                yield return user.OnKnockedOutBattler(battlerType, battlerIndex, battleManager);
            }

            callback?.Invoke(newHP, prevHP, false);
        }

        /// <summary>
        /// Can this battler heal?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it can.</returns>
        public bool CanHeal(BattleManager battleManager)
        {
            bool canHeal = true;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                canHeal &= status.CanHeal(this, battleManager);

            return canHeal;
        }

        /// <summary>
        /// Calculate the multiplier to use when draining HP.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="target">Target to drain from, can be null.</param>
        /// <returns>The multiplier to apply.</returns>
        public float CalculateDrainHPMultiplier(BattleManager battleManager, Battler target)
        {
            float multiplier = 1;

            if (CanUseHeldItemInBattle(battleManager))
                multiplier *= HeldItem.CalculateDrainHPMultiplier(battleManager, this, target);

            return multiplier;
        }

        /// <summary>
        /// Calculate the multiplier to use this monster's HP is being drained.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="drainer">Monster draining the HP.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <returns>The multiplier to apply.</returns>
        public float CalculateDrainerDrainHPMultiplier(BattleManager battleManager,
                                                       Battler drainer,
                                                       bool effectIgnoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility().CalculateDrainerDrainHPMultiplier(this, drainer, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Called when the battler faints.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ownType">This battler's own battler type.</param>
        /// <param name="ownIndex">This battler's own index.</param>
        public IEnumerator OnFainted(BattleManager battleManager, BattlerType ownType, int ownIndex)
        {
            // Fainting should remove status.
            yield return RemoveStatusInBattle(battleManager, false);

            // If it belongs to the player, reduce the friendship.
            // Based on: https://bulbapedia.bulbagarden.net/wiki/Friendship#Generation_VII
            if (ownType == BattlerType.Ally
             && battleManager.Battlers.GetNumberOfBattlersUnderPlayersControl() > ownIndex)
            {
                if (StatData.Level - StatData.Level > 30)
                {
                    if (Friendship < 200)
                        ChangeFriendship(-5, battleManager.PlayerCharacter.Scene.Asset, battleManager.Localizer);
                    else
                        ChangeFriendship(-10, battleManager.PlayerCharacter.Scene.Asset, battleManager.Localizer);
                }
                else
                    ChangeFriendship(-1, battleManager.PlayerCharacter.Scene.Asset, battleManager.Localizer);
            }

            yield return ReturnToOriginalForm(battleManager);
        }

        /// <summary>
        /// Called when another battler has fainted.
        /// </summary>
        /// <param name="otherType">Fainted battler.</param>
        /// <param name="otherIndex">Fainted battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnOtherBattlerFainted(BattlerType otherType, int otherIndex, BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility().OnOtherBattlerFainted(this, otherType, otherIndex, battleManager);
        }

        /// <summary>
        /// Called when this battler is knocked out by another battler
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="userType">User of the effect.</param>
        /// <param name="userIndex">User of the effect.</param>
        /// <param name="userMove">Move used for the knock out, if any.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator OnKnockedOutByBattler(BattlerType userType,
                                                  int userIndex,
                                                  Move userMove,
                                                  BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility().OnKnockedOutByBattler(this, userType, userIndex, userMove, battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                yield return status.OnKnockedOutByBattler(this, userType, userIndex, userMove, battleManager);
        }

        /// <summary>
        /// Called when this battler knocks out another battler.
        /// For example, by using a move on them.
        /// </summary>
        /// <param name="otherType">Fainted battler.</param>
        /// <param name="otherIndex">Fainted battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator OnKnockedOutBattler(BattlerType otherType, int otherIndex, BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility().OnKnockedOutBattler(this, otherType, otherIndex, battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                yield return status.OnKnockedOutBattler(this, otherType, otherIndex, battleManager);
        }

        #endregion

        #region Item

        /// <summary>
        /// Called to check if the monster can use its held item.
        /// </summary>
        /// <returns>True if it can be used.</returns>
        public bool CanUseHeldItemInBattle(BattleManager battleManager)
        {
            bool canUse = CanUseHeldItem();

            if (!canUse) return false;

            foreach (GlobalStatus status in battleManager.Statuses.GetGlobalStatuses().Select(slot => slot.Key))
                canUse &= status.CanUseHeldItem(this, battleManager);

            foreach (Battler otherBattler in battleManager.Battlers.GetBattlersFighting()
                                                          .Where(battler => battler != this))
                canUse &= otherBattler.CanOtherMonsterUseHeldItem(this, HeldItem, false, battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                canUse &= status.CanUseHeldItem(this);

            return canUse;
        }

        /// <summary>
        /// Check if this battler allows other battlers to use a held item.
        /// </summary>
        /// <param name="other">Battler attempting to use the item.</param>
        /// <param name="itemToUse">Item they want to use.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public bool CanOtherMonsterUseHeldItem(Battler other,
                                               Item itemToUse,
                                               bool effectIgnoresAbilities,
                                               BattleManager battleManager) =>
            !CanUseAbility(battleManager, effectIgnoresAbilities)
         || GetAbility().CanOtherMonsterUseHeldItem(this, other, itemToUse, battleManager);

        /// <summary>
        /// Check if the monster can use bag items.
        /// </summary>
        /// <returns>True if it can.</returns>
        public bool CanUseBagItem(Item item, BattleManager battleManager)
        {
            bool canUse = true;

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                canUse &= status.CanUseBagItem(this, item, battleManager);

            return canUse;
        }

        /// <summary>
        /// Consume the held item in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="customMessage">Custom message when consuming.</param>
        /// <param name="hasBeenConsumedByOwner">Has the item been consumed by the battler or by other, like with Fling?</param>
        public IEnumerator ConsumeItemInBattle(BattleManager battleManager,
                                               string customMessage = "Battle/ItemConsumed",
                                               bool hasBeenConsumedByOwner = true)
        {
            if (!CanUseHeldItemInBattle(battleManager))
            {
                Logger.Error("The item can't be used! This should be checked before calling this method.");
                yield break;
            }

            yield return HeldItem.OnItemConsumed();

            yield return HeldItem.OnItemConsumedInBattle(this, battleManager);

            if (HeldItem is Berry) HasEatenBerryThisBattle = true;

            yield return DialogManager.ShowDialogAndWait(customMessage,
                                                         localizableModifiers: false,
                                                         modifiers: HeldItem.GetName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            ConsumedItemData.ConsumedItem = HeldItem;
            ConsumedItemData.CanBeRecycled = hasBeenConsumedByOwner;
            ConsumedItemData.CanBeRecoveredAfterBattle = true;

            HeldItem = null;
        }

        /// <summary>
        /// Called each time the battler eats a berry.
        /// </summary>
        /// <param name="berry">Berry that was eaten.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator OnBerryEaten(Berry berry, BattleManager battleManager)
        {
            if (CanUseAbility(battleManager, false)) yield return GetAbility().OnBerryEaten(berry, this, battleManager);
        }

        #endregion

        #region Moves

        /// <summary>
        /// Replace a move only for the current battle.
        /// </summary>
        /// <param name="index">Index to replace.</param>
        /// <param name="move">New move.</param>
        /// <param name="recoverOriginalOnWithdraw">Recover the original when withdrawing from the field?</param>
        /// <param name="currentPPOverride">Override for the move's PP.</param>
        /// <param name="maxPPOverride">Override for the move's max PP.</param>
        public void TemporaryReplaceMove(int index,
                                         Move move,
                                         bool recoverOriginalOnWithdraw,
                                         int currentPPOverride = -1,
                                         byte maxPPOverride = 0)
        {
            if (!RecoverableMoves.ContainsKey(index)) RecoverableMoves[index] = CurrentMoves[index];

            CurrentMoves[index] = new MoveSlot(move)
                                  {
                                      CurrentPP =
                                          currentPPOverride >= 0 ? (byte) currentPPOverride : move.BasePowerPoints,
                                      MaxPP = maxPPOverride > 0 ? maxPPOverride : move.BasePowerPoints
                                  };

            if (recoverOriginalOnWithdraw) recoverOnWithdrawMoves.AddIfNew(index);
        }

        /// <summary>
        /// Set the last move performed and if it was successful.
        /// </summary>
        /// <param name="move">Move to set.</param>
        /// <param name="wasSuccessful">Was the move successful?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        internal void SetLastPerformedMove(Move move, bool wasSuccessful, BattleManager battleManager)
        {
            LastPerformedAction.SetLastMove(move, wasSuccessful);
            MovesUsedSinceEnteringBattle.AddIfNew(move);

            if (wasSuccessful) battleManager.Moves.LastSuccessfullyPerformedMove = move;
        }

        /// <summary>
        /// Retrieve a list of the moves that can be used.
        /// </summary>
        /// <returns></returns>
        public override List<MoveSlot> GetUsableMoves(BattleManager battleManager)
        {
            List<MoveSlot> usableMoves =
                base.GetUsableMoves(battleManager).Where(slot => slot.Move != null && slot.CurrentPP > 0).ToList();

            usableMoves = VolatileStatuses.Aggregate(usableMoves,
                                                     (current, volatileStatusSlot) =>
                                                         volatileStatusSlot.Key.OnRetrieveUsableMoves(this, current));

            foreach (Battler otherBattler in battleManager.Battlers.GetBattlersFighting()
                                                          .Where(battler => battler != this))
            {
                foreach (VolatileStatus status in otherBattler.VolatileStatuses.Select(slot => slot.Key))
                    usableMoves = status.OnRetrieveUsableMovesForOtherBattler(otherBattler,
                                                                              this,
                                                                              usableMoves,
                                                                              battleManager);
            }

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                usableMoves = status.OnRetrieveUsableMoves(this, usableMoves);

            return usableMoves;
        }

        /// <summary>
        /// Called when calculating the priority of a move this monster is going to use.
        /// </summary>
        public int GetMovePriorityModifier(Move move,
                                           List<Battler> targets,
                                           int currentPriority,
                                           BattleManager battleManager,
                                           bool showNotifications)
        {
            int modifier = 0;

            if (CanUseAbility(battleManager, false))
                modifier += GetAbility()
                   .GetMovePriorityModifier(move, this, targets, currentPriority, battleManager, showNotifications);

            return modifier;
        }

        /// <summary>
        /// Is this monster immune to the given move for reasons other than those moves immunities?
        /// Example: Immunity to sound moves.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <param name="userType">User of that move.</param>
        /// <param name="userIndex">User of that move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if immune.</returns>
        public bool IsImmuneToMove(Move move,
                                   BattlerType userType,
                                   int userIndex,
                                   bool effectIgnoresAbilities,
                                   BattleManager battleManager)
        {
            bool immune = false;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                immune |= GetAbility().IsImmuneToMove(this, move, userType, userIndex, battleManager);

            return immune;
        }

        /// <summary>
        /// Calculates the effectiveness of a move used against this battler.
        /// </summary>
        /// <param name="user">The user of the move.</param>
        /// <param name="move">The move to check.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications when calculating?</param>
        /// <param name="effectiveness">The effectiveness value.</param>
        /// <returns>If the move has effectiveness at all.</returns>
        public bool GetEffectivenessOfMove(Battler user,
                                           Move move,
                                           bool ignoresAbilities,
                                           BattleManager battleManager,
                                           bool showNotifications,
                                           out float effectiveness)
        {
            if (move != null && move.IsAffectedByTypeEffectiveness)
            {
                effectiveness =
                    GetEffectivenessOfType(move.GetMoveTypeInBattle(user, battleManager),
                                           battleManager,
                                           showNotifications,
                                           user,
                                           move,
                                           ignoresAbilities);

                return true;
            }

            effectiveness = 1f;
            return false;
        }

        /// <summary>
        /// Get the effectiveness of a type against this monster.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications when calculating?</param>
        /// <param name="user">User of the effect used against this monster.</param>
        /// <param name="move">Move being used. Can be null.</param>
        /// <param name="effectIgnoresAbilities">Does the effect checking this ignore abilities?</param>
        /// <returns>The effectiveness of that type against this monster.</returns>
        internal float GetEffectivenessOfType(MonsterType type,
                                              BattleManager battleManager,
                                              bool showNotifications,
                                              Battler user = null,
                                              Move move = null,
                                              bool effectIgnoresAbilities = false)
        {
            (MonsterType firstType, MonsterType secondType) = GetTypes(battleManager.YAPUSettings);

            float effectiveness = 1;

            bool moveHasOverrides = move != null && move.OverridesEffectiveness;

            if (moveHasOverrides && move.EffectivenessOverride.TryGetValue(firstType, out float newEffectiveness))
                effectiveness *= newEffectiveness;
            else
                effectiveness = GetAttackMultiplierOfTypesWhenDefending(type, firstType, battleManager);

            if (secondType != null)
            {
                if (moveHasOverrides && move.EffectivenessOverride.TryGetValue(secondType, out newEffectiveness))
                    effectiveness *= newEffectiveness;
                else
                    effectiveness *= GetAttackMultiplierOfTypesWhenDefending(type, secondType, battleManager);
            }

            if (battleManager.YAPUSettings.TypesThatCanOnlyAffectGrounded.Contains(type)
             && !IsGrounded(battleManager, effectIgnoresAbilities, true))
                effectiveness = 0;

            ModifyEffectivenessAfterTypeCalculationWhenTargeted(user,
                                                                move,
                                                                battleManager,
                                                                showNotifications,
                                                                ref effectiveness);

            user?.ModifyMultiplierOfTypesWhenAttacking(this, move, battleManager, ref effectiveness);

            return effectiveness;
        }

        /// <summary>
        /// Called to modify the effectiveness after doing the type calculation.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="move">Move used.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications when calculating?</param>
        /// <param name="effectiveness">Current effectiveness.</param>
        private void ModifyEffectivenessAfterTypeCalculationWhenTargeted(Battler user,
                                                                         Move move,
                                                                         BattleManager battleManager,
                                                                         bool showNotifications,
                                                                         ref float effectiveness)
        {
            if (CanUseAbility(battleManager, false))
                GetAbility()
                   .ModifyEffectivenessAfterTypeCalculationWhenTargeted(this,
                                                                        user,
                                                                        move,
                                                                        battleManager,
                                                                        showNotifications,
                                                                        ref effectiveness);
        }

        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>Tuple stating if the number has been modified and the new value.</returns>
        public (bool, int) GetNumberOfHitsOfMultihitMove(BattleManager battleManager,
                                                         Move move,
                                                         bool effectIgnoresAbilities,
                                                         List<(BattlerType Type, int Index)> targets)
        {
            // ReSharper disable once InvertIf
            if (CanUseAbility(battleManager, false))
            {
                (bool modified, int numberOfHits) =
                    GetAbility().GetNumberOfHitsOfMultihitMove(battleManager, this, move, targets);

                if (modified) return (true, numberOfHits);
            }

            // TODO: Item or status callbacks?

            return (false, 1);
        }

        /// <summary>
        /// Allow the user of a move to modify the effectiveness of a move when attacking.
        /// </summary>
        private void ModifyMultiplierOfTypesWhenAttacking(Battler target,
                                                          Move move,
                                                          BattleManager battleManager,
                                                          ref float multiplier)
        {
            if (CanUseAbility(battleManager, false))
                GetAbility().ModifyMultiplierOfTypesWhenAttacking(this, target, move, battleManager, ref multiplier);
        }

        /// <summary>
        /// Get the attack multiplier of two types with the user defending.
        /// </summary>
        /// <param name="attackerType">Attacking type.</param>
        /// <param name="userType">Defending type of the user.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The attack multiplier.</returns>
        private float GetAttackMultiplierOfTypesWhenDefending(MonsterType attackerType,
                                                              MonsterType userType,
                                                              BattleManager battleManager)
        {
            // ReSharper disable once InvertIf
            if (CanUseHeldItemInBattle(battleManager))
            {
                float multiplierOverride =
                    HeldItem.OnCalculateAttackMultiplierOfTypesWhenDefending(this,
                                                                             attackerType,
                                                                             userType,
                                                                             battleManager);

                if (multiplierOverride >= 0) return multiplierOverride;
            }

            return attackerType.AttackMultipliers[userType];
        }

        /// <summary>
        /// Called to check if the battler can perform the secondary effect of a move when using it.
        /// </summary>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="battleManager"></param>
        /// <returns>True if it can.</returns>
        public bool CanPerformSecondaryEffectOfMove(List<(BattlerType Type, int Index)> targets,
                                                    Move move,
                                                    BattleManager battleManager)
        {
            bool canPerform = true;

            if (CanUseAbility(battleManager, false))
                canPerform &= GetAbility().CanPerformSecondaryEffectOfMove(this, targets, move, battleManager);

            return canPerform;
        }

        /// <summary>
        /// Is the monster affected by secondary effects of damage moves?
        /// </summary>
        /// <param name="attacker">Attacking battler.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="damageDealt">Damage that the move dealt.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it is affected.</returns>
        public bool IsAffectedBySecondaryEffectsOfDamageMove(Battler attacker,
                                                             DamageMove move,
                                                             int damageDealt,
                                                             bool effectIgnoresAbilities,
                                                             BattleManager battleManager)
        {
            bool affected = CanBattle && damageDealt > 0;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                affected &= GetAbility()
                   .IsAffectedBySecondaryEffectsOfDamageMove(this,
                                                             attacker,
                                                             move,
                                                             damageDealt,
                                                             battleManager);

            return affected;
        }

        /// <summary>
        /// Get a multiplier to apply to the chance of the secondary effect of a move.
        /// Ex: Pledges or Serene Grace.
        /// </summary>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="battleManager"></param>
        /// <returns>The multiplier to apply.</returns>
        public float GetMultiplierForChanceOfSecondaryEffectOfMove(List<(BattlerType Type, int Index)> targets,
                                                                   Move move,
                                                                   BattleManager battleManager)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, false))
                multiplier *= GetAbility()
                   .GetMultiplierForChanceOfSecondaryEffectOfMove(this, targets, move, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Move's targets.</param>
        /// <param name="ignoreStatus">Does the move ignore the battler's status?</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public IEnumerator OnAboutToPerformMove(Move move,
                                                BattleManager battleManager,
                                                List<(BattlerType Type, int Index)> targets,
                                                bool ignoreStatus,
                                                bool ignoresAbilities,
                                                Action<bool> finished)
        {
            bool useMove = true;

            if (DoesWeatherHaveEffect(battleManager) && battleManager.Scenario.GetWeather(out Weather weather))
                yield return weather.OnAboutToPerformMove(this,
                                                          move,
                                                          battleManager,
                                                          targets,
                                                          ignoreStatus,
                                                          ignoresAbilities,
                                                          stillPerform => useMove &= stillPerform);

            if (!useMove)
            {
                finished.Invoke(useMove);
                yield break;
            }

            if (GetStatus() != null && !ignoreStatus)
                yield return GetStatus()
                   .OnAboutToPerformMove(this,
                                         move,
                                         battleManager,
                                         shouldUse => useMove &= shouldUse);

            if (!useMove)
            {
                finished.Invoke(useMove);
                yield break;
            }

            if (CanUseAbility(battleManager, ignoresAbilities))
                yield return GetAbility()
                   .OnAboutToPerformMove(move,
                                         this,
                                         battleManager,
                                         targets,
                                         ignoreStatus,
                                         ignoresAbilities,
                                         shouldUse => useMove &= shouldUse);

            if (!useMove)
            {
                finished.Invoke(useMove);
                yield break;
            }

            // Iterate a copy in case a callback modifies it.
            List<VolatileStatus> volatiles = VolatileStatuses.Keys.ToList();

            foreach (VolatileStatus status in volatiles)
                yield return status.OnAboutToUseAMove(this,
                                                      move,
                                                      battleManager,
                                                      targets,
                                                      shouldUse => useMove &= shouldUse);

            finished.Invoke(useMove);
        }

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has this move been reflected?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="finished">Callback stating if the move will still be used, the new targets for the move.</param>
        public IEnumerator OnOtherBattlerAboutToUseAMove(Battler user,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         bool hasBeenReflected,
                                                         bool ignoresAbilities,
                                                         Action<bool, List<(BattlerType Type, int Index)>> finished)
        {
            bool useMove = true;

            foreach (VolatileStatus status in VolatileStatuses.Keys)
                yield return status.OnOtherBattlerAboutToUseAMove(this,
                                                                  user,
                                                                  move,
                                                                  battleManager,
                                                                  targets,
                                                                  hasBeenReflected,
                                                                  (stillPerform, modifiedTargets, newTargets) =>
                                                                  {
                                                                      if (!stillPerform) useMove = false;
                                                                      if (modifiedTargets) targets = newTargets;
                                                                  });

            if (CanUseAbility(battleManager, ignoresAbilities))
                yield return GetAbility()
                   .OnOtherBattlerAboutToUseAMove(this,
                                                  user,
                                                  move,
                                                  battleManager,
                                                  targets,
                                                  hasBeenReflected,
                                                  (stillPerform, newTargets) =>
                                                  {
                                                      if (!stillPerform) useMove = false;
                                                      if (newTargets != null) targets = newTargets;
                                                  });

            finished.Invoke(useMove, targets);
        }

        /// <summary>
        /// Called before being hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">States true if it will still hit.</param>
        /// <param name="bypassSubstitute">Does the move bypass the substitute?</param>
        /// <param name="didShowUsedMessageNormally">Was the move used message normally displayed?</param>
        public IEnumerator AboutToBeHitByMove(Move move,
                                              Battler user,
                                              BattleManager battleManager,
                                              bool didShowUsedMessageNormally,
                                              Action<bool> callback,
                                              bool bypassSubstitute = false)
        {
            (BattlerType thisType, int thisIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(this);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            if (user.CanUseAbility(battleManager, false)
             && user.GetAbility().ByPassesSubstitute(thisType, thisIndex, battleManager, userType, userIndex, move))
                bypassSubstitute = true;

            bool notFailed = true;

            if (battleManager.Scenario.Terrain != null
             && battleManager.Scenario.Terrain.IsAffected(this, battleManager))
                yield return battleManager.Scenario.Terrain.OnAboutToBeHitByMove(this,
                    move,
                    battleManager,
                    user,
                    didShowUsedMessageNormally,
                    stillPerform => notFailed &= stillPerform);

            if (!notFailed)
            {
                callback.Invoke(notFailed);
                yield break;
            }

            foreach (SideStatus status in battleManager.Statuses.GetSideStatuses(thisType).Select(slot => slot.Key))
                yield return status.OnAboutToBeHitByMove(this,
                                                         move,
                                                         battleManager,
                                                         user,
                                                         didShowUsedMessageNormally,
                                                         stillPerform => notFailed &= stillPerform);

            if (!notFailed)
            {
                callback.Invoke(notFailed);
                yield break;
            }

            if (bypassSubstitute || !Substitute.SubstituteEnabled)
            {
                foreach (KeyValuePair<VolatileStatus, int> statusSlot in VolatileStatuses)
                    yield return statusSlot.Key.OnAboutToBeHitByMove(this,
                                                                     move,
                                                                     battleManager,
                                                                     user,
                                                                     didShowUsedMessageNormally,
                                                                     stillPerform => notFailed &= stillPerform);

                if (GetStatus() != null)
                    yield return GetStatus()
                       .AboutToBeHitByMove(move, this, user, battleManager, didShowUsedMessageNormally);
            }

            callback.Invoke(notFailed);
        }

        /// <summary>
        /// Called when a move that targets this battler is about to execute its effect.
        /// Abilities like Lightning Rod or Flash Fire can replace this effect for something else
        /// and prevent the original effect from executing.
        /// </summary>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public IEnumerator ShouldReplaceMoveEffectWhenHit(Move move,
                                                          Battler user,
                                                          bool effectIgnoresAbilities,
                                                          BattleManager battleManager,
                                                          Action<bool> callback)
        {
            bool stillExecute = true;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .ShouldReplaceMoveEffectWhenHit(this,
                                                   move,
                                                   user,
                                                   battleManager,
                                                   stillPerform => stillExecute &= stillPerform);

            callback.Invoke(stillExecute);
        }

        /// <summary>
        /// Called when the battler is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="bypassSubstitute">Does the move bypass the substitute?</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="finished">Callback stating the new effectiveness if it will force survive.</param>
        public IEnumerator OnHitByMove(DamageMove move,
                                       float effectiveness,
                                       bool bypassSubstitute,
                                       bool effectIgnoresAbilities,
                                       BattleManager battleManager,
                                       Battler moveUser,
                                       Action<float, bool> finished)
        {
            bool forceSurvive = false;

            if (bypassSubstitute || !Substitute.SubstituteEnabled)
            {
                if (CanUseAbility(battleManager, effectIgnoresAbilities))
                    yield return GetAbility()
                       .OnHitByMove(move,
                                    effectiveness,
                                    this,
                                    battleManager,
                                    moveUser,
                                    newEffectiveness =>
                                    {
                                        effectiveness = newEffectiveness;
                                    });

                if (CanUseHeldItemInBattle(battleManager))
                {
                    bool consumeItemOfTarget = false;

                    yield return HeldItem.OnHitByMove(move,
                                                      effectiveness,
                                                      this,
                                                      battleManager,
                                                      moveUser,
                                                      battleManager.Localizer,
                                                      (shouldConsume, newEffectiveness) =>
                                                      {
                                                          if (shouldConsume) consumeItemOfTarget = true;

                                                          effectiveness = newEffectiveness;
                                                      });

                    if (consumeItemOfTarget) yield return ConsumeItemInBattle(battleManager);
                }

                foreach (KeyValuePair<VolatileStatus, int> slot in VolatileStatuses)
                    yield return slot.Key.OnHitByMove(move,
                                                      effectiveness,
                                                      this,
                                                      battleManager,
                                                      moveUser,
                                                      effectIgnoresAbilities,
                                                      (multiplier, willForceSurvive) =>
                                                      {
                                                          effectiveness *= multiplier;

                                                          if (willForceSurvive) forceSurvive = true;
                                                      });
            }

            finished.Invoke(effectiveness, forceSurvive);
        }

        /// <summary>
        /// Called after the battler is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="damageDealt">Damage dealt by the move.</param>
        /// <param name="previousHP">HP it had before being it.</param>
        /// <param name="wasCritical">Was it a critical hit?</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public IEnumerator AfterHitByMove(DamageMove move,
                                          float effectiveness,
                                          Battler user,
                                          int damageDealt,
                                          uint previousHP,
                                          bool wasCritical,
                                          bool substituteTookHit,
                                          bool effectIgnoresAbilities,
                                          int hitNumber,
                                          int expectedMoveHits,
                                          BattleManager battleManager,
                                          ILocalizer localizer)
        {
            if (GetStatus() != null)
                yield return GetStatus()
                   .AfterHitByMove(move,
                                   effectiveness,
                                   this,
                                   user,
                                   battleManager);

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .AfterHitByMove(move,
                                   effectiveness,
                                   this,
                                   user,
                                   damageDealt,
                                   previousHP,
                                   wasCritical,
                                   substituteTookHit,
                                   effectIgnoresAbilities,
                                   hitNumber,
                                   expectedMoveHits,
                                   battleManager);

            if (!CanUseHeldItemInBattle(battleManager)) yield break;

            bool consumeItem = false;

            yield return HeldItem.AfterHitByMove(move,
                                                 effectiveness,
                                                 this,
                                                 user,
                                                 substituteTookHit,
                                                 effectIgnoresAbilities,
                                                 battleManager,
                                                 localizer,
                                                 shouldConsume =>
                                                 {
                                                     if (shouldConsume) consumeItem = true;
                                                 });

            if (consumeItem) yield return ConsumeItemInBattle(battleManager);
        }

        /// <summary>
        /// Called after the battler is hit by a multihit move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoreAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public IEnumerator AfterHitByMultihitMove(DamageMove move,
                                                  float effectiveness,
                                                  Battler user,
                                                  bool substituteTookHit,
                                                  bool ignoreAbilities,
                                                  BattleManager battleManager,
                                                  ILocalizer localizer)
        {
            if (!CanUseHeldItemInBattle(battleManager)) yield break;

            bool consumeItem = false;

            yield return HeldItem.AfterHitByMultihitMove(move,
                                                         effectiveness,
                                                         this,
                                                         user,
                                                         substituteTookHit,
                                                         ignoreAbilities,
                                                         battleManager,
                                                         localizer,
                                                         shouldConsume =>
                                                         {
                                                             if (shouldConsume) consumeItem = true;
                                                         });

            if (consumeItem) yield return ConsumeItemInBattle(battleManager);
        }

        /// <summary>
        /// Called each time the battler lands a hit with a move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="targets">The move's targets.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public IEnumerator AfterHittingWithMove(Move move,
                                                List<(BattlerType Type, int Index)> targets,
                                                BattleManager battleManager,
                                                ILocalizer localizer)
        {
            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consumeItem = false;

                yield return HeldItem.AfterHittingWithMove(move,
                                                           this,
                                                           targets,
                                                           battleManager,
                                                           localizer,
                                                           shouldConsume =>
                                                           {
                                                               if (shouldConsume) consumeItem = true;
                                                           });

                if (consumeItem) yield return ConsumeItemInBattle(battleManager);
            }

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().AfterHittingWithMove(move, this, targets, battleManager);
        }

        /// <summary>
        /// Called after the battler uses a move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="targets">The move's targets.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public IEnumerator AfterUsingMove(Move move,
                                          List<(BattlerType Type, int Index)> targets,
                                          BattleManager battleManager,
                                          ILocalizer localizer)
        {
            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consumeItem = false;

                yield return HeldItem.AfterUsingMove(move,
                                                     this,
                                                     targets,
                                                     battleManager,
                                                     localizer,
                                                     shouldConsume =>
                                                     {
                                                         if (shouldConsume) consumeItem = true;
                                                     });

                if (consumeItem) yield return ConsumeItemInBattle(battleManager);
            }

            if (CanUseAbility(battleManager, false))
                yield return GetAbility().AfterUsingMove(move, this, targets, battleManager);
        }

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="ignoreAbilities"></param>
        /// <returns>A multiplier to apply to the power.</returns>
        public float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                         Move move,
                                                         Battler target,
                                                         bool ignoreAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, false))
                multiplier *= GetAbility()
                   .GetMovePowerMultiplierWhenUsingMove(battleManager, move, this, target, ignoreAbilities);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                multiplier *= status.GetMovePowerMultiplierWhenUsingMove(this, move, target, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="finished">Callback stating
        /// Was the chance modified?
        /// Multiplier to apply to the chance.
        /// Critical stage modifier to use.
        /// Change it to always hit?
        /// </param>
        /// <returns>Has the chance been changed?</returns>
        public IEnumerator OnCalculateCriticalChance(Battler target,
                                                     BattleManager battleManager,
                                                     Move move,
                                                     Action<bool, float, byte, bool> finished)
        {
            bool wasModified = false;

            float multiplier = 1;
            byte modifier = 0;
            bool alwaysHit = false;

            if (CanUseAbility(battleManager, false))
                wasModified |= GetAbility()
                   .OnCalculateCriticalChance(this,
                                              target,
                                              battleManager,
                                              move,
                                              ref multiplier,
                                              ref modifier,
                                              ref alwaysHit);

            wasModified = VolatileStatuses.Aggregate(wasModified,
                                                     (current, status) => current
                                                                        | status.Key.OnCalculateCriticalChance(this,
                                                                              target,
                                                                              battleManager,
                                                                              move,
                                                                              ref multiplier,
                                                                              ref alwaysHit));

            if (CanUseHeldItemInBattle(battleManager))
            {
                wasModified |= HeldItem.OnCalculateCriticalChance(this,
                                                                  target,
                                                                  battleManager,
                                                                  move,
                                                                  ref multiplier,
                                                                  ref modifier,
                                                                  ref alwaysHit,
                                                                  out bool consume);

                if (consume) yield return ConsumeItemInBattle(battleManager);
            }

            finished.Invoke(wasModified, multiplier, modifier, alwaysHit);
        }

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Multiplier to apply to the chance.</returns>
        public float OnCalculateCriticalChanceWhenTargeted(Battler user,
                                                           BattleManager battleManager,
                                                           Move move,
                                                           bool effectIgnoresAbilities,
                                                           out bool alwaysHit)
        {
            float multiplier = 1;
            alwaysHit = false;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility()
                   .OnCalculateCriticalChanceWhenTargeted(this, user, battleManager, move, out alwaysHit);

            return multiplier;
        }

        /// <summary>
        /// Get the power of a move that is going to hit this battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="bypassSubstitute">Does this move bypass the substitute?</param>
        /// <returns>A multiplier to apply to the power.</returns>
        public float GetMovePowerMultiplierWhenHit(BattleManager battleManager,
                                                   Move move,
                                                   Battler user,
                                                   bool effectIgnoresAbilities,
                                                   bool bypassSubstitute)
        {
            (BattlerType thisType, int thisIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(this);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            if (user.CanUseAbility(battleManager, effectIgnoresAbilities)
             && user.GetAbility().ByPassesSubstitute(thisType, thisIndex, battleManager, userType, userIndex, move))
                bypassSubstitute = true;

            float multiplier = 1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            // ReSharper disable once InvertIf
            if (bypassSubstitute || !Substitute.SubstituteEnabled)
                foreach (KeyValuePair<VolatileStatus, int> status in VolatileStatuses)
                    multiplier *= status.Key.GetMovePowerMultiplierWhenHit(battleManager, move, user, this);

            return multiplier;
        }

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                       Battler target,
                                                       Move move,
                                                       bool ignoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, false))
                multiplier *= GetAbility()
                   .GetMoveAccuracyMultiplierWhenUsed(battleManager, this, target, move, ignoresAbilities);

            if (CanUseHeldItemInBattle(battleManager))
                multiplier *= HeldItem.OnCalculateAccuracyWhenUsed(move, this, target, battleManager);

            foreach (GlobalStatus status in battleManager.Statuses.GetGlobalStatuses().Select(slot => slot.Key))
                multiplier *= status.GetMoveAccuracyMultiplierWhenUsed(battleManager, this, target, move);

            return multiplier;
        }

        /// <summary>
        /// Get the multiplier to apply to the accuracy when being the target of a move.
        /// </summary>
        public float GetMoveAccuracyMultiplierWhenTargeted(BattleManager battleManager,
                                                           Battler user,
                                                           Move move,
                                                           bool effectIgnoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility().GetMoveAccuracyMultiplierWhenTargeted(this, battleManager, user, move);
            // TODO: Item.

            return multiplier;
        }

        /// <summary>
        /// Does this ability bypass all accuracy checks when using a move?
        /// </summary>
        public bool DoesBypassAllAccuracyChecksWhenUsing(Move move,
                                                         Battler target,
                                                         BattleManager battleManager)
        {
            bool bypass = false;

            if (CanUseAbility(battleManager, false))
                bypass |= GetAbility().DoesBypassAllAccuracyChecksWhenUsing(this, move, target, battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                bypass |= status.DoesBypassAllAccuracyChecksWhenUsing(this, move, target, battleManager);

            return bypass;
        }

        /// <summary>
        /// Does this ability bypass all accuracy checks when targeted by a move?
        /// </summary>
        public bool DoesBypassAllAccuracyChecksWhenTargeted(Move move,
                                                            Battler user,
                                                            bool effectIgnoresAbilities,
                                                            BattleManager battleManager)
        {
            bool bypass = false;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                bypass |= GetAbility().DoesBypassAllAccuracyChecksWhenTargeted(this, move, user, battleManager);

            foreach (VolatileStatus status in VolatileStatuses.Select(slot => slot.Key))
                bypass |= status.DoesBypassAllAccuracyChecksWhenTargeted(this, move, user, battleManager);

            return bypass;
        }

        /// <summary>
        /// Called when calculating a move's stab damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness"></param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public IEnumerator OnCalculateStabDamageWhenUsing(DamageMove move,
                                                          float multiplier,
                                                          Battler target,
                                                          float effectiveness,
                                                          bool isCritical,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Action<float> finished)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility()
                   .OnCalculateStabDamageWhenUsing(move,
                                                   multiplier,
                                                   this,
                                                   target,
                                                   effectiveness,
                                                   isCritical,
                                                   battleManager,
                                                   localizer,
                                                   newMultiplier => multiplier = newMultiplier);

            // TODO: Item callbacks.

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness"></param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="hitNumber">Number of the current hit.</param>
        /// <param name="expectedHitNumber">Expected number of hits.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public IEnumerator OnCalculateMoveDamageWhenUsing(DamageMove move,
                                                          float multiplier,
                                                          Battler target,
                                                          float effectiveness,
                                                          bool isCritical,
                                                          int hitNumber,
                                                          int expectedHitNumber,
                                                          bool ignoresAbilities,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          List<(BattlerType Type, int Index)> allTargets,
                                                          Action<float> finished)
        {
            if (CanUseAbility(battleManager, false))
                yield return GetAbility()
                   .OnCalculateMoveDamageWhenUsing(move,
                                                   multiplier,
                                                   this,
                                                   target,
                                                   effectiveness,
                                                   isCritical,
                                                   hitNumber,
                                                   expectedHitNumber,
                                                   ignoresAbilities,
                                                   allTargets,
                                                   battleManager,
                                                   localizer,
                                                   newMultiplier => multiplier = newMultiplier);

            if (CanUseHeldItemInBattle(battleManager))
            {
                bool consumeItem = false;

                yield return HeldItem.OnCalculateMoveDamageWhenUsing(move,
                                                                     multiplier,
                                                                     this,
                                                                     target,
                                                                     battleManager,
                                                                     localizer,
                                                                     (shouldConsume, newMultiplier) =>
                                                                     {
                                                                         if (shouldConsume) consumeItem = true;
                                                                         multiplier = newMultiplier;
                                                                     });

                if (consumeItem) yield return ConsumeItemInBattle(battleManager);
            }

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Called when calculating a move's damage when an ally uses it.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Ally using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness"></param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public IEnumerator OnCalculateMoveDamageWhenAllyUsing(DamageMove move,
                                                              float multiplier,
                                                              Battler user,
                                                              Battler target,
                                                              float effectiveness,
                                                              bool isCritical,
                                                              bool effectIgnoresAbilities,
                                                              BattleManager battleManager,
                                                              ILocalizer localizer,
                                                              Action<float> finished)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .OnCalculateMoveDamageWhenAllyUsing(this,
                                                       move,
                                                       multiplier,
                                                       this,
                                                       target,
                                                       effectiveness,
                                                       isCritical,
                                                       battleManager,
                                                       localizer,
                                                       newMultiplier => multiplier = newMultiplier);

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Called when calculating a move's damage on itself.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                             float multiplier,
                                                             Battler user,
                                                             bool effectIgnoresAbilities,
                                                             BattleManager battleManager,
                                                             Action<float> finished)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .OnCalculateMoveDamageWhenTargeted(move,
                                                      multiplier,
                                                      user,
                                                      this,
                                                      battleManager,
                                                      newMultiplier => multiplier = newMultiplier);

            // TODO: Items.

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Called when calculating a move's damage on an ally.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Ally target of the move.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public IEnumerator OnCalculateMoveDamageWhenAllyTargeted(DamageMove move,
                                                                 float multiplier,
                                                                 Battler user,
                                                                 Battler target,
                                                                 bool effectIgnoresAbilities,
                                                                 BattleManager battleManager,
                                                                 Action<float> finished)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .OnCalculateMoveDamageWhenAllyTargeted(this,
                                                          move,
                                                          multiplier,
                                                          user,
                                                          target,
                                                          battleManager,
                                                          newMultiplier => multiplier = newMultiplier);

            // TODO: Items.

            finished.Invoke(multiplier);
        }

        /// <summary>
        /// Is this monster currently affect by burn damage reduction?
        /// </summary>
        public bool IsAffectedByBurnDamageReduction(BattleManager battleManager)
        {
            bool affected = true;

            if (CanUseAbility(battleManager, false))
                affected &= GetAbility().IsAffectedByBurnDamageReduction(this, battleManager);

            return affected;
        }

        /// <summary>
        /// Is this monster currently affect by paralysis speed reduction?
        /// </summary>
        public bool IsAffectedByParalysisSpeedReduction(BattleManager battleManager)
        {
            bool affected = true;

            if (CanUseAbility(battleManager, false))
                affected &= GetAbility().IsAffectedByParalysisSpeedReduction(this, battleManager);

            return affected;
        }

        #endregion

        #region Statuses

        ///  <summary>
        ///  Check if a status can be added to the monster.
        ///  </summary>
        ///  <param name="status">Status to add.</param>
        ///  <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        ///  <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        ///  <param name="takeCurrentStatusIntoAccount">Take the current status into account?</param>
        ///  <param name="callback">Callback telling if it can be added</param>
        ///  <param name="bypassSubstitute">Does this effect bypass the substitute?</param>
        public IEnumerator CanAddStatus(Status status,
                                        BattleManager battleManager,
                                        BattlerType userType,
                                        int userIndex,
                                        bool effectIgnoresAbilities,
                                        bool takeCurrentStatusIntoAccount,
                                        Action<bool> callback,
                                        bool bypassSubstitute = false)
        {
            (BattlerType thisType, int thisIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(this);
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, false)
             && user.GetAbility().ByPassesSubstitute(thisType, thisIndex, battleManager, userType, userIndex))
                bypassSubstitute = true;

            bool canAdd = status.CanAddStatus(this,
                                              battleManager,
                                              user,
                                              effectIgnoresAbilities,
                                              takeCurrentStatusIntoAccount);

            if (!canAdd)
            {
                callback.Invoke(false);
                yield break;
            }

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .CanAddStatus(status,
                                 thisType,
                                 thisIndex,
                                 battleManager,
                                 userType,
                                 userIndex,
                                 // ReSharper disable once AccessToModifiedClosure
                                 add => canAdd &= add);

            if (!canAdd)
            {
                callback.Invoke(false);
                yield break;
            }

            canAdd &= bypassSubstitute || !Substitute.SubstituteEnabled || thisType == userType;

            if (!canAdd)
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: GetNameOrNickName(battleManager.Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

            foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(thisType))
                yield return statusSlot.Key.CanAddStatus(status,
                                                         thisType,
                                                         thisIndex,
                                                         battleManager,
                                                         userType,
                                                         userIndex,
                                                         add => canAdd &= add);

            if (battleManager.Scenario.Terrain != null
             && battleManager.Scenario.Terrain.IsAffected(this, battleManager))
                yield return battleManager.Scenario.Terrain.CanAddStatus(status,
                                                                         thisType,
                                                                         thisIndex,
                                                                         battleManager,
                                                                         userType,
                                                                         userIndex,
                                                                         add => canAdd &= add);

            foreach (Battler battler in battleManager.Battlers.GetBattlersFighting())
                yield return battler.CanAnyMonsterAddStatus(status,
                                                            thisType,
                                                            thisIndex,
                                                            battleManager,
                                                            userType,
                                                            userIndex,
                                                            effectIgnoresAbilities,
                                                            add => canAdd &= add);

            callback.Invoke(canAdd);
        }

        ///  <summary>
        ///  Check if a status can be added to the monster.
        ///  </summary>
        ///  <param name="status">Status to add.</param>
        ///  <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        ///  <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        ///  <param name="callback">Callback telling if it can be added</param>
        ///  <param name="bypassSubstitute">Does the effect bypass the substitute?</param>
        public IEnumerator CanAddStatus(VolatileStatus status,
                                        BattleManager battleManager,
                                        BattlerType userType,
                                        int userIndex,
                                        bool effectIgnoresAbilities,
                                        Action<bool> callback,
                                        bool bypassSubstitute = false)

        {
            (BattlerType thisType, int thisIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(this);
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, false)
             && user.GetAbility().ByPassesSubstitute(thisType, thisIndex, battleManager, userType, userIndex))
                bypassSubstitute = true;

            bool canAdd = bypassSubstitute || !Substitute.SubstituteEnabled || thisType == userType;

            if (!canAdd)
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: GetNameOrNickName(battleManager.Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .CanAddStatus(status,
                                 thisType,
                                 thisIndex,
                                 battleManager,
                                 userType,
                                 userIndex,
                                 add => canAdd &= add);

            foreach (KeyValuePair<SideStatus, int> statusSlots in battleManager.Statuses.GetSideStatuses(thisType))
                yield return statusSlots.Key.CanAddStatus(status,
                                                          thisType,
                                                          thisIndex,
                                                          battleManager,
                                                          userType,
                                                          userIndex,
                                                          add => canAdd &= add);

            if (battleManager.Scenario.Terrain != null
             && battleManager.Scenario.Terrain.IsAffected(this, battleManager))
                yield return battleManager.Scenario.Terrain.CanAddStatus(status,
                                                                         thisType,
                                                                         thisIndex,
                                                                         battleManager,
                                                                         userType,
                                                                         userIndex,
                                                                         add => canAdd &= add);

            foreach (Battler battler in battleManager.Battlers.GetBattlersFighting())
                yield return battler.CanAnyMonsterAddStatus(status,
                                                            thisType,
                                                            thisIndex,
                                                            battleManager,
                                                            userType,
                                                            userIndex,
                                                            effectIgnoresAbilities,
                                                            add => canAdd &= add);

            callback.Invoke(canAdd);
        }

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAnyMonsterAddStatus(Status status,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          bool effectIgnoresAbilities,
                                                          Action<bool> callback)
        {
            bool canAdd = true;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .CanAnyMonsterAddStatus(status,
                                           this,
                                           targetType,
                                           targetIndex,
                                           battleManager,
                                           userType,
                                           userIndex,
                                           add => canAdd &= add);

            if (!canAdd)
            {
                callback.Invoke(canAdd);
                yield break;
            }

            foreach (KeyValuePair<VolatileStatus, int> statusSlot in VolatileStatuses)
                yield return statusSlot.Key.CanAnyMonsterAddStatus(status,
                                                                   this,
                                                                   targetType,
                                                                   targetIndex,
                                                                   battleManager,
                                                                   userType,
                                                                   userIndex,
                                                                   add => canAdd &= add);

            callback.Invoke(canAdd);
        }

        /// <summary>
        /// Check if a status can be added to any monster on the battlefield.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAnyMonsterAddStatus(VolatileStatus status,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          bool effectIgnoresAbilities,
                                                          Action<bool> callback)
        {
            bool canAdd = true;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility()
                   .CanAnyMonsterAddStatus(status,
                                           this,
                                           targetType,
                                           targetIndex,
                                           battleManager,
                                           userType,
                                           userIndex,
                                           add => canAdd &= add);

            if (!canAdd)
            {
                callback.Invoke(canAdd);
                yield break;
            }

            foreach (KeyValuePair<VolatileStatus, int> statusSlot in VolatileStatuses)
                yield return statusSlot.Key.CanAnyMonsterAddStatus(status,
                                                                   this,
                                                                   targetType,
                                                                   targetIndex,
                                                                   battleManager,
                                                                   userType,
                                                                   userIndex,
                                                                   add => canAdd &= add);

            callback.Invoke(canAdd);
        }

        ///  <summary>
        ///  Add a status to this monster.
        ///  </summary>
        ///  <param name="status">Status to add.</param>
        ///  <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        ///  <param name="userIndex">The index of the monster that triggered this change.</param>
        ///  <param name="effectIgnoresAbilities">Does the effect ignore abilities?</param>
        ///  <param name="takeCurrentStatusIntoAccount">Take the current status into account?</param>
        ///  <param name="showMessage">Show a message when adding?</param>
        ///  <param name="finished">Callback telling if it was added.</param>
        ///  <param name="extraData">Extra data for the status.</param>
        public IEnumerator AddStatusInBattle(Status status,
                                             BattleManager battleManager,
                                             BattlerType userType,
                                             int userIndex,
                                             bool effectIgnoresAbilities,
                                             bool takeCurrentStatusIntoAccount,
                                             bool showMessage,
                                             Action<bool> finished = null,
                                             params object[] extraData)
        {
            bool canAdd = false;

            yield return CanAddStatus(status,
                                      battleManager,
                                      userType,
                                      userIndex,
                                      effectIgnoresAbilities,
                                      takeCurrentStatusIntoAccount,
                                      add => canAdd = add);

            if (canAdd)
            {
                if (Status != null) yield return status.OnStatusRemovedInBattle(this, battleManager, false);

                Status = status;

                yield return status.OnStatusAddedInBattle(this,
                                                          battleManager,
                                                          effectIgnoresAbilities,
                                                          showMessage,
                                                          extraData);

                if (CanUseAbility(battleManager, effectIgnoresAbilities))
                    yield return GetAbility().OnStatusAddedInBattle(this, userType, userIndex, battleManager);

                finished?.Invoke(true);
            }
            else
                finished?.Invoke(false);
        }

        /// <summary>
        /// Add a status to this monster.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Show a message when adding?</param>
        public IEnumerator StatusTick(BattleManager battleManager, bool showMessage = true)
        {
            if (Status != null) yield return Status.OnStatusTickInBattle(this, battleManager, showMessage);
        }

        /// <summary>
        /// Remove the monster's status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Show a message when removing?</param>
        public IEnumerator RemoveStatusInBattle(BattleManager battleManager, bool showMessage = true)
        {
            if (Status == null) yield break;

            yield return Status.OnStatusRemovedInBattle(this, battleManager, showMessage);
            Status = null;
        }

        /// <summary>
        /// Called to modify the sleep countdown when added to this monter.
        /// </summary>
        /// <returns>Multiplier to apply to the sleep.</returns>
        public float CalculateSleepCountDownMultiplier(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                multiplier *= GetAbility().CalculateSleepCountDownMultiplier(this, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Add a volatile status to this battler.
        /// </summary>
        /// <param name="status">The status to add.</param>
        /// <param name="countDown">The status countdown.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="ignoreAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="extraData">Extra data the status may need.</param>
        public IEnumerator AddVolatileStatus(VolatileStatus status,
                                             int countDown,
                                             BattleManager battleManager,
                                             BattlerType userType,
                                             int userIndex,
                                             bool ignoreAbilities,
                                             params object[] extraData)
        {
            bool canAdd = true;

            yield return CanAddStatus(status, battleManager, userType, userIndex, ignoreAbilities, add => canAdd = add);

            if (!canAdd) yield break;

            VolatileStatuses[status] = countDown;

            yield return status.OnAddStatus(battleManager, this, extraData);

            if (!CanUseHeldItemInBattle(battleManager)) yield break;

            bool consumeItem = false;

            yield return HeldItem.OnVolatileStatusAdded(battleManager,
                                                        this,
                                                        userType,
                                                        userIndex,
                                                        status,
                                                        countDown,
                                                        shouldConsume =>
                                                        {
                                                            if (shouldConsume) consumeItem = true;
                                                        });

            if (consumeItem) yield return ConsumeItemInBattle(battleManager);
        }

        /// <summary>
        /// Check if the battler has a volatile status.
        /// </summary>
        /// <param name="status">Status to check.</param>
        /// <returns>True if it has it.</returns>
        public bool HasVolatileStatus(VolatileStatus status) => VolatileStatuses.Any(pair => pair.Key == status);

        /// <summary>
        /// Remove a volatile status.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        internal IEnumerator RemoveVolatileStatus(VolatileStatus status,
                                                  BattleManager battleManager)
        {
            if (!VolatileStatuses.ContainsKey(status)) yield break;

            yield return status.OnRemoveStatus(battleManager, this);

            VolatileStatuses.Remove(status);
        }

        /// <summary>
        /// Remove all the volatile statuses of this battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator RemoveAllVolatileStatusWithoutAnimation(BattleManager battleManager)
        {
            Queue<(VolatileStatus, int)> toKeep = new();

            foreach (KeyValuePair<VolatileStatus, int> pair in VolatileStatuses)
            {
                yield return pair.Key.OnRemoveStatus(battleManager, this, false);

                if (pair.Key.PersistsWhenSwitching) toKeep.Enqueue((pair.Key, pair.Value));
            }

            VolatileStatuses.Clear();

            while (toKeep.TryDequeue(out (VolatileStatus Status, int Timeout) pair))
                VolatileStatuses[pair.Status] = pair.Timeout;
        }

        /// <summary>
        /// Trigger the countdown of statuses and remove them if the count down is finished.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public IEnumerator TriggerVolatileStatusesCountdown(BattleManager battleManager)
        {
            Dictionary<VolatileStatus, int> tickedStatuses = new();

            foreach (KeyValuePair<VolatileStatus, int> slot in VolatileStatuses)
            {
                int newValue = -1;

                if (slot.Value != -1) newValue = slot.Value - 1;

                if (newValue != 0)
                {
                    tickedStatuses[slot.Key] = newValue;
                    yield return slot.Key.OnTickStatus(battleManager, this);
                }
                else
                    yield return slot.Key.OnRemoveStatus(battleManager, this);
            }

            VolatileStatuses = tickedStatuses;
        }

        /// <summary>
        /// Calculate the random countdown of a volatile status to inflict on a target.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="status">Status to inflict.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateRandomCountdownOfVolatileStatus(BattleManager battleManager,
                                                            VolatileStatus status,
                                                            BattlerType targetType,
                                                            int targetIndex)
        {
            // The first effect that modifies it will be the one used (for now only items).

            // ReSharper disable once InvertIf
            if (CanUseHeldItemInBattle(battleManager))
            {
                int duration =
                    HeldItem.CalculateRandomCountdownOfVolatileStatus(battleManager,
                                                                      status,
                                                                      this,
                                                                      targetType,
                                                                      targetIndex);

                if (duration != -2) return duration;
            }

            return -2;
        }

        /// <summary>
        /// Calculate the duration of a new side status effect created by the holder.
        /// </summary>
        /// <param name="statusToAdd">Status to add.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="inBattleIndex">In battle index of the affected roster. Only used for dialogs.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateSideStatusDuration(SideStatus statusToAdd,
                                               BattlerType side,
                                               int inBattleIndex,
                                               BattleManager battleManager)
        {
            // The first effect that modifies it will be the one used (for now only items).

            // ReSharper disable once InvertIf
            if (CanUseHeldItemInBattle(battleManager))
            {
                int duration =
                    HeldItem.CalculateSideStatusDuration(statusToAdd, side, inBattleIndex, this, battleManager);

                if (duration != -2) return duration;
            }

            return -2;
        }

        /// <summary>
        /// Calculate the multiplier to apply when this mon takes poison damage.
        /// </summary>
        public float CalculatePoisonDamageMultiplier(BattleManager battleManager)
        {
            float multiplier = 1;

            if (CanUseAbility(battleManager, false))
                multiplier *= GetAbility().CalculatePoisonDamageMultiplier(this, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Called when the monster flinches.
        /// </summary>
        public IEnumerator OnFlinched(BattleManager battleManager, bool effectIgnoresAbilities)
        {
            if (CanUseAbility(battleManager, effectIgnoresAbilities))
                yield return GetAbility().OnFlinched(this, battleManager);
        }

        #endregion

        #region Scenario

        /// <summary>
        /// Does the weather have effect?
        /// </summary>
        public bool DoesWeatherHaveEffect(BattleManager battleManager)
        {
            bool hasEffect = true;

            if (CanUseAbility(battleManager, false))
                hasEffect &= GetAbility().DoesWeatherHaveEffect(this, battleManager);

            return hasEffect;
        }

        /// <summary>
        /// Called when a terrain is set on the battlefield.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="terrain">Terrain that has been set.</param>
        public IEnumerator OnTerrainSet(BattleManager battleManager, Terrain terrain)
        {
            if (!CanUseHeldItemInBattle(battleManager)) yield break;

            bool consumeItem = false;

            yield return HeldItem.OnTerrainSet(battleManager,
                                               terrain,
                                               this,
                                               shouldConsume =>
                                               {
                                                   if (shouldConsume) consumeItem = true;
                                               });

            if (consumeItem) yield return ConsumeItemInBattle(battleManager);
        }

        #endregion
    }
}