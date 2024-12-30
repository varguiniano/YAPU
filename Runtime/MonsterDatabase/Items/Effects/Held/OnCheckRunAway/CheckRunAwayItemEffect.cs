using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCheckRunAway
{
    /// <summary>
    /// Data class for an item effect that can modify if the battler holding it can run away.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/CheckRunAwayItemEffect", fileName = "CheckRunAwayItemEffect")]
    public class CheckRunAwayItemEffect : MonsterDatabaseScriptable<CheckRunAwayItemEffect>
    {
        /// <summary>
        /// Can the battler run away?
        /// </summary>
        [SerializeField]
        private bool CanRunAway;

        /// <summary>
        /// Override all other effects and always can?
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(CanRunAway))]
        private bool OverrideAllOtherEffects;

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="battler">Battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Show explanation messages?</param>
        /// <returns>If it can run away, and if it overrides all other effects preventing run away.</returns>
        public (bool, bool)
            CheckCanRunAway(Item item, Battler battler, BattleManager battleManager, bool showMessages) =>
            (CanRunAway, CanRunAway && OverrideAllOtherEffects);

        /// <summary>
        /// Called when the battler runs away.
        /// </summary>
        public IEnumerator OnRunAway(Battler owner, Item item, BattleManager battleManager)
        {
            item.ShowItemNotification(owner, battleManager.Localizer);
            yield break;
        }
    }
}