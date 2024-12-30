using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability QuickDraw.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/QuickDraw", fileName = "QuickDraw")]
    public class QuickDraw : Ability
    {
        /// <summary>
        /// Chance to be first.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float Chance = .3f;

        /// <summary>
        /// Called to determine the priority of the battler inside its priority bracket.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Finished callback stating if it should go first (1), last (-1) or normal (0).</param>
        public override IEnumerator OnDeterminePriority(Battler battler,
                                                        BattleManager battleManager,
                                                        Action<int> finished)
        {
            if (battleManager.RandomProvider.Value01() <= Chance)
            {
                finished.Invoke(0);
                yield break;
            }

            ShowAbilityNotification(battler);

            finished.Invoke(1);
        }
    }
}