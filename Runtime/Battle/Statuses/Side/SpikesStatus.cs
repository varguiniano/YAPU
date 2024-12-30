using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the Spikes side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/SpikesStatus", fileName = "SpikesStatus")]
    public class SpikesStatus : LayeredSideStatus
    {
        /// <summary>
        /// Damage done based on the number of layers.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<uint, float> DamagePerLayer;

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

            if (!battler.IsGrounded(battleManager, false)) yield break;

            uint layer = LayerCount[side];

            if (layer == 0) yield break;

            float damagePercentage = 0;

            foreach (KeyValuePair<uint, float> pair in DamagePerLayer.Where(pair => layer >= pair.Key))
                damagePercentage = pair.Value;

            int damage = -Mathf.Max(Mathf.FloorToInt(battler.GetStats(battleManager)[Stat.Hp] * damagePercentage), 1);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                side,
                                                                battlerIndex,
                                                                damage,
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return DialogManager.ShowDialogAndWait("Status/Side/SpikesStatus/Effect",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer));
        }
    }
}