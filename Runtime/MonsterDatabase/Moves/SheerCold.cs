using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move SheerCold.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/SheerCold", fileName = "SheerCold")]
    public class SheerCold : OhkoMove
    {
        // TODO: Animation.

        /// <summary>
        /// Types that have a different base accuracy.
        /// </summary>
        [FoldoutGroup("Targeting")]
        [SerializeField]
        private SerializableDictionary<MonsterType, int> TypesWithDifferentBaseAccuracy;

        /// <summary>
        /// Formula: https://bulbapedia.bulbagarden.net/wiki/Sheer_Cold_(move)#Generation_VII_onwards
        /// </summary>
        public override int GetPreStageAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager)
        {
            int accuracy = BaseAccuracy;

            foreach (KeyValuePair<MonsterType, int> pair in TypesWithDifferentBaseAccuracy)
                if (user.IsOfType(pair.Key, battleManager.YAPUSettings))
                    accuracy = pair.Value;

            if (target == null) return accuracy;

            if (user.StatData.Level < target.StatData.Level) return 0;

            return user.StatData.Level - target.StatData.Level + accuracy;
        }
    }
}