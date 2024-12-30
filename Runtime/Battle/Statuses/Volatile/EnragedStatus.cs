using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for when a monster is enraged.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Enraged", fileName = "EnragedStatus")]
    public class EnragedStatus : VolatileStatus
    {
        /// <summary>
        /// Reference to the rage move.
        /// </summary>
        [FoldoutGroup("Rage")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move Rage;

        /// <summary>
        /// Stat to increase.
        /// </summary>
        [FoldoutGroup("Rage")]
        [SerializeField]
        private Stat StatToIncrease;

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break; // No dialog.
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// Remove the status if the move used is not rage.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler,
                                                      Move move,
                                                      BattleManager battleManager,
                                                      List<(BattlerType Type, int Index)> targets,
                                                      Action<bool> finished)
        {
            if (move != Rage) battleManager.Statuses.ScheduleRemoveStatus(this, battler);

            yield return base.OnAboutToUseAMove(battler, move, battleManager, targets, finished);
        }

        /// <summary>
        /// Called when the holder is hit by a move.
        /// Increase the stat stage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="finished">Callback stating the multiplier for the effectiveness and if it will force survive.</param>
        public override IEnumerator OnHitByMove(DamageMove move,
                                                float effectiveness,
                                                Battler battler,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                bool ignoresAbilities,
                                                Action<float, bool> finished)
        {
            (BattlerType battlerType, int battlerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.BattlerStats.ChangeStatStage(battlerType,
                                                                    battlerIndex,
                                                                    StatToIncrease,
                                                                    1,
                                                                    battlerType,
                                                                    battlerIndex,
                                                                    ignoreAbilities: ignoresAbilities);

            yield return base.OnHitByMove(move,
                                          effectiveness,
                                          battler,
                                          battleManager,
                                          moveUser,
                                          ignoresAbilities,
                                          finished);
        }
    }
}