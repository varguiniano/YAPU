using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Sleep monster status.
    /// TODO: Slow down animation when asleep.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Sleep", fileName = "Sleep")]
    public class Sleep : Status
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect AnimationPrefab;

        /// <summary>
        /// Reference to the asleep audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Min number of turns to stay asleep when random calculating.
        /// </summary>
        [FoldoutGroup("Status")]
        [SerializeField]
        private byte MinTurns;

        /// <summary>
        /// Max number of turns to stay asleep when random calculating.
        /// </summary>
        [FoldoutGroup("Status")]
        [SerializeField]
        private byte MaxTurns;

        /// <summary>
        /// Moves that bypass this status and can be used.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Status")]
        [SerializeField]
        private List<Move> BypassMoves;

        /// <summary>
        /// Countdown of the effect per battler.
        /// </summary>
        private readonly Dictionary<Battler, int> countdownPerBattler = new();

        /// <summary>
        /// Called when the status is added in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ignoresAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        /// <param name="extraData"> [0]: Override for the countdown counter, -1 is infinite.</param>
        public override IEnumerator OnStatusAddedInBattle(Battler battler,
                                                          BattleManager battleManager,
                                                          bool ignoresAbilities,
                                                          bool showMessage = true,
                                                          params object[] extraData)
        {
            int countdown;

            if (extraData.Length == 1)
            {
                int countdownOverride = (int) extraData[0];
                countdown = countdownOverride;
            }
            else
                countdown = battleManager.RandomProvider.Range(MinTurns, MaxTurns + 1);

            countdown = (int) (countdown * battler.CalculateSleepCountDownMultiplier(battleManager, ignoresAbilities));

            SetCountdown(battleManager.Localizer, battler, countdown);

            yield return PlayAnimation(battler, battleManager);

            yield return base.OnStatusAddedInBattle(battler, battleManager, ignoresAbilities, showMessage);

            if (countdown == 0) yield return battleManager.Statuses.RemoveStatus(battler);
        }

        /// <summary>
        /// No need to show message.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusTickInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool showMessage = true)
        {
            yield return base.OnStatusTickInBattle(battler, battleManager, false);
        }

        /// <summary>
        /// Remove the counter from the dictionary.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusRemovedInBattle(Battler battler,
                                                            BattleManager battleManager,
                                                            bool showMessage = true)
        {
            countdownPerBattler.Remove(battler);

            return base.OnStatusRemovedInBattle(battler, battleManager, showMessage);
        }

        /// <summary>
        /// Called when the battler has started.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleStarted(BattleManager battleManager, Battler battler)
        {
            SetCountdown(battleManager.Localizer, battler, battleManager.RandomProvider.Range(MinTurns, MaxTurns + 1));

            return base.OnBattleStarted(battleManager, battler);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            countdownPerBattler.Remove(battler);

            return base.OnBattleEnded(battler);
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToPerformMove(Battler battler,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Action<bool> finished)
        {
            if (countdownPerBattler[battler] == 0)
            {
                yield return battleManager.Statuses.RemoveStatus(battler);
                finished.Invoke(true);
            }
            else
            {
                yield return PlayAnimation(battler, battleManager);

                yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                countdownPerBattler[battler]--;

                finished.Invoke(BypassMoves.Contains(move));
            }
        }

        /// <summary>
        /// Set the countdown for a battler.
        /// </summary>
        private void SetCountdown(ILocalizer localizer, Battler battler, int countdown)
        {
            Logger.Info(battler.GetNameOrNickName(localizer) + " will be asleep for " + countdown + " turns.");

            countdownPerBattler[battler] = countdown;
        }

        /// <summary>
        /// Play the asleep animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator PlayAnimation(Battler battler, BattleManager battleManager)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battler);

            VisualEffect animation = Instantiate(AnimationPrefab, sprite.Pivot);

            animation.enabled = true;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return new WaitForSeconds(1.6f / battleManager.BattleSpeed);

            animation.Stop();

            DOVirtual.DelayedCall(3, () => Destroy(animation.gameObject));
        }
    }
}