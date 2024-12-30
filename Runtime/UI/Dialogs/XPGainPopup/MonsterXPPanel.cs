using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.XPGainPopup
{
    /// <summary>
    /// Controller of the panel to show a monster's xp increase.
    /// </summary>
    public class MonsterXPPanel : WhateverBehaviour<MonsterXPPanel>
    {
        /// <summary>
        /// Sound to play on level up.
        /// </summary>
        [SerializeField]
        private AudioReference LevelUpSound;

        /// <summary>
        /// Reference to the monster icon.
        /// </summary>
        [SerializeField]
        private MonsterIcon Icon;

        /// <summary>
        /// Reference to the gender indicator.
        /// </summary>
        [SerializeField]
        private GenderIndicator Gender;

        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        private TMP_Text Name;

        /// <summary>
        /// Reference to the XP level.
        /// </summary>
        [SerializeField]
        private TMP_Text Level;

        /// <summary>
        /// Reference to the amount raised text.
        /// </summary>
        [SerializeField]
        private TMP_Text Amount;

        /// <summary>
        /// Reference to the xp bar.
        /// </summary>
        [SerializeField]
        private TweenableSlider XPBar;

        /// <summary>
        /// Reference to the level up text transform.
        /// </summary>
        [SerializeField]
        private Transform LevelUpText;

        /// <summary>
        /// Reference to the level hider.
        /// </summary>
        [SerializeField]
        private HidableUiElement LevelHider;

        /// <summary>
        /// Flag to know if it is animating.
        /// </summary>
        [ReadOnly]
        public bool Animating;

        /// <summary>
        /// Monster being displayed.
        /// </summary>
        private MonsterInstance monster;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Experience lookup table to use.
        /// </summary>
        [Inject]
        private ExperienceLookupTable experienceLookupTable;

        /// <summary>
        /// Callback for when the animation finishes.
        /// </summary>
        private Action animationFinished;

        /// <summary>
        /// Set the monster to display.
        /// </summary>
        /// <param name="newMonster">Monster to display.</param>
        public void SetMonster(MonsterInstance newMonster)
        {
            monster = newMonster;

            Icon.SetIcon(monster);

            Gender.Show(!monster.EggData.IsEgg);
            LevelHider.Show(!monster.EggData.IsEgg);

            Gender.SetGender(monster.PhysicalData.Gender);

            Name.SetText(monster.GetNameOrNickName(localizer));
            UpdateLevel(monster.StatData.Level);
            Amount.SetText("");

            SetBarValue(monster.StatData.CurrentLevelExperience,
                        (uint) monster.GetExperienceForNextLevel(experienceLookupTable));

            LevelUpText.localScale = Vector3.zero;
        }

        /// <summary>
        /// Update the level value.
        /// </summary>
        /// <param name="level">Level value.</param>
        public void UpdateLevel(byte level) => Level.SetText(level.ToString());

        /// <summary>
        /// Set the xp bar value without tweening.
        /// </summary>
        /// <param name="value">Value of the bar.</param>
        /// <param name="max">Max value.</param>
        public void SetBarValue(uint value, uint max) => XPBar.SetValue(1, value, max);

        /// <summary>
        /// Play the animation of the monster gaining xp and levels.
        /// </summary>
        /// <param name="amount">The amount of xp gained.</param>
        /// <param name="levelUps">Each time the monster leveled up.</param>
        /// <param name="finished">Event raised when finished.</param>
        public void PlayAnimation(uint amount, IEnumerable<MonsterInstance.LevelUpData> levelUps, Action finished) =>
            StartCoroutine(PlayAnimationRoutine(amount, levelUps, finished));

        /// <summary>
        /// Play the animation of the monster gaining xp and levels.
        /// </summary>
        /// <param name="amount">The amount of xp gained.</param>
        /// <param name="levelUps">Each time the monster leveled up.</param>
        /// <param name="finished">Event raised when finished.</param>
        private IEnumerator PlayAnimationRoutine(uint amount,
                                                 IEnumerable<MonsterInstance.LevelUpData> levelUps,
                                                 Action finished)
        {
            Animating = true;

            animationFinished = finished;

            Amount.SetText("+" + amount);

            Queue<MonsterInstance.LevelUpData> queue = new(levelUps);

            uint[] previousStats = new uint[6];
            uint[] newStats = new uint[6];

            bool previousStatsSet = false;
            bool didLevelUp = false;

            byte lastLevel = 0;

            while (queue.Count > 0)
            {
                int levelsLeft = queue.Count;

                MonsterInstance.LevelUpData level = queue.Dequeue();

                if (!previousStatsSet)
                {
                    foreach (Stat stat in Utils.GetAllItems<Stat>())
                        previousStats[(int) stat] = level.PreviousData[stat];

                    previousStatsSet = true;
                }

                foreach (Stat stat in Utils.GetAllItems<Stat>()) newStats[(int) stat] = level.NewData[stat];

                bool leveled = false;

                int fullXP = experienceLookupTable.GetExperienceNeededForNextLevel(monster.FormData.GrowthRate,
                    level.Level - 1);

                XPBar.SetValue(levelsLeft,
                               fullXP,
                               fullXP,
                               true,
                               () =>
                               {
                                   AudioManager.Instance.PlayAudio(LevelUpSound);
                                   Level.SetText(level.Level.ToString());
                                   leveled = true;
                               });

                yield return new WaitUntil(() => leveled);

                LevelUpText.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);

                XPBar.SetValue(1,
                               0,
                               experienceLookupTable.GetExperienceNeededForNextLevel(monster.FormData.GrowthRate,
                                   level.Level + 1));

                yield return new WaitForSeconds(1f / levelsLeft);

                didLevelUp = true;
                lastLevel = level.Level;

                if (lastLevel == 100) break;
            }

            if (!didLevelUp || lastLevel != 100)
            {
                bool lastFinished = false;

                XPBar.SetValue(1,
                               monster.StatData.CurrentLevelExperience,
                               monster.GetExperienceForNextLevel(experienceLookupTable),
                               true,
                               () => lastFinished = true);

                yield return new WaitUntil(() => lastFinished);
            }

            Animating = false;

            if (!didLevelUp)
            {
                AfterPanelDismissed();
                yield break;
            }

            yield return DialogManager.WaitForDialog;

            DialogManager.OnNextDialog += AfterPanelDismissed;

            DialogManager.ShowStatsUpPanel(monster, previousStats, newStats);

            yield return DialogManager.WaitForDialog;

            DialogManager.HideStatsUpPanel();
        }

        /// <summary>
        /// Called after the stats panel is dismissed.
        /// </summary>
        private void AfterPanelDismissed()
        {
            DialogManager.OnNextDialog -= AfterPanelDismissed;
            animationFinished?.Invoke();
            animationFinished = null;
        }
    }
}