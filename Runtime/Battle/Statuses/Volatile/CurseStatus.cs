using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the cursed volatile status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/CurseStatus", fileName = "Cursed")]
    public class CurseStatus : VolatileStatus
    {
        /// <summary>
        /// HP reduction of the curse.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPReduction = 1f / 8f;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the cursed audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return PlayCurseAnimation(battler, battleManager);

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return PlayCurseAnimation(battler, battleManager);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                -Mathf.Max((int)(MonsterMathHelper
                                                                                          .CalculateStat(battler,
                                                                                               Stat.Hp,
                                                                                               battleManager)
                                                                                     * HPReduction),
                                                                           1),
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return base.OnTickStatus(battleManager, battler);
        }

        /// <summary>
        /// Play the cursed animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator PlayCurseAnimation(Battler battler, BattleManager battleManager)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battler);

            BasicSpriteAnimation animation = Instantiate(AnimationPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return animation.PlayAnimation(battleManager.BattleSpeed);

            Destroy(animation.gameObject);
        }
    }
}