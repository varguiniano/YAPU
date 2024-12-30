using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class for the hail weather.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Weather/Hail", fileName = "HailWeather")]
    public class HailWeather : RainyWeather
    {
        /// <summary>
        /// Types immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Abilities immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<Ability> ImmuneAbilities;

        /// <summary>
        /// Held items immune to this move.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        [FoldoutGroup("Immunities")]
        [SerializeField]
        protected List<Item> ImmuneHeldItems;

        /// <summary>
        /// Percentage of HP drain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrain = 1f / 16f;

        /// <summary>
        /// Localization key for the damage dialog.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        protected string DamageLocalizationKey;

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherTick(BattleManager battleManager)
        {
            yield return base.WeatherTick(battleManager);

            foreach (Battler battler in battleManager.Battlers.GetBattlersFighting()
                                                     .Where(battler => battler.CanBattle
                                                                    && !battler.IsOfAnyType(ImmuneTypes,
                                                                                battleManager.YAPUSettings)
                                                                    && !(battler.CanUseAbility(battleManager, false)
                                                                      && ImmuneAbilities.Contains(battler.GetAbility()))
                                                                    && (!battler.CanUseHeldItemInBattle(battleManager)
                                                                     || !ImmuneHeldItems.Contains(battler.HeldItem))))
            {
                yield return DialogManager.ShowDialogAndWait(DamageLocalizationKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                         .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                int amount =
                    Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) * HPDrain), 1);

                yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                    BattlerType.Ally,
                                                                    -2,
                                                                    -amount,
                                                                    isSecondaryDamage: true);
            }
        }

        /// <summary>
        /// Add the damage localization key.
        /// </summary>
        protected override void RefreshLocalizableNames()
        {
            base.RefreshLocalizableNames();
            DamageLocalizationKey = BaseLocalizationRoot + name + "/Damage";
        }
    }
}