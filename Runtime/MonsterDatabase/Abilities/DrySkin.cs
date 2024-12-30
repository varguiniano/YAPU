using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability DrySkin.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/DrySkin", fileName = "DrySkin")]
    public class DrySkin : Ability
    {
        /// <summary>
        /// Types that heal instead of doing their effect when hitting this monster.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> HPHealingTypes;

        /// <summary>
        /// Types that deal increased damage to this monster.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> IncreasedDamageTypes;

        /// <summary>
        /// Weathers that heal this monster.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Weather, float> WeatherHPIncrements;

        /// <summary>
        /// Weathers that damage this monster.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Weather, float> WeathersThatDamage;

        /// <summary>
        /// Replace the move's effect when for healing.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public override IEnumerator ShouldReplaceMoveEffectWhenHit(Battler owner,
                                                                   Move move,
                                                                   Battler user,
                                                                   BattleManager battleManager,
                                                                   Action<bool> callback)
        {
            MonsterType moveType = move.GetMoveTypeInBattle(user, battleManager);

            if (HPHealingTypes.SerializedList.All(slot => slot.Key != moveType) || owner == user)
            {
                callback.Invoke(true);
                yield break;
            }

            if (!owner.CanHeal(battleManager))
            {
                callback.Invoke(false);
                yield break;
            }

            ShowAbilityNotification(owner);

            battleManager.GetMonsterSprite(owner).FXAnimator.PlayBoost(battleManager.BattleSpeed);

            int hpToHeal = (int) (owner.GetStats(battleManager)[Stat.Hp] * HPHealingTypes[moveType]);

            yield return battleManager.BattlerHealth.ChangeLife(owner, owner, hpToHeal);

            battleManager.Animation.UpdatePanels();

            callback.Invoke(false);
        }

        /// <summary>
        /// Called when calculating a move's damage on itself.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                      float multiplier,
                                                                      Battler user,
                                                                      Battler target,
                                                                      BattleManager battleManager,
                                                                      Action<float> finished)
        {
            MonsterType moveType = move.GetMoveTypeInBattle(user, battleManager);

            if (IncreasedDamageTypes.SerializedList.All(slot => slot.Key != moveType) || target == user)
            {
                finished.Invoke(multiplier);
                yield break;
            }

            ShowAbilityNotification(target);

            finished.Invoke(multiplier * IncreasedDamageTypes[moveType]);
        }

        /// <summary>
        /// Called once after each turn.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public override IEnumerator AfterTurnPostStatus(Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer)
        {
            if (!battleManager.Scenario.GetWeather(out Weather weather)) yield break;

            if (WeathersThatDamage.SerializedList.Any(slot => slot.Key == weather))
            {
                ShowAbilityNotification(battler);

                (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

                yield return DialogManager.ShowDialogAndWait("Abilities/BoostStatOnWeatherAndLoseHPAbility/Hit",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(localizer),
                                                                            localizer[LocalizableName]
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                    type,
                                                                    index,
                                                                    -(int) (battler.GetStats(battleManager)[Stat.Hp]
                                                                          * WeathersThatDamage[weather]),
                                                                    isSecondaryDamage: true);
            }
            else if (WeatherHPIncrements.SerializedList.Any(slot => slot.Key == weather))
            {
                if (!battler.CanHeal(battleManager)) yield break;

                ShowAbilityNotification(battler);

                (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

                battleManager.GetMonsterSprite(battler).FXAnimator.PlayBoost(battleManager.BattleSpeed);

                yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                    type,
                                                                    index,
                                                                    (int) (battler.GetStats(battleManager)[Stat.Hp]
                                                                         * WeatherHPIncrements[weather]),
                                                                    isSecondaryDamage: true);
            }
        }
    }
}