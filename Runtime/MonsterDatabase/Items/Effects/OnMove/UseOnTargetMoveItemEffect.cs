using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnMove
{
    /// <summary>
    /// Data class representing an effect of an item that can be used on a target move slot.
    /// </summary>
    public abstract class UseOnTargetMoveItemEffect : MonsterDatabaseScriptable<UseOnTargetMoveItemEffect>
    {
        /// <summary>
        /// Check if the effect can be used on a monster.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public virtual bool IsCompatible(MonsterInstance monsterInstance) => true;

        /// <summary>
        /// Check if the effect can be used on a move slot.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="index">Move slot index.</param>
        /// <returns>True if it can be used.</returns>
        public virtual bool IsMoveCompatible(MonsterInstance monsterInstance, int index) => true;

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="index">Move slot index.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public virtual IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                        int index,
                                                        PlayerCharacter playerCharacter,
                                                        ILocalizer localizer,
                                                        Action<bool> finished)
        {
            yield break;
        }

        /// <summary>
        /// Check if the effect can be used on a monster in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public virtual bool IsCompatible(BattleManager battleManager, Battler battler) => true;

        /// <summary>
        /// Check if the effect can be used on a move slot.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="index">Move slot index.</param>
        /// <returns>True if it can be used.</returns>
        public virtual bool IsMoveCompatible(BattleManager battleManager, Battler battler, int index) => true;

        /// <summary>
        /// Use on a battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="index">Move slot index.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public virtual IEnumerator UseOnBattler(BattleManager battleManager,
                                                Battler battler,
                                                int index,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            yield break;
        }
    }
}