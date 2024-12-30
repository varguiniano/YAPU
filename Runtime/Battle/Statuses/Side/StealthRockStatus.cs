using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the StealthRock side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/StealthRockStatus", fileName = "StealthRockStatus")]
    public class StealthRockStatus : SideStatus
    {
        /// <summary>
        /// Type this status will use to calculate damage.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType Type;

        /// <summary>
        /// Percentage of HP to inflict based on the effectiveness.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<float, float> DamagePerEffectiveness;

        /// <summary>
        /// Callback for when a battler enters the battle on the side this status is in.
        /// </summary>
        /// <param name="side">Side of this status.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnBattlerEnteredSide(BattlerType side,
                                                         int battlerIndex,
                                                         BattleManager battleManager)
        {
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(side, battlerIndex);

            float effectiveness = battler.GetEffectivenessOfType(Type, battleManager, true);

            float damagePercentage = 0;

            foreach (KeyValuePair<float, float> pair in DamagePerEffectiveness)
                if (effectiveness <= pair.Key + 0.01f) // 0.01 for float precision tolerance.
                    damagePercentage = pair.Value;

            int damage = -Mathf.Max(Mathf.FloorToInt(battler.GetStats(battleManager)[Stat.Hp] * damagePercentage), 1);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                side,
                                                                battlerIndex,
                                                                damage,
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return DialogManager.ShowDialogAndWait("Status/Side/StealthRockStatus/Effect",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer));
        }
    }
}