using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Monster.Saves
{
    /// <summary>
    /// Version of the MonsterInstance class that can be serialized to a string.
    /// </summary>
    [Serializable]
    public class SavableMonsterInstance
    {
        /// <summary>
        /// Is this a null entry?
        /// </summary>
        public bool IsNullEntry;

        /// <summary>
        /// Monster species.
        /// </summary>
        public int Species;

        /// <summary>
        /// Monster form.
        /// </summary>
        public int Form;

        /// <summary>
        /// Origin data.
        /// </summary>
        public SavableOriginData OriginData;

        /// <summary>
        /// Physical data.
        /// </summary>
        public MonsterPhysicalData PhysicalData;

        /// <summary>
        /// Stat data.
        /// </summary>
        public SavableStatData StatData;

        /// <summary>
        /// Ability.
        /// </summary>
        public int Ability;

        /// <summary>
        /// Does it have a nickname?
        /// </summary>
        public bool HasNickname;

        /// <summary>
        /// Nickname, if it has one.
        /// </summary>
        public string Nickname;

        /// <summary>
        /// Current trainer.
        /// </summary>
        public string CurrentTrainer;

        /// <summary>
        /// This monster's friendship.
        /// </summary>
        public byte Friendship;

        /// <summary>
        /// This monster's HP.
        /// </summary>
        public uint CurrentHP;

        /// <summary>
        /// Status.
        /// </summary>
        public string Status;

        /// <summary>
        /// Move slot.
        /// </summary>
        public List<SavableMoveSlot> CurrentMoves;

        /// <summary>
        /// Learnt moves.
        /// </summary>
        public List<int> LearntMoves;

        /// <summary>
        /// Conditions this monster has.
        /// </summary>
        public SerializableDictionary<Condition, byte> Conditions;
        
        /// <summary>
        /// Sheen this monster has.
        /// </summary>
        public uint Sheen;

        /// <summary>
        /// When changing to max form, monsters will increase this stats based on this level.
        /// </summary>
        public byte MaxFormLevel;

        /// <summary>
        /// This monster's data related to the virus.
        /// </summary>
        public VirusData VirusData;

        /// <summary>
        /// Ribbons this monster has.
        /// </summary>
        public List<string> Ribbons;

        /// <summary>
        /// Held item.
        /// </summary>
        public int HeldItem;

        /// <summary>
        /// Data related to being an egg.
        /// </summary>
        public EggData EggData;

        /// <summary>
        /// Stores necessary data for monsters to evolve.
        /// </summary>
        [FormerlySerializedAs("EvolutionTrackingData")]
        public ExtraData ExtraData;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="monsterInstance">Original monster instance.</param>
        public SavableMonsterInstance(MonsterInstance monsterInstance)
        {
            if (monsterInstance == null || monsterInstance.IsNullEntry)
            {
                IsNullEntry = true;
                return;
            }

            Species = monsterInstance.Species.name.GetHashCode();
            Form = monsterInstance.Form.name.GetHashCode();
            OriginData = new SavableOriginData(monsterInstance.OriginData);
            PhysicalData = monsterInstance.PhysicalData;
            StatData = new SavableStatData(monsterInstance.StatData);
            Ability = monsterInstance.GetAbility().name.GetHashCode();
            HasNickname = monsterInstance.HasNickname;
            CurrentTrainer = monsterInstance.CurrentTrainer;
            Friendship = monsterInstance.Friendship;
            CurrentHP = monsterInstance.CurrentHP;
            Status = monsterInstance.GetStatus() == null ? "Null" : monsterInstance.GetStatus().name;

            CurrentMoves = new List<SavableMoveSlot>();
            foreach (MoveSlot slot in monsterInstance.CurrentMoves) CurrentMoves.Add(new SavableMoveSlot(slot));

            LearntMoves = new List<int>();
            foreach (Move move in monsterInstance.LearntMoves) LearntMoves.Add(move.name.GetHashCode());

            Conditions = monsterInstance.Conditions;
            Sheen = monsterInstance.Sheen;
            MaxFormLevel = monsterInstance.MaxFormLevel;
            VirusData = monsterInstance.VirusData;

            Ribbons = new List<string>();
            foreach (Ribbon ribbon in monsterInstance.Ribbons) Ribbons.Add(ribbon.name);

            HeldItem = monsterInstance.HeldItem == null
                           ? "Null".GetHashCode()
                           : monsterInstance.HeldItem.name.GetHashCode();

            ExtraData = monsterInstance.ExtraData;
            EggData = monsterInstance.EggData;
        }

        /// <summary>
        /// Load the data back into a monster instance.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="database">Database reference.</param>
        /// <returns>A full monster instance.</returns>
        public MonsterInstance ToMonsterInstance(YAPUSettings settings, MonsterDatabaseInstance database)
        {
            if (IsNullEntry) return null;

            MoveSlot[] moves = new MoveSlot[4];

            for (int i = 0; i < CurrentMoves.Count; i++)
            {
                SavableMoveSlot slot = CurrentMoves[i];

                moves[i] = slot.MoveHash != "Null".GetHashCode()
                               ? new MoveSlot(database.GetMoveByHashKey(slot.MoveHash))
                                 {
                                     MaxPP = slot.MaxPP,
                                     CurrentPP = slot.CurrentPP
                                 }
                               : new MoveSlot();
            }

            List<Move> learntMoves = new();

            foreach (Move move in LearntMoves
                                 .Select(database.GetMoveByHashKey)
                                 .Where(move => !learntMoves.Contains(move)))
                learntMoves.Add(move);

            List<Ribbon> ribbons = Ribbons.Select(database.GetRibbonByName).ToList();
            
            // Hack to add a personality value to saves from older versions.
            if (ExtraData.PersonalityValue == 0) ExtraData.PersonalityValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            MonsterInstance monsterInstance = new(settings,
                                                  database,
                                                  database.GetMonsterByHash(Species),
                                                  database.GetFormByHash(Form),
                                                  StatData.Level,
                                                  database.GetNatures().First(nature => nature.name == StatData.Nature),
                                                  database.GetAbilityByHash(Ability),
                                                  PhysicalData.Gender,
                                                  PhysicalData.HeightModifier,
                                                  PhysicalData.WeightModifier,
                                                  0,
                                                  null, // Check below for moves.
                                                  learntMoves,
                                                  StatData.IndividualValues,
                                                  StatData.EffortValues,
                                                  Conditions,
                                                  MaxFormLevel,
                                                  HasNickname ? Nickname : "",
                                                  CurrentTrainer,
                                                  OriginData.Region, // Check below for origin game.
                                                  OriginData.Location,
                                                  OriginData.Trainer,
                                                  OriginData.OriginType,
                                                  (Ball) database.GetItemByHash(OriginData.Ball),
                                                  OriginData.IsAlpha,
                                                  false, // Check below for virus.
                                                  ribbons,
                                                  HeldItem == "Null".GetHashCode()
                                                      ? null
                                                      : database.GetItemByHash(HeldItem))
                                              {
                                                  CurrentHP = CurrentHP,
                                                  CurrentMoves =
                                                      moves, // This one is saved later as the constructor parameter would override the max and current PPs.
                                                  VirusData =
                                                      VirusData, // Constructor only allow for the having virus boolean.
                                                  ExtraData = ExtraData,
                                                  EggData = EggData,
                                                  Friendship = Friendship,
                                                  Sheen = Sheen
                                              };

            monsterInstance.StatData.CurrentLevelExperience = StatData.CurrentLevelExperience;

            monsterInstance.OriginData.Game = OriginData.Game; // Game isn't available on constructor.

            monsterInstance.SetStatus(Status == "Null"
                                          ? null
                                          : database.GetStatus().First(status => status.name == Status));

            return monsterInstance;
        }
    }
}