using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// RoughSkin ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/RoughSkin", fileName = "RoughSkin")]
    public class RoughSkin : Ability
    {
        /// <summary>
        /// Percentage to HP to recoil.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float RecoilPercentageOfMaxHP = 1f / 16f;

        /// <summary>
        /// Localization key for the recoil message.
        /// </summary>
        [SerializeField]
        private string RecoilMessage = "Abilities/RoughSkin/RecoilMessage";

        /// <summary>
        /// Recoil damage.
        /// </summary>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler owner,
                                                   Battler user,
                                                   int damageDealt,
                                                   uint previousHP,
                                                   bool wasCritical,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   int hitNumber,
                                                   int expectedMoveHits,
                                                   BattleManager battleManager)
        {
            if (!move.DoesMoveMakeContact(user, owner, battleManager, ignoresAbilities)) yield break;

            ShowAbilityNotification(owner);

            yield return DialogManager.ShowDialogAndWait(RecoilMessage,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                owner,
                                                                -(int) (user.GetStats(battleManager)[Stat.Hp]
                                                                      * RecoilPercentageOfMaxHP),
                                                                isSecondaryDamage: true);
        }
    }
}