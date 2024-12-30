using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// General animations module for the battle manager.
    /// </summary>
    public class BattleManagerAnimationModule : BattleManagerModule<BattleManagerAnimationModule>
    {
        /// <summary>
        /// Animation to play at the start of the battle.
        /// </summary>
        internal IEnumerator BattleStartAnimation()
        {
            if (BattleManager.EnemyType == EnemyType.Wild) Audio.PlayCries(BattlerType.Enemy);
            if (BattleManager.EnemyType == EnemyType.Wild) Audio.PlayCries(BattlerType.Enemy);

            switch (BattleManager.EnemyType)
            {
                case EnemyType.Wild:

                    foreach (Battler battler in Battlers.GetBattlersFighting(BattlerType.Enemy))
                    {
                        BattleManager.Dex.RegisterAsSeen(battler, true, false);

                        DialogManager.ShowDialog(battler.OriginData.IsAlpha
                                                     ? "Battle/Intro/Single/Alpha"
                                                     : "Battle/Intro/Single/Wild",
                                                 acceptInput: false,
                                                 modifiers: battler.Species.LocalizableName,
                                                 switchToNextAfterSeconds: 2f / BattleManager.BattleSpeed);
                    }

                    break;
                case EnemyType.Trainer when BattleManager.BattleType == BattleType.SingleBattle:
                    DialogManager.ShowDialog("Battle/Intro/Single/Trainer",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: Characters.EnemyCharacters[0]
                                                                  .GetLocalizedFullName(BattleManager.Localizer),
                                             switchToNextAfterSeconds: 2f / BattleManager.BattleSpeed);

                    break;

                case EnemyType.Trainer when BattleManager.BattleType == BattleType.DoubleBattle:
                    DialogManager.ShowDialog("Battle/Intro/Double/Trainer",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            Characters.EnemyCharacters[0]
                                                                      .GetLocalizedFullName(BattleManager.Localizer),
                                                            Characters.EnemyCharacters[1]
                                                                      .GetLocalizedFullName(BattleManager.Localizer)
                                                        },
                                             switchToNextAfterSeconds: 2f / BattleManager.BattleSpeed);

                    break;

                default: Logger.Error("Enemy type " + BattleManager.EnemyType + " is not implemented."); break;
            }

            yield return new WaitForSeconds(2f / BattleManager.BattleSpeed);

            BattleManager.BattleCamera.ReturnToMain(BattleManager.BattleSpeed);
            yield return WaitAFrame;
            yield return WaitAFrame;

            yield return BattleManager.WaitUntilCameraStopped;

            yield return DialogManager.WaitForDialog;

            if (BattleManager.EnemyType == EnemyType.Trainer)
                yield return BattleManagerBattlerSwitch.SendBattlersIn(BattlerType.Enemy);
            else
                yield return BattleManagerBattlerSwitch.RegisterWildsAsInField();
        }

        /// <summary>
        /// Update all the panels.
        /// </summary>
        public void UpdatePanels()
        {
            foreach (MonsterPanel panel in BattleManager.AllyPanels) panel.UpdatePanel(1, false);
            foreach (MonsterPanel panel in BattleManager.EnemyPanels) panel.UpdatePanel(1, false);
        }

        /// <summary>
        /// Play a form switch animation for a battler.
        /// </summary>
        /// <param name="battlerType">Battler type.</param>
        /// <param name="battleIndex">Battler index.</param>
        public IEnumerator PlayFormChangeAnimation(BattlerType battlerType, int battleIndex)
        {
            yield return BattleManager.GetMonsterSprite(battlerType, battleIndex)
                                      .FXAnimator.PlayFormChange(BattleManager.BattleSpeed);

            Battlers.UpdateMonsterSprite(battlerType, battleIndex);
            Audio.PlayCry(battlerType, battleIndex);
        }

        /// <summary>
        /// Play the battle end animation.
        /// </summary>
        public IEnumerator BattleEndAnimation()
        {
            bool playerWon = Rosters.AreAllBattlersOfTypeFainted(BattlerType.Enemy)
                          || Capture.CapturedMonster != null;

            BattleManager.EnemyRosterIndicators[0].Show(false);
            BattleManager.EnemyRosterIndicators[1].Show(false);

            foreach (MonsterPanel panel in BattleManager.AllyPanels) panel.SlideOut();
            foreach (MonsterPanel panel in BattleManager.EnemyPanels) panel.SlideOut();
            
            // Make sure all animations have finished.
            yield return new WaitForSeconds(3f);

            // In case of having run away, no animation is necessary.
            if (BattleManager.DidRunAway) yield break;

            if (playerWon)
                yield return PlayerWonAnimation();
            else
                yield return PlayerLostAnimation();
        }

        /// <summary>
        /// Animation to play when the player won.
        /// </summary>
        private IEnumerator PlayerWonAnimation()
        {
            WhateverDevs.TwoDAudio.Runtime.AudioManager.Instance.StopAllAudios();

            switch (BattleManager.EnemyType)
            {
                case EnemyType.Wild:
                    WhateverDevs.TwoDAudio.Runtime.AudioManager.Instance.PlayAudio(Audio.WildWinMusic, true);
                    break;
                case EnemyType.Trainer:
                    WhateverDevs.TwoDAudio.Runtime.AudioManager.Instance.PlayAudio(Audio.TrainerWinMusic, true);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(1f / BattleManager.BattleSpeed);

            string message;
            List<string> modifiers = new();

            switch (BattleManager.EnemyType)
            {
                case EnemyType.Wild:
                    if (Capture.CapturedMonster == null)
                    {
                        message = "Battle/Win/Wild";

                        // TODO: Double battle.
                        modifiers.Add(Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 0)
                                              .GetNameOrNickName(BattleManager.Localizer));
                    }
                    else
                    {
                        message = "Battle/Capture/Success";
                        modifiers.Add(Capture.CapturedMonster.GetNameOrNickName(BattleManager.Localizer));
                    }

                    break;
                case EnemyType.Trainer:

                    BattleManager.BattleCamera.FocusOnEnemy(BattleManager.BattleSpeed);

                    int numberOfEnemies = BattleManager.BattleType switch
                    {
                        BattleType.SingleBattle => 1,
                        BattleType.DoubleBattle => 2,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        BattleTrainerSprite enemy = BattleManager.EnemyTrainerSprites[i];
                        enemy.SlideBack(BattleManager.BattleSpeed);
                        modifiers.Add(Characters.EnemyCharacters[i].GetLocalizedFullName(BattleManager.Localizer));
                    }

                    message = BattleManager.BattleType switch
                    {
                        BattleType.SingleBattle => "Battle/Win/Single/Trainer",
                        BattleType.DoubleBattle => "Battle/Win/Double/Trainer",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            yield return DialogManager.ShowDialogAndWait(message,
                                                         localizableModifiers: false,
                                                         modifiers: modifiers.ToArray(),
                                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            if (BattleManager.EnemyType == EnemyType.Trainer)
                for (int i = 0; i < Characters.EnemyCharacters.Length; ++i)
                {
                    if (Characters.EnemyCharacters[i] == null) continue;

                    yield return DialogManager.ShowDialogAndWait(BattleManager.EnemyTrainersAfterBattleDialogKeys[i],
                                                                 Characters.EnemyCharacters[i],
                                                                 switchToNextAfterSeconds: 1.5f);
                }
            else
            {
                if (Capture.CapturedMonster != null && BattleManager.PlayerSettings.CatchingYieldsXP)
                    yield return Battlers.YieldXPAndEV(Capture.CapturedMonster, false);
            }
        }

        /// <summary>
        /// Animation to play when the player lost.
        /// </summary>
        private IEnumerator PlayerLostAnimation()
        {
            yield return DialogManager.ShowDialogAndWait("Battle/Lost",
                                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
        }
    }
}