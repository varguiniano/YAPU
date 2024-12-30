using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that implements the Red Card effect.
    /// I say the Red Card specifically because that item has SO MANY MECHANIC EXCEPTIONS.
    /// I hate Gen 5.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/RedCardEffect", fileName = "RedCardEffect")]
    public class RedCardEffect : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Reference to the ingrain status.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private VolatileStatus IngrainStatus;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Move user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHitByMove(Item item,
                                                   DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            // ReSharper disable twice InlineTemporaryVariable
            Battler cardHolder = battler;
            Battler target = user;

            // TODO: Sheer Force.
            // TODO: Suction Cups.
            // TODO: Knock off, thief, covet, magician.
            // TODO: Pickpocket.
            // TODO: Emergency Exit, Wimp Out.

            // Do not trigger if
            // The holder has fainted.
            // The holder had a substitute.
            // The target is no longer fighting.
            if (!cardHolder.CanBattle
             || substituteTookHit
             || !battleManager.Battlers.GetBattlersFighting().Contains(target))
                yield break;

            (BattlerType cardHolderType, int cardHolderIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(cardHolder);

            (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            // No extra effect if sed against wilds.
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (targetType == BattlerType.Enemy && battleManager.EnemyType == EnemyType.Wild) yield break;

            // If the attacker has ingrain, consume the Red Card and do nothing, for some random drunk GameFreak reason.
            if (target.HasVolatileStatus(IngrainStatus))
            {
                finished.Invoke(true);
                yield break;
            }

            item.ShowItemNotification(battler, localizer);

            // In the case of being a single battle against a wild and the wild has a Red card, make the player flee.
            if (targetType == BattlerType.Ally
             && battleManager.EnemyType == EnemyType.Wild
             && battleManager.BattleType == BattleType.SingleBattle)
                yield return battleManager.Battlers.RunAway(targetType, targetIndex, true, true);

            // Used against trainers, force switch to the next available if there is.
            if (target.CanSwitch(battleManager,
                                 targetType,
                                 targetIndex,
                                 null,
                                 false,
                                 item,
                                 false,
                                 true))
            {
                int newBattlerIndex = -1;

                List<Battler> targetRoster = battleManager.Rosters.GetRoster(targetType, targetIndex);

                for (int i = 0; i < targetRoster.Count; ++i)
                {
                    Battler candidate = targetRoster[i];
                    if (!candidate.CanBattle || candidate == target) continue;

                    newBattlerIndex = i;
                    break;
                }

                if (newBattlerIndex != -1)
                    yield return battleManager.BattleManagerBattlerSwitch.SwitchBattler(targetType,
                        targetIndex,
                        newBattlerIndex);
                else
                {
                    finished.Invoke(false);
                    yield break;
                }
            }

            finished.Invoke(true);
        }
    }
}