using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability MindsEye.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MindsEye", fileName = "MindsEye")]
    public class MindsEye : Ability
    {
        /// <summary>
        /// Type bypassed by this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [SerializeField]
        private MonsterType BypassedType;

        /// <summary>
        /// Prevent lowering accuracy.
        /// </summary>
        public override IEnumerator OnStatChange(Battler owner,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Action<short> callback)
        {
            callback.Invoke(modifier < 0 && stat == BattleStat.Accuracy ? (short) 0 : modifier);
            yield break;
        }

        /// <summary>
        /// Ignores evasion.
        /// </summary>
        public override bool IgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing() => true;

        /// <summary>
        /// Allow the user of a move to modify the effectiveness of a move when attacking.
        /// </summary>
        public override void ModifyMultiplierOfTypesWhenAttacking(Battler owner,
                                                                  Battler target,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  ref float multiplier)
        {
            if (target.IsOfType(BypassedType, battleManager.YAPUSettings) && multiplier < 1) multiplier = 1;
        }
    }
}