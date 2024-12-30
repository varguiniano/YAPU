using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the PerishSong status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/PerishSong", fileName = "PerishSong")]
    public class PerishSongStatus : VolatileStatus
    {
        /// <summary>
        /// Starting counter for the perish song.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int StartingCounter = 4;

        /// <summary>
        /// Dictionary that keeps track of perish song counters.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        private readonly Dictionary<Battler, int> perishCounters = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need. 0 must be if affected by binding band.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            // No dialog.

            if (!perishCounters.TryAdd(battler, StartingCounter)) yield break;
        }

        /// <summary>
        /// Have the vortex damage the mon.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            perishCounters[battler]--;

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        perishCounters[battler].ToString()
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            if (perishCounters[battler] == 0)
                yield return battleManager.BattlerHealth.ChangeLife(battler, battler, -(int) battler.CurrentHP);
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            // No dialog.

            perishCounters.Remove(battler);

            yield break;
        }

        /// <summary>
        /// Called when this status is passed from one battler to another.
        /// For example with Baton Pass.
        /// </summary>
        /// <param name="oldOwner">Old owner of the status.</param>
        /// <param name="newOwner">New owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override void OnMonsterChanged(Battler oldOwner, Battler newOwner, BattleManager battleManager)
        {
            base.OnMonsterChanged(oldOwner, newOwner, battleManager);

            perishCounters[newOwner] = perishCounters[oldOwner];
            perishCounters.Remove(oldOwner);
        }

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            perishCounters.Clear();

            yield return base.OnBattleEnded(battler);
        }
    }
}