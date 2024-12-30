using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for a volatile status that overrides the ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/OverrideAbilityStatus",
                     fileName = "OverrideAbilityStatus")]
    public class OverrideAbilityStatus : VolatileStatus
    {
        /// <summary>
        /// Get the ability from the extra data passed?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        public bool GetFromExtraData;

        /// <summary>
        /// Override for the ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        [HideIf(nameof(GetFromExtraData))]
        private Ability Override;

        /// <summary>
        /// Show the ability dialogs?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool ShowDialogs;

        /// <summary>
        /// Override to use when the ability is received from the extra data.
        /// </summary>
        private readonly Dictionary<Battler, Ability> dynamicOverride = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            if (ShowDialogs) yield return base.OnAddStatus(battleManager, battler, extraData);

            if (GetFromExtraData) dynamicOverride[battler] = (Ability) extraData[0];
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// No message.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            if (GetFromExtraData) dynamicOverride.Remove(battler);

            if (ShowDialogs) yield return base.OnRemoveStatus(battleManager, battler, playAnimation);

            yield break;
        }

        /// <summary>
        /// Override the monsters ability.
        /// </summary>
        /// <param name="battler">Reference to the monster.</param>
        public override Ability OnGetAbility(Battler battler) => GetFromExtraData ? dynamicOverride[battler] : Override;
    }
}