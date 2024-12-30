using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing a status that makes the monster always land the next attack against a target.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/AlwaysHitNextTurnAgainstTarget",
                     fileName = "AlwaysHitNextTurnAgainstTarget")]
    public class AlwaysHitNextTurnAgainstTarget : VolatileStatus
    {
        /// <summary>
        /// Dictionary that keeps track of the monster focusing and their target.
        /// </summary>
        private readonly Dictionary<Battler, Battler> targets = new();
        
        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">[0] target.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            targets[battler] = (Battler) extraData[0];

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        targets[battler]
                                                                           .GetNameOrNickName(battleManager
                                                                               .Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// No message.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            targets.Remove(battler);
            
            yield break;
        }
        
        /// <summary>
        /// Does this status bypass all accuracy checks when using a move?
        /// </summary>
        public override bool DoesBypassAllAccuracyChecksWhenUsing(Battler owner,
                                                                  Move move,
                                                                  Battler target,
                                                                  BattleManager battleManager)
        {
            if (!targets.TryGetValue(owner, out Battler focusTarget)) return false;
            
            return focusTarget == target;
        }
    }
}