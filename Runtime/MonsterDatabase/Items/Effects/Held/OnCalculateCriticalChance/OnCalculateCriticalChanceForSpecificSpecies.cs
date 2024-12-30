using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateCriticalChance
{
    /// <summary>
    /// Base class for item effects that modify the critical chance when using a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateCriticalChanceItemEffect/SpecificSpecies",
                     fileName = "OnCalculateCriticalChanceForSpecificSpecies")]
    public class OnCalculateCriticalChanceForSpecificSpecies : OnCalculateCriticalChanceItemEffect
    {
        /// <summary>
        /// Species compatible with this effect.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        [SerializeField]
        private List<MonsterEntry> CompatibleSpecies;

        /// <summary>
        /// Apply only for specific species.
        /// </summary>
        public override bool OnCalculateCriticalChance(Battler owner,
                                                       Item item,
                                                       Battler target,
                                                       BattleManager battleManager,
                                                       Move move,
                                                       ref float multiplier,
                                                       ref byte modifier,
                                                       ref bool alwaysHit,
                                                       out bool shouldConsume)
        {
            if (CompatibleSpecies.Contains(owner.Species))
            {
                item.ShowItemNotification(owner, battleManager.Localizer);

                multiplier *= Multiplier;
                modifier += CriticalStageModifier;
                alwaysHit |= AlwaysHit;

                shouldConsume = ShouldConsume;
                return true;
            }

            multiplier = 1;
            modifier = 0;
            alwaysHit = false;

            shouldConsume = false;

            return false;
        }
    }
}