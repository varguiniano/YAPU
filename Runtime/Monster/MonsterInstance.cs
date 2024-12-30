using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
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
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using Direction = Varguiniano.YAPU.Runtime.Characters.CharacterController.Direction;
using Move = Varguiniano.YAPU.Runtime.MonsterDatabase.Moves.Move;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Class that stores all the data related to a monster instance.
    /// </summary>
    [Serializable]
    public class MonsterInstance : Loggable<MonsterInstance>, IWorldDataContainer, ICommandParameter
    {
        /// <summary>
        /// Species of this monster.
        /// </summary>
        [DisableIf(nameof(IsNullEntry))]
        [InfoBox("Don't try to set these values, use a new MonsterInstance() call from code.\n(If this object was meant to add more monsters it will have a button for it.)",
                 InfoMessageType.Warning,
                 nameof(IsNullEntry))]
        public MonsterEntry Species;

        /// <summary>
        /// Form of this monster.
        /// </summary>
        [DisableIf(nameof(IsNullEntry))]
        public Form Form;

        /// <summary>
        /// Is this entry null?
        /// This is useful since Unity creates an instance with everything set to null when serializing.
        /// </summary>
        public bool IsNullEntry => Species == null || Form == null;

        /// <summary>
        /// Get the form data of this monster species.
        /// </summary>
        public DataByFormEntry FormData => Species[Form];

        /// <summary>
        /// Origin data of this monster.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public OriginData OriginData;

        /// <summary>
        /// Physical data for this monster.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public MonsterPhysicalData PhysicalData;

        /// <summary>
        /// Stat data for this monster.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public MonsterStatData StatData;

        /// <summary>
        /// This monster's ability.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        [SerializeField]
        protected Ability Ability;

        /// <summary>
        /// Does it have a nickname?
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public bool HasNickname;

        /// <summary>
        /// Nickname, if it has one.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public string Nickname;

        /// <summary>
        /// Current trainer.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public string CurrentTrainer;

        /// <summary>
        /// This monster's friendship.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public byte Friendship;

        /// <summary>
        /// This monster's HP.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public uint CurrentHP;

        /// <summary>
        /// Monster's status.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        [SerializeField]
        protected Status Status;

        /// <summary>
        /// The monster's moves.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public MoveSlot[] CurrentMoves;

        /// <summary>
        /// Moves this monster has learnt in its life.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public List<Move> LearntMoves;

        /// <summary>
        /// Conditions this monster has.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public SerializableDictionary<Condition, byte> Conditions;

        /// <summary>
        /// Sheen this monster has.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public uint Sheen;

        /// <summary>
        /// When changing to max form, monsters will increase this stats based on this level.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public byte MaxFormLevel;

        /// <summary>
        /// This monster's data related to the virus.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public VirusData VirusData;

        /// <summary>
        /// This monster's ribbons.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public List<Ribbon> Ribbons;

        /// <summary>
        /// This monster's held item.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public Item HeldItem;

        /// <summary>
        /// Data related to being an egg.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public EggData EggData;

        /// <summary>
        /// Stores necessary data for monsters to evolve.
        /// </summary>
        [HideIf(nameof(IsNullEntry))]
        public ExtraData ExtraData;

        /// <summary>
        /// Get the height of the monster.
        /// </summary>
        /// <returns>The height of the monster.</returns>
        public float Height => FormData.Height * PhysicalData.HeightModifier;

        /// <summary>
        /// Get the Weight of the monster.
        /// </summary>
        /// <returns>The Weight of the monster.</returns>
        public float Weight
        {
            get
            {
                float weight = FormData.Weight * PhysicalData.WeightModifier;

                (float modifier, float multiplier) = GetAbility().GetMonsterWeight(this);

                return weight * multiplier + modifier;
            }
        }

        /// <summary>
        /// Can this monster battle?
        /// </summary>
        public bool CanBattle => CurrentHP > 0 && !EggData.IsEgg;

        /// <summary>
        /// Is this monster below its evolution level?
        /// </summary>
        public bool IsBelowEvolutionLevel =>
            FormData.Evolutions.Where(data => data is EvolveByLevel)
                    .Cast<EvolveByLevel>()
                    .Any(data => data.TargetLevel > StatData.Level);

        /// <summary>
        /// Parameter less constructor that may be used internally.
        /// </summary>
        internal MonsterInstance()
        {
        }

        /// <summary>
        /// Constructor for a monster instance wit overrides for most of its data.
        /// </summary>
        /// <param name="settings">Internal settings for YAPU reference.</param>
        /// <param name="database">Reference to the database to use to create this monster.</param>
        /// <param name="species">Species of this monster.</param>
        /// <param name="form">Form of this monster. If an invalid form is passed, it will use the default.</param>
        /// <param name="level">Level of this monster.</param>
        /// <param name="nature">Nature of this monster. Optional, a random one will be chosen.</param>
        /// <param name="ability">Ability of this monster. Optional, a random one will be chosen.</param>
        /// <param name="gender">Gender of this monster. Non binary species will take non binary always, binary species can be overriden by this parameter or a random one will be chosen.</param>
        /// <param name="heightModifierOverride">Override for the height multiplier.</param>
        /// <param name="weightModifierOverride">Override for the weight modifier.</param>
        /// <param name="friendshipModifier">Friendship of this monster. It will be added to the species base.</param>
        /// <param name="moveRosterOverride">Move roster for this monster. Optional, a random one will be chosen. If override contains invalid moves for the species, random ones will be chosen for those moves.</param>
        /// <param name="learntMovesOverride">Override for the learnt moves of this monster. Optional, default level ones will be filled.</param>
        /// <param name="ivOverride">IVs for this monster. Optional, random ones will be chosen.</param>
        /// <param name="evOverride">EVs for this monster. Optional, default will set them as 0s.</param>
        /// <param name="conditionsOverride">Conditions for this monster. Optional, default will set them as 0s.</param>
        /// <param name="maxFormLevel">Level of the max form for this monster. Optional, default is 0.</param>
        /// <param name="nickname">Nickname for this monster. Optional, it won't have a nick name if not set.</param>
        /// <param name="currentTrainer">Current trainer of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originRegion">Origin region of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originLocation">Origin location of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originalTrainer">Original trainer of this monster. Optional, the current trainer will be used.</param>
        /// <param name="originType">Type of origin of this monster. Default is caught.</param>
        /// <param name="captureBall">Ball it was captured with.</param>
        /// <param name="isAlpha">Is this an alpha monster?</param>
        /// <param name="hasVirus">Does this monster have the virus? Default is false.</param>
        /// <param name="ribbons">Ribbons for this monster. Default is none.</param>
        /// <param name="heldItem">Held item for this monster.</param>
        /// <param name="isEgg">Is this monster an egg?</param>
        // ReSharper disable once FunctionComplexityOverflow
        public MonsterInstance(YAPUSettings settings,
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
                               List<Move> learntMovesOverride = null,
                               SerializableDictionary<Stat, byte> ivOverride = null,
                               SerializableDictionary<Stat, byte> evOverride = null,
                               SerializableDictionary<Condition, byte> conditionsOverride = null,
                               byte maxFormLevel = 0,
                               string nickname = "",
                               string currentTrainer = "Wild",
                               string originRegion = "YAPULand",
                               string originLocation = "Route 1",
                               string originalTrainer = "",
                               OriginData.Type originType = OriginData.Type.Caught,
                               Ball captureBall = null,
                               bool isAlpha = false,
                               bool hasVirus = false,
                               List<Ribbon> ribbons = null,
                               Item heldItem = null,
                               bool isEgg = false)
        {
            DataByFormEntry data = ProcessSpeciesAndForm(species, form);

            CurrentTrainer = currentTrainer;

            PhysicalData =
                new MonsterPhysicalData(settings, data, gender, heightModifierOverride, weightModifierOverride);

            ProcessStatData(database, level, nature, ivOverride, evOverride, isEgg, settings);

            ProcessOriginData(settings,
                              originRegion,
                              originLocation,
                              originalTrainer,
                              originType,
                              captureBall,
                              isAlpha);

            ProcessAbility(data, ability);

            if (nickname.IsNullEmptyOrWhiteSpace())
                HasNickname = false;
            else
            {
                HasNickname = true;
                Nickname = nickname;
            }

            Friendship = (byte) Mathf.Clamp(data.BaseFriendship + friendshipModifier, 0, 255);

            Status = null;

            ProcessMoves(data, moveRosterOverride, learntMovesOverride);

            ProcessConditions(conditionsOverride);

            ProcessMaxFormLevel(maxFormLevel);

            VirusData = new VirusData(hasVirus);

            Ribbons = ribbons ?? new List<Ribbon>();

            CurrentHP = MonsterMathHelper.CalculateStat(this, Stat.Hp, null);

            ProcessHeldItem(heldItem);

            EggData = new EggData
                      {
                          IsEgg = isEgg,
                          EggCyclesLeft = isEgg ? data.EggCycles : (byte) 0
                      };

            if (ExtraData.PersonalityValue == 0) ExtraData.PersonalityValue = Random.Range(int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Retrieve the name or nickname of this monster.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized name or nickname of this monster.</returns>
        public virtual string GetNameOrNickName(ILocalizer localizer) =>
            EggData.IsEgg ? localizer["Monsters/Egg"] :
            HasNickname ? Nickname : localizer[Species.LocalizableName];

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => GetNameOrNickName(localizer);

        /// <summary>
        /// Change the HP of this monster.
        /// </summary>
        /// <param name="amount">The unclamped amount to change the HP to.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <returns>The new HP, the previous HP and if force survive was triggered..</returns>
        public (int NewHP, int PreviousHP, bool TriggeredForceSurvive) ChangeHP(int amount, bool forceSurvive = false)
        {
            uint maxHP = MonsterMathHelper.CalculateStat(this, Stat.Hp, null);

            int previousHP = (int) CurrentHP;

            int uncappedDelta = (int) (CurrentHP + amount);

            amount = Mathf.Min(uncappedDelta, (int) maxHP);

            bool triggeredForceSurvive = amount <= 0 && forceSurvive;

            amount = Mathf.Max(forceSurvive ? 1 : 0, amount);

            CurrentHP = (uint) amount;

            return ((int) CurrentHP, previousHP, triggeredForceSurvive);
        }

        /// <summary>
        /// Get the experience needed for next level.
        /// </summary>
        /// <param name="lookupTable">Lookup table to use.</param>
        /// <returns>The experience needed for the next level.</returns>
        public int GetExperienceForNextLevel(ExperienceLookupTable lookupTable) =>
            StatData.GetExperienceForNextLevel(FormData, lookupTable);

        /// <summary>
        /// Raise the experience of this monster.
        /// </summary>
        /// <param name="lookupTable">Reference to the experience lookup table to use.</param>
        /// <param name="amount">Amount to raise.</param>
        /// <param name="levelUpCallback">Callback raised if the monster levels up.</param>
        public void RaiseExperience(ExperienceLookupTable lookupTable, uint amount, Action<LevelUpData> levelUpCallback)
        {
            if (StatData.Level == 100) return;

            int remainingXP = (int) amount;

            while (remainingXP > 0)
            {
                int xpForNextLevel = StatData.GetExperienceForNextLevel(FormData, lookupTable);
                int xpLeftForNextLevel = (int) (xpForNextLevel - StatData.CurrentLevelExperience);

                int iterationAmount = Mathf.Min(remainingXP, xpLeftForNextLevel);
                remainingXP -= iterationAmount;

                StatData.CurrentLevelExperience += (uint) iterationAmount;

                if (StatData.CurrentLevelExperience != xpForNextLevel) continue;

                LevelUpData levelUpData = new()
                                          {
                                              PreviousXP = (uint) (StatData.CurrentLevelExperience - iterationAmount),
                                              PreviousData = GetStats(null)
                                          };

                StatData.Level++;
                StatData.CurrentLevelExperience = 0;

                levelUpData.Level = StatData.Level;
                levelUpData.NewData = GetStats(null);

                levelUpData.MovesToLearn = FormData.GetMovesLearntInLevel(StatData.Level);

                CurrentHP += levelUpData.NewData[Stat.Hp] - levelUpData.PreviousData[Stat.Hp];

                levelUpData.Monster = this;

                Logger.Info(levelUpData);

                levelUpCallback?.Invoke(levelUpData);

                if (StatData.Level == 100) remainingXP = 0;
            }
        }

        /// <summary>
        /// Change the friendship of this monster.
        /// </summary>
        /// <param name="amount">Amount to change.</param>
        /// <param name="currentScene">Current scene we are in.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public void ChangeFriendship(int amount, SceneInfoAsset currentScene, ILocalizer localizer)
        {
            int amountToChange = Mathf.Min(amount, byte.MaxValue - Friendship);
            amountToChange = Friendship + amountToChange < 0 ? 0 : amountToChange;

            if (amountToChange > 0)
            {
                // TODO: Held item callbacks (+50% if soothe bell).

                // This only works if the player didn't change the language, but there is not much we can do about that.
                if (OriginData.Location == localizer[currentScene.LocalizableNameKey]) amountToChange++;

                amountToChange += OriginData.Ball.GetFriendShipModifier();
            }

            // Need to reclamp after modifiers.
            amountToChange = Mathf.Min(amountToChange, byte.MaxValue - Friendship);
            amountToChange = Friendship + amountToChange < 0 ? 0 : amountToChange;

            Logger.Info(Species.name + "'s friendship changed by " + amountToChange + ".");

            Friendship = (byte) (Friendship + amountToChange);
        }

        /// <summary>
        /// Raise the value of an EV by the given amount.
        /// </summary>
        /// <param name="settings">Reference to the settings.</param>
        /// <param name="stat">Stat to raise.</param>
        /// <param name="amount">Amount to raise.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public void ChangeEV(YAPUSettings settings, Stat stat, int amount, ILocalizer localizer) =>
            StatData.ChangeEV(settings, VirusData, stat, amount, GetNameOrNickName(localizer), Logger);

        /// <summary>
        /// Retrieve the current stats of this monster.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A dictionary with the stats and its value.</returns>
        public virtual Dictionary<Stat, uint> GetStats(BattleManager battleManager)
        {
            Dictionary<Stat, uint> stats = new();

            foreach (Stat stat in Utils.GetAllItems<Stat>())
                stats[stat] = MonsterMathHelper.CalculateStat(this, stat, battleManager);

            if (battleManager == null) return stats;

            Dictionary<Stat, uint> newStats = new(stats);

            foreach (GlobalStatus status in battleManager.Statuses.GetGlobalStatuses().Select(slot => slot.Key))
            {
                SerializableDictionary<Stat, Stat>
                    replacements = status.OnCalculateStatReplacement(this, battleManager);

                foreach (KeyValuePair<Stat, Stat> replacement in replacements)
                    newStats[replacement.Value] = stats[replacement.Key];
            }

            return newStats;
        }

        /// <summary>
        /// Retrieve the current stats of this monster if it would use a specific form.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <param name="form">Form to check.</param>
        /// <returns>A dictionary with the stats and its value.</returns>
        public Dictionary<Stat, uint> GetStatsForSpecificForm(BattleManager battleManager, Form form)
        {
            Dictionary<Stat, uint> stats = new();

            foreach (Stat stat in Utils.GetAllItems<Stat>())
                stats[stat] = MonsterMathHelper.CalculateStatWithSpecificForm(this, form, stat, battleManager);

            return stats;
        }

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="stat">Stat to be calculated.</param>
        /// <param name="form">Form to calculate the stat with, this may not be the same form as the current one.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <param name="overrideBaseValue">If > 0 override the base value with this value.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public virtual float OnCalculateStat(Stat stat,
                                             Form form,
                                             BattleManager battleManager,
                                             out uint overrideBaseValue)
        {
            float multiplier = 1;
            overrideBaseValue = 0;

            multiplier *=
                (Form == form ? GetAbility() : GetAbilityToUseWithForm(Species, form)).OnCalculateStat(this,
                    stat,
                    battleManager);

            if (CanUseHeldItem()) multiplier *= HeldItem.OnCalculateStat(this, stat);

            if (GetStatus() != null) multiplier *= GetStatus().OnCalculateStat(this, stat, battleManager);

            return multiplier;
        }

        /// <summary>
        /// Completely heal this monster.
        /// TODO: What about permafaint?
        /// </summary>
        public virtual void CompletelyHeal()
        {
            CurrentHP = MonsterMathHelper.CalculateStat(this, Stat.Hp, null);

            for (int i = 0; i < CurrentMoves.Length; i++) CurrentMoves[i].CurrentPP = CurrentMoves[i].MaxPP;

            Status = null;
        }

        /// <summary>
        /// Check if the monster is at max health.
        /// </summary>
        /// <returns>True if it is.</returns>
        public bool IsAtMaxHP() => CurrentHP == MonsterMathHelper.CalculateStat(this, Stat.Hp, null);

        /// <summary>
        /// Called to check if the monster can use its held item.
        /// </summary>
        /// <returns>True if it can be used.</returns>
        public virtual bool CanUseHeldItem() => HeldItem != null;

        /// <summary>
        /// Consume the held item.
        /// </summary>
        public virtual IEnumerator ConsumeItem()
        {
            if (CanUseHeldItem()) yield return HeldItem.OnItemConsumed();

            HeldItem = null;
        }

        /// <summary>
        /// Clone this instance.
        /// </summary>
        public MonsterInstance Clone()
        {
            if (IsNullEntry) return default;

            return new MonsterInstance
                   {
                       Species = Species,
                       Form = Form,
                       OriginData = OriginData,
                       PhysicalData = PhysicalData,
                       StatData = StatData.DeepClone(),
                       Ability = GetAbility(),
                       HasNickname = HasNickname,
                       Nickname = Nickname,
                       CurrentTrainer = CurrentTrainer,
                       Friendship = Friendship,
                       CurrentHP = CurrentHP,
                       Status = Status,
                       CurrentMoves = CloneMoveSlots(),
                       LearntMoves = LearntMoves.ShallowClone(),
                       Conditions = Conditions.ShallowClone(),
                       Sheen = Sheen,
                       MaxFormLevel = MaxFormLevel,
                       VirusData = VirusData,
                       Ribbons = Ribbons.ShallowClone(),
                       HeldItem = HeldItem
                   };
        }

        /// <summary>
        /// Retrieve the types of this battler.
        /// They can be modified by volatile states.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The two types of the monster.</returns>
        public virtual (MonsterType, MonsterType) GetTypes(YAPUSettings settings) =>
            EggData.IsEgg ? (settings.EggsType, null) : (FormData.FirstType, FormData.SecondType);

        /// <summary>
        /// Check if the monster is of a type.
        /// </summary>
        /// <param name="monsterType">Type to check.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>True if it is.</returns>
        public bool IsOfType(MonsterType monsterType, YAPUSettings settings)
        {
            (MonsterType type0, MonsterType type1) = GetTypes(settings);
            return type0 == monsterType || type1 == monsterType;
        }

        /// <summary>
        /// Check if the monster is of any type on a list.
        /// </summary>
        /// <param name="monsterType">Types to check.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>True if it is.</returns>
        public bool IsOfAnyType(IEnumerable<MonsterType> monsterType, YAPUSettings settings) =>
            monsterType.Any(type => IsOfType(type, settings));

        /// <summary>
        /// Is this monster of the same gender of another given monster?
        /// </summary>
        /// <param name="other">Other to check.</param>
        /// <returns>True if it is of the same gender.</returns>
        public bool IsSameGenderAs(MonsterInstance other) =>
            PhysicalData.Gender != MonsterGender.NonBinary
         && other.PhysicalData.Gender != MonsterGender.NonBinary
         && PhysicalData.Gender == other.PhysicalData.Gender;

        /// <summary>
        /// Is this monster of the opposite gender of another given monster?
        /// </summary>
        /// <param name="other">Other to check.</param>
        /// <returns>True if it is of the opposite gender.</returns>
        public bool IsOppositeGenderOf(MonsterInstance other) =>
            PhysicalData.Gender != MonsterGender.NonBinary
         && other.PhysicalData.Gender != MonsterGender.NonBinary
         && PhysicalData.Gender != other.PhysicalData.Gender;

        /// <summary>
        /// Get this monster's ability.
        /// </summary>
        /// <returns>The monster's ability.</returns>
        public virtual Ability GetAbility() => Ability;

        /// <summary>
        /// Set the new ability of this monster.
        /// </summary>
        /// <param name="ability">New ability.</param>
        public void SetAbility(Ability ability) => Ability = ability;

        /// <summary>
        /// Get a clone of the move slots.
        /// </summary>
        /// <returns>A clone of the move slots.</returns>
        public MoveSlot[] CloneMoveSlots() =>
            new[] {CurrentMoves[0], CurrentMoves[1], CurrentMoves[2], CurrentMoves[3]};

        /// <summary>
        /// Exchange two move slots.
        /// </summary>
        /// <param name="first">First move slot.</param>
        /// <param name="second">Second move slot.</param>
        public void ExchangeMoveSlots(int first, int second) =>
            (CurrentMoves[first], CurrentMoves[second]) = (CurrentMoves[second], CurrentMoves[first]);

        /// <summary>
        /// Retrieve a list of the moves that can be used.
        /// </summary>
        /// <returns></returns>
        public virtual List<MoveSlot> GetUsableMoves(BattleManager battleManager) => GetMovesWithPP();

        /// <summary>
        /// Retrieve a list of the damage moves that can be used.
        /// </summary>
        /// <returns></returns>
        public virtual List<MoveSlot> GetUsableDamageMoves(BattleManager battleManager) =>
            GetUsableMoves(battleManager).Where(slot => slot.Move is DamageMove).ToList();

        /// <summary>
        /// Retrieve a list of the moves that have PP.
        /// </summary>
        /// <returns>A list of move slots.</returns>
        public List<MoveSlot> GetMovesWithPP() =>
            CurrentMoves
               .Where(slot =>
                          slot.Move != null
                       && slot.CurrentPP > 0)
               .ToList();

        /// <summary>
        /// Get the index of a move, if the battler has it.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>Its index or -1 if it doesn't have it.</returns>
        public int GetMoveIndex(Move move)
        {
            for (int i = 0; i < CurrentMoves.Length; i++)
                if (CurrentMoves[i].Move == move)
                    return i;

            Logger.Warn("This monster doesn't have that move.");

            return -1;
        }

        /// <summary>
        /// Check if the monster knows a move.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if it does.</returns>
        public bool KnowsMove(Move move) => CurrentMoves.Any(slot => slot.Move == move);

        /// <summary>
        /// Check if the monster can learn a move.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if it can.</returns>
        public bool CanLearnMove(Move move) => FormData.CanLearnMove(move) && !KnowsMove(move);

        /// <summary>
        /// Have the monster eat a dish.
        /// </summary>
        /// <param name="flavours">Flavours of the given dish.</param>
        /// <param name="smoothness">Smoothness of the given dish.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="callback">Callback that states true if the dish was eaten.</param>
        public IEnumerator EatDish(SerializableDictionary<Flavour, int> flavours,
                                   uint smoothness,
                                   PlayerCharacter playerCharacter,
                                   ILocalizer localizer,
                                   Action<bool> callback)
        {
            if (Sheen > playerCharacter.YAPUSettings.MaxSheen)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/Eating/NoMore",
                                                             localizableModifiers: false,
                                                             modifiers: GetLocalizedName(localizer));

                callback.Invoke(false);
                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Dialogs/Eating/Ate",
                                                         localizableModifiers: false,
                                                         modifiers: GetLocalizedName(localizer));

            float likingMultiplier;

            Flavour mainFlavour = Flavour.Neutral;
            int mainFlavourValue = int.MinValue;

            foreach (KeyValuePair<Flavour, int> pair in flavours)
                if (pair.Value > mainFlavourValue)
                {
                    mainFlavour = pair.Key;
                    mainFlavourValue = pair.Value;
                }

            if (StatData.Nature.GetLikedFlavour() == mainFlavour)
            {
                likingMultiplier = 1.1f;
                yield return DialogManager.ShowDialogAndWait("Dialogs/Eating/Liked");

                ChangeFriendship(Mathf.FloorToInt(flavours[mainFlavour] * likingMultiplier),
                                 playerCharacter.Scene.Asset,
                                 localizer);
            }
            else if (StatData.Nature.GetDislikedFlavour() == mainFlavour)
            {
                likingMultiplier = .9f;
                yield return DialogManager.ShowDialogAndWait("Dialogs/Eating/DidntLike");
            }
            else
                likingMultiplier = 1f;

            foreach (KeyValuePair<Flavour, int> flavour in flavours)
            {
                int boost = Mathf.Max(Mathf.FloorToInt(flavour.Value * likingMultiplier), 0);

                Conditions[flavour.Key.GetBoostedCondition()] =
                    (byte) Mathf.Clamp(Conditions[flavour.Key.GetBoostedCondition()] + boost, 0, byte.MaxValue);
            }

            Sheen += smoothness;

            if (Sheen < playerCharacter.YAPUSettings.MaxSheen) yield break;

            Sheen = playerCharacter.YAPUSettings.MaxSheen;

            yield return DialogManager.ShowDialogAndWait("Dialogs/Eating/Ate",
                                                         localizableModifiers: false,
                                                         modifiers: GetLocalizedName(localizer));
        }

        /// <summary>
        /// Add a status to this monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="showMessage">Show a message when adding?</param>
        /// <param name="finished">Callback telling if it was added.</param>
        public IEnumerator AddStatus(Status status,
                                     ILocalizer localizer,
                                     YAPUSettings settings,
                                     bool showMessage = true,
                                     Action<bool> finished = null)
        {
            if (status.CanAddStatus(this, settings))
            {
                Status = status;
                yield return status.OnStatusAdded(this, localizer, showMessage);
                finished?.Invoke(true);
            }
            else
                finished?.Invoke(false);
        }

        /// <summary>
        /// Get the status of this monster.
        /// </summary>
        /// <returns></returns>
        public Status GetStatus() => Status;

        /// <summary>
        /// Get the status of this monster when going out of battle.
        /// </summary>
        /// <returns></returns>
        public Status GetStatusForOutOfBattle() => Status == null ? null : Status.OutOfBattleDefault;

        /// <summary>
        /// Set the status.
        /// Don't call this method if you don't know what you are doing, use the AddStatus() method.
        /// </summary>
        /// <param name="status">Status to set.</param>
        internal void SetStatus(Status status) => Status = status;

        /// <summary>
        /// Remove the monster's status.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="showMessage">Show a message when removing?</param>
        public IEnumerator RemoveStatus(ILocalizer localizer, bool showMessage = true)
        {
            yield return Status.OnStatusRemoved(this, localizer, showMessage);
            Status = null;
        }

        /// <summary>
        /// Get the moves this monster has that can be used out of battle.
        /// </summary>
        /// <returns></returns>
        public List<Move> GetOutOfBattleMoves()
        {
            List<Move> moves = new();

            foreach (MoveSlot slot in CurrentMoves)
                if (slot.Move != null && slot.Move.CanBeUsedOutsideBattle)
                    moves.Add(slot.Move);

            return moves;
        }

        /// <summary>
        /// Called after the battle has happened.
        /// </summary>
        public virtual IEnumerator AfterBattle(ILocalizer localizer)
        {
            yield return GetAbility().AfterBattle(this, localizer);
        }

        /// <summary>
        /// Modify the minimum and maximum levels of an encounter.
        /// </summary>
        /// <param name="encounter">Encounter type.</param>
        /// <param name="minimum">Minimum level.</param>
        /// <param name="maximum">Maximum level.</param>
        /// <returns>The new limits.</returns>
        public (byte minimum, byte maximum)
            ModifyEncounterLevels(EncounterType encounter, byte minimum, byte maximum) =>
            GetAbility().ModifyEncounterLevels(this, encounter, minimum, maximum);

        /// <summary>
        /// Callback that allows the monster to modify the nature of an wild mon encounter.
        /// </summary>
        /// <param name="encounterType">Type of encounter.</param>
        /// <returns>The modified nature or null if not modified.</returns>
        public Nature ModifyEncounterNature(EncounterType encounterType) =>
            GetAbility().ModifyEncounterNature(this, encounterType);

        /// <summary>
        /// Modify the possible wild encounters that can be found.
        /// </summary>
        /// <param name="possibleEncounters">Current possible encounters.</param>
        /// <param name="sceneInfo">Info for the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        public void ModifyPossibleEncounters(ref List<WildEncounter> possibleEncounters,
                                             SceneInfo sceneInfo,
                                             EncounterType encounterType) =>
            GetAbility().ModifyPossibleEncounters(ref possibleEncounters, this, sceneInfo, encounterType);

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public float OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType)
        {
            float multiplier = 1;

            multiplier *= GetAbility().OnCalculateEncounterChance(playerCharacter, encounterType);

            if (CanUseHeldItem()) multiplier *= HeldItem.OnCalculateEncounterChance(encounterType);

            return multiplier;
        }

        /// <summary>
        /// Called after a wild level has been chosen on an encounter, last chance to prevent it.
        /// Ex: Keen eye.
        /// </summary>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        /// <param name="level">Level of the wild monster.</param>
        public bool ShouldPreventEncounter(EncounterType encounterType, byte level) =>
            GetAbility().ShouldPreventEncounter(encounterType, this, level);

        /// <summary>
        /// Allow the monster to evolve?
        /// </summary>
        /// <param name="evolutionData">Evolution data to use.</param>
        /// <returns>True if it allows evolution.</returns>
        public bool AllowEvolution(EvolutionData evolutionData) =>
            !CanUseHeldItem() || HeldItem.AllowEvolution(this, evolutionData);

        /// <summary>
        /// Callback that allows this monster to modify the steps needed for an egg cycle.
        /// </summary>
        /// <returns></returns>
        public float ModifyStepsNeededForEggCycle() => GetAbility().ModifyStepsNeededForEggCycle(this);

        /// <summary>
        /// Calculate the number of IVs to pass on breeding.
        /// </summary>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The number to pass. -1 if unchanged.</returns>
        public int OnCalculateNumberOfIVsToPassOnBreeding(MonsterInstance otherParent,
                                                          MonsterEntry species,
                                                          Form form,
                                                          bool isMother)
        {
            // ReSharper disable once InvertIf
            if (CanUseHeldItem())
            {
                int ivNumberOverride = HeldItem.OnCalculateNumberOfIVsToPassOnBreeding(this,
                    otherParent,
                    species,
                    form,
                    isMother);

                if (ivNumberOverride != -1) return ivNumberOverride;
            }

            return -1;
        }

        /// <summary>
        /// Calculate the specific IVs to pass on breeding.
        /// </summary>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>The specific IVs that should be passed.</returns>
        public SerializableDictionary<Stat, byte> OnCalculateSpecificIVsToPassOnBreeding(MonsterInstance otherParent,
            MonsterEntry species,
            Form form,
            bool isMother)
        {
            if (CanUseHeldItem())
                return HeldItem.OnCalculateSpecificIVsToPassOnBreeding(this,
                                                                       otherParent,
                                                                       species,
                                                                       form,
                                                                       isMother);

            return new SerializableDictionary<Stat, byte>();
        }

        /// <summary>
        /// Calculate the nature to pass on breeding.
        /// </summary>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="species">Species of the offspring.</param>
        /// <param name="form">Form of the offspring.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's nature?</returns>
        public bool OnCalculateNatureToPassOnBreeding(MonsterInstance otherParent,
                                                      MonsterEntry species,
                                                      Form form,
                                                      bool isMother)
        {
            if (CanUseHeldItem())
                return HeldItem.OnCalculateNatureToPassOnBreeding(this,
                                                                  otherParent,
                                                                  species,
                                                                  form,
                                                                  isMother);

            return false;
        }

        /// <summary>
        /// Calculate the form to pass on breeding.
        /// </summary>
        /// <param name="otherParent">Other parent.</param>
        /// <param name="isMother">Is this monster the mother?</param>
        /// <returns>Should it pass this parent's Form?</returns>
        public bool OnCalculateFormToPassOnBreeding(MonsterInstance otherParent,
                                                    bool isMother)
        {
            if (CanUseHeldItem())
                return HeldItem.OnCalculateFormToPassOnBreeding(this,
                                                                otherParent,
                                                                isMother);

            return false;
        }

        /// <summary>
        /// Get the sprite to look in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running?</param>
        /// <param name="fishing">Is the character fishing?</param>
        /// <returns>A sprite.</returns>
        // ReSharper disable once CyclomaticComplexity
        public Sprite GetLooking(Direction direction, bool swimming, bool biking, bool running, bool fishing) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            direction switch
            {
                Direction.Down when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldIdleShinySpriteDownMale,
                Direction.Down when Form.IsShiny => FormData.WorldIdleShinySpriteDown,
                Direction.Down when FormData.HasMaleMaterialOverride => FormData.WorldIdleSpriteDownMale,
                Direction.Down => FormData.WorldIdleSpriteDown,

                Direction.Up when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldIdleShinySpriteUpMale,
                Direction.Up when Form.IsShiny => FormData.WorldIdleShinySpriteUp,
                Direction.Up when FormData.HasMaleMaterialOverride => FormData.WorldIdleSpriteUpMale,
                Direction.Up => FormData.WorldIdleSpriteUp,

                Direction.Left when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldIdleShinySpriteLeftMale,
                Direction.Left when Form.IsShiny => FormData.WorldIdleShinySpriteLeft,
                Direction.Left when FormData.HasMaleMaterialOverride => FormData.WorldIdleSpriteLeftMale,
                Direction.Left => FormData.WorldIdleSpriteLeft,

                Direction.Right when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldIdleShinySpriteRightMale,
                Direction.Right when Form.IsShiny => FormData.WorldIdleShinySpriteRight,
                Direction.Right when FormData.HasMaleMaterialOverride => FormData.WorldIdleSpriteRightMale,
                Direction.Right => FormData.WorldIdleSpriteRight,

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        /// <summary>
        /// Get the sprites to walk in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running.</param>
        /// <returns>A list of sprites.</returns>
        // ReSharper disable once CyclomaticComplexity
        public List<Sprite> GetWalking(Direction direction, bool swimming, bool biking, bool running) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            direction switch
            {
                Direction.Down when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldWalkingShinySpriteDownMale,
                Direction.Down when Form.IsShiny => FormData.WorldWalkingShinySpriteDown,
                Direction.Down when FormData.HasMaleMaterialOverride => FormData.WorldWalkingSpriteDownMale,
                Direction.Down => FormData.WorldWalkingSpriteDown,

                Direction.Up when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldWalkingShinySpriteUpMale,
                Direction.Up when Form.IsShiny => FormData.WorldWalkingShinySpriteUp,
                Direction.Up when FormData.HasMaleMaterialOverride => FormData.WorldWalkingSpriteUpMale,
                Direction.Up => FormData.WorldWalkingSpriteUp,

                Direction.Left when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldWalkingShinySpriteLeftMale,
                Direction.Left when Form.IsShiny => FormData.WorldWalkingShinySpriteLeft,
                Direction.Left when FormData.HasMaleMaterialOverride => FormData.WorldWalkingSpriteLeftMale,
                Direction.Left => FormData.WorldWalkingSpriteLeft,

                Direction.Right when Form.IsShiny && FormData.HasMaleMaterialOverride => FormData
                   .WorldWalkingShinySpriteRightMale,
                Direction.Right when Form.IsShiny => FormData.WorldWalkingShinySpriteRight,
                Direction.Right when FormData.HasMaleMaterialOverride => FormData.WorldWalkingSpriteRightMale,
                Direction.Right => FormData.WorldWalkingSpriteRight,

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        /// <summary>
        /// Get the monster icon.
        /// </summary>
        /// <returns>A sprite with the icon.</returns>
        public Sprite GetIcon() => FormData.GetIcon(EggData.IsEgg, Form.IsShiny, PhysicalData.Gender);

        /// <summary>
        /// Rebuild the origin data of a monster when it has been captured or hatched.
        /// </summary>
        /// <param name="sceneInfoAsset"></param>
        /// <param name="originType">Origin type.</param>
        /// <param name="trainerName">Name of the trainer that captured it.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public void RebuildOriginData(SceneInfoAsset sceneInfoAsset,
                                      OriginData.Type originType,
                                      string trainerName,
                                      YAPUSettings yapuSettings,
                                      ILocalizer localizer) =>
            OriginData = new OriginData
                         {
                             Game = yapuSettings.OriginGame,
                             Region = localizer[sceneInfoAsset.Region.LocalizableName],
                             Location = localizer[sceneInfoAsset.LocalizableNameKey],
                             Ball = OriginData.Ball,
                             IsAlpha = OriginData.IsAlpha,
                             OriginalLevel = StatData.Level,
                             OriginType = originType,
                             Trainer = trainerName
                         };

        /// <summary>
        /// Change the form of this monster.
        /// </summary>
        /// <param name="newForm">New form to change to.</param>
        public void ChangeForm(Form newForm) => EvolveToSpeciesAndForm(Species, newForm);

        /// <summary>
        /// Evolve this monster to a new species and form.
        /// </summary>
        /// <param name="newSpecies">New species to evolve to.</param>
        /// <param name="newForm">New form to evolve to.</param>
        /// <param name="retroactivelyLearnMoves">Retroactively learn the moves for the new species and form?</param>
        /// <param name="replaceHp">Replace the Hp using a percentage?</param>
        public void EvolveToSpeciesAndForm(MonsterEntry newSpecies,
                                           Form newForm,
                                           bool retroactivelyLearnMoves = true,
                                           bool replaceHp = true)
        {
            float healthPercentage = (float) CurrentHP / GetStats(null)[Stat.Hp];

            Ability = GetAbilityToUseWithForm(newSpecies, newForm);

            Species = newSpecies;
            Form = newForm;

            if (replaceHp) CurrentHP = (uint) (GetStats(null)[Stat.Hp] * healthPercentage);

            if (!retroactivelyLearnMoves) return;

            foreach (MoveLevelPair slot in FormData.MovesByLevel
                                                   .Where(slot => slot.Level <= StatData.Level
                                                               && !LearntMoves.Contains(slot.Move)))
                LearntMoves.Add(slot.Move);
        }

        /// <summary>
        /// Callback that allows the monster to add extra moves to the baby mon when breeding.
        /// this is useful for effects like Light Ball.
        /// </summary>
        /// <param name="isMother">Is this mon the mother?</param>
        /// <param name="otherParent">Reference to the other parent.</param>
        /// <param name="babySpecies">Species of the baby.</param>
        /// <param name="babyForm">Form of the baby.</param>
        /// <returns>A list of the moves to include.</returns>
        public List<Move> AddExtraLearntMovesWhenBreeding(bool isMother,
                                                          MonsterInstance otherParent,
                                                          MonsterEntry babySpecies,
                                                          Form babyForm)
        {
            List<Move> moves = new();

            // ReSharper disable once InvertIf
            if (CanUseHeldItem())
                foreach (Move move in HeldItem
                                     .AddExtraLearntMovesWhenBreeding(this,
                                                                      isMother,
                                                                      otherParent,
                                                                      babySpecies,
                                                                      babyForm)
                                     .Where(move => !moves.Contains(move)))
                    moves.Add(move);

            return moves;
        }

        /// <summary>
        /// Get the ability this monster would have with the given form.
        /// </summary>
        /// <param name="newSpecies">Species to check.</param>
        /// <param name="newForm">Form to check.</param>
        /// <returns>The ability this monster would have with that form.</returns>
        private Ability GetAbilityToUseWithForm(MonsterEntry newSpecies, Form newForm)
        {
            DataByFormEntry newFormData = newSpecies[newForm];
            Ability hypotheticalAbility = newFormData.Abilities[0];

            // We don't want to use GetAbility() here because we want the original indexes of the abilities.
            if (FormData.Abilities.Contains(Ability))
                hypotheticalAbility = FormData.Abilities.Count <= newFormData.Abilities.Count
                                          ? newFormData.Abilities[FormData.Abilities.IndexOf(Ability)]
                                          : newFormData.Abilities[0];

            if (!FormData.HiddenAbilities.Contains(Ability)) return hypotheticalAbility;

            if (FormData.HiddenAbilities.Count <= newFormData.HiddenAbilities.Count)
                hypotheticalAbility = newFormData.HiddenAbilities[FormData.HiddenAbilities.IndexOf(Ability)];
            else if (newFormData.HiddenAbilities.Count > 0)
                hypotheticalAbility = newFormData.HiddenAbilities[0];
            else
                hypotheticalAbility = newFormData.Abilities[0];

            return hypotheticalAbility;
        }

        /// <summary>
        /// Process the species and form when creating the monster.
        /// </summary>
        /// <param name="species">Species of this monster.</param>
        /// <param name="form">Form of this monster. If an invalid form is passed, it will use the default.</param>
        private DataByFormEntry ProcessSpeciesAndForm(MonsterEntry species,
                                                      Form form)
        {
            Species = species;

            if (!Species.IsFormAvailable(form))
            {
                Logger.Error(Species.name + " doesn't have the " + form.name + " form, defaulting.");
                Form = Species.GetFirstAvailableForm();
            }
            else
                Form = form;

            return Species[Form];
        }

        /// <summary>
        /// Process the stat data when creating the monster.
        /// </summary>
        /// <param name="database">Reference to the database to use to create this monster.</param>
        /// <param name="level">Level of this monster.</param>
        /// <param name="nature">Nature of this monster. Optional, a random one will be chosen.</param>
        /// <param name="ivOverride">IVs for this monster. Optional, random ones will be chosen.</param>
        /// <param name="evOverride">EVs for this monster. Optional, default will set them as 0s.</param>
        /// <param name="isEgg">Is this monster an egg?</param>
        /// <param name="settings">Reference to the settings.</param>
        private void ProcessStatData(MonsterDatabaseInstance database,
                                     byte level,
                                     Nature nature,
                                     SerializableDictionary<Stat, byte> ivOverride,
                                     SerializableDictionary<Stat, byte> evOverride,
                                     bool isEgg,
                                     YAPUSettings settings) =>
            StatData = new MonsterStatData(database,
                                           (byte) Mathf.Clamp(isEgg ? settings.LevelMonstersAreBornAt : level, 1, 100),
                                           nature,
                                           ivOverride,
                                           evOverride);

        /// <summary>
        /// Process the origin data when creating the monster.
        /// </summary>
        /// <param name="settings">Internal settings for YAPU reference.</param>
        /// <param name="originRegion">Origin region of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originLocation">Origin location of this monster. Optional, though it should overriden most of the time.</param>
        /// <param name="originalTrainer">Original trainer of this monster. Optional, the current trainer will be used.</param>
        /// <param name="originType">Type of origin of this monster. Default is caught.</param>
        /// <param name="captureBall">Ball it was captured with.</param>
        /// <param name="isAlpha">Is this an alpha monster?</param>
        private void ProcessOriginData(YAPUSettings settings,
                                       string originRegion,
                                       string originLocation,
                                       string originalTrainer,
                                       OriginData.Type originType,
                                       Ball captureBall,
                                       bool isAlpha) =>
            OriginData = new OriginData
                         {
                             Game = settings.OriginGame,
                             Region = originRegion,
                             Location = originLocation,
                             Trainer = originalTrainer.IsNullEmptyOrWhiteSpace()
                                           ? CurrentTrainer
                                           : originalTrainer,
                             OriginType = originType,
                             OriginalLevel = originType == OriginData.Type.Hatched ? (byte) 1 : StatData.Level,
                             IsAlpha = isAlpha,
                             Ball = captureBall != null ? captureBall : settings.DefaultBall
                         };

        /// <summary>
        /// Process the ability parameter on creation.
        /// </summary>
        /// <param name="data">Data for this species and form.</param>
        /// <param name="ability">Ability that was passed as parameter.</param>
        private void ProcessAbility(DataByFormEntry data, Ability ability)
        {
            if (ability != null)
                if (data.Abilities.Contains(ability) || data.HiddenAbilities.Contains(ability))
                    Ability = ability;
                else
                {
                    Logger.Error(Species.name
                               + " with form "
                               + Form.name
                               + " can't have the "
                               + ability.name
                               + ", defaulting.");

                    Ability = data.Abilities[0];
                }
            else
                Ability = data.Abilities.Random();
        }

        /// <summary>
        /// Process the moves passed by parameter on creation.
        /// </summary>
        /// <param name="data">Data for this species and form.</param>
        /// <param name="moveRosterOverride">Override of the move roster passed as parameter.</param>
        /// <param name="learntMovesOverride">Learnt moves override.</param>
        private void ProcessMoves(DataByFormEntry data,
                                  IReadOnlyList<Move> moveRosterOverride,
                                  List<Move> learntMovesOverride)
        {
            CurrentMoves = new MoveSlot[] {default, default, default, default};

            LearntMoves = learntMovesOverride ?? new List<Move>();

            MoveLevelPair[] levelMovesDescending = data.MovesByLevel.Where(pair => pair.Level <= StatData.Level)
                                                       .OrderByDescending(x => x.Level)
                                                       .ToArray();

            if (moveRosterOverride == null)
                for (int i = 0; i < 4; ++i)
                    CurrentMoves[i] = levelMovesDescending.Length > i
                                          ? new MoveSlot(levelMovesDescending[i].Move)
                                          : default;
            else
                for (int i = 0; i < moveRosterOverride.Count; ++i)
                {
                    Move move = moveRosterOverride[i];

                    if (data.CanLearnMove(move))
                        CurrentMoves[i] = new MoveSlot(move);
                    else
                    {
                        Logger.Error(move != null
                                         ? Species.name
                                         + " with form "
                                         + Form.name
                                         + " can't learn the "
                                         + move.name
                                         + " move, defaulting."
                                         : "Move number " + i + " was null. Defaulting.");

                        CurrentMoves[i] = levelMovesDescending.Length > i
                                       && !LearntMoves.Contains(levelMovesDescending[i].Move)
                                              ? new MoveSlot(levelMovesDescending[i].Move)
                                              : default;
                    }
                }

            foreach (MoveSlot moveSlot in CurrentMoves)
                if (moveSlot.Move != null && !LearntMoves.Contains(moveSlot.Move))
                    LearntMoves.Add(moveSlot.Move);

            foreach (MoveLevelPair pair in data.MovesByLevel.Where(pair => pair.Level <= StatData.Level
                                                                        && !LearntMoves.Contains(pair.Move)))
                LearntMoves.Add(pair.Move);
        }

        /// <summary>
        /// Process the conditions parameter when creating.
        /// </summary>
        /// <param name="conditionsOverride">Override of the initial conditions.</param>
        private void ProcessConditions(SerializableDictionary<Condition, byte> conditionsOverride)
        {
            if (conditionsOverride == null)
            {
                Conditions = new SerializableDictionary<Condition, byte>();

                foreach (Condition condition in Utils.GetAllItems<Condition>()) Conditions[condition] = 0;
            }
            else
                Conditions = conditionsOverride;
        }

        /// <summary>
        /// Process the max form level when creating.
        /// </summary>
        /// <param name="maxFormLevel">Max form level to add.</param>
        private void ProcessMaxFormLevel(byte maxFormLevel)
        {
            MaxFormLevel = maxFormLevel;

            MaxFormLevel = (byte) Mathf.Clamp(MaxFormLevel, 0, 10);
        }

        /// <summary>
        /// Process the held item parameter when creating.
        /// </summary>
        /// <param name="itemToHold"></param>
        private void ProcessHeldItem(Item itemToHold)
        {
            if (itemToHold == null) return;

            if (!itemToHold.CanBeHeld)
            {
                Logger.Error(itemToHold.name + " can't be held!");
                return;
            }

            HeldItem = itemToHold;
        }

        /// <summary>
        /// Data sent back when the monster levels up.
        /// </summary>
        public struct LevelUpData
        {
            /// <summary>
            /// The new level.
            /// </summary>
            public byte Level;

            /// <summary>
            /// Experience before starting the level up.
            /// </summary>
            public uint PreviousXP;

            /// <summary>
            /// Previous level data.
            /// </summary>
            public Dictionary<Stat, uint> PreviousData;

            /// <summary>
            /// New data.
            /// </summary>
            public Dictionary<Stat, uint> NewData;

            /// <summary>
            /// Moves that should be learn after the level up.
            /// </summary>
            public List<Move> MovesToLearn;

            /// <summary>
            /// Reference to the monster leveling up.
            /// </summary>
            public MonsterInstance Monster;

            /// <summary>
            /// Print the level up data.
            /// </summary>
            /// <returns>A legible string.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder("New level: ").Append(Level)
                                                                        .Append("\n")
                                                                        .Append("Previous level stats:\n");

                foreach ((Stat stat, uint value) in PreviousData)
                    builder.Append(stat).Append(": ").Append(value).Append("\n");

                builder.Append("This level stats:\n");

                foreach ((Stat stat, uint value) in NewData)
                    builder.Append(stat).Append(": ").Append(value).Append("\n");

                foreach (Move move in MovesToLearn)
                    builder.Append("Can learn move: ").Append(move.LocalizableName).Append("\n");

                return builder.ToString();
            }
        }
    }
}