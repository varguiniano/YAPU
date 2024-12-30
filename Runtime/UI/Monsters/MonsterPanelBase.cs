using System;
using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Base class for monster panels.
    /// </summary>
    public abstract class MonsterPanelBase : WhateverBehaviour<MonsterPanelBase>
    {
        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected TMP_Text NameText;

        /// <summary>
        /// Reference to the level text.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected TMP_Text LevelText;

        /// <summary>
        /// Reference to the level hider.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected HidableUiElement LevelHider;

        /// <summary>
        /// Reference to the status indicator.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected StatusIndicator StatusIndicator;

        /// <summary>
        /// Reference to the gender indicator.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected GenderIndicator GenderIndicator;

        /// <summary>
        /// Reference to the health slider.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        public HealthSlider HealthSlider;

        /// <summary>
        /// Flag to know if the panel is currently updating.
        /// </summary>
        public bool Updating { get; private set; }

        /// <summary>
        /// Reference to the text that displays the health.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected TMP_Text HealthText;

        /// <summary>
        /// Reference to the xp slider.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected TweenableSlider ExperienceSlider;

        /// <summary>
        /// Reference to the hider for the health and the exp.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        protected HidableUiElement BarsHider;

        /// <summary>
        /// Reference to the current monster.
        /// </summary>
        private MonsterInstance monster;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the experience lookup table.
        /// </summary>
        [Inject]
        private ExperienceLookupTable experienceLookupTable;

        /// <summary>
        /// Set the monster to use in this panel.
        /// </summary>
        /// <param name="monsterReference">Reference to the monster to use.</param>
        /// <param name="playLowHealthSound">Play the low health sound?</param>
        public void SetMonster(MonsterInstance monsterReference, bool playLowHealthSound = true)
        {
            monster = monsterReference;

            UpdatePanel(1, playLowHealthSound);
        }

        /// <summary>
        /// Get a reference to the monster in the panel.
        /// </summary>
        public MonsterInstance GetMonster() => monster;

        /// <summary>
        /// Updates the data in the panel.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="playLowHealthSound">Play the low health sound?</param>
        /// </summary>
        public virtual void UpdatePanel(float speed,
                                        bool playLowHealthSound = true,
                                        bool tween = false,
                                        Action finished = null) =>
            CoroutineRunner.RunRoutine(UpdatePanelRoutine(speed, playLowHealthSound, tween, finished));

        /// <summary>
        /// Updates the data in the panel.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="playLowHealthSound">Play the low health sound?</param>
        /// </summary>
        private IEnumerator UpdatePanelRoutine(float speed,
                                               bool playLowHealthSound = true,
                                               bool tween = false,
                                               Action finished = null)
        {
            yield return new WaitWhile(() => Updating);

            Updating = true;

            if (monster?.IsNullEntry != false)
            {
                Updating = false;
                finished?.Invoke();
                yield break;
            }

            NameText.SetText(monster.GetNameOrNickName(localizer));

            if (LevelText != null)
            {
                LevelText.SetText(monster.StatData.Level.ToString());
                LevelHider.Show(!monster.EggData.IsEgg);
            }

            if (GenderIndicator != null)
            {
                GenderIndicator.SetGender(monster.PhysicalData.Gender);
                GenderIndicator.Show(!monster.EggData.IsEgg);
            }

            if (HealthSlider != null)
            {
                uint maxHP = MonsterMathHelper.CalculateStat(monster, Stat.Hp, null);

                HealthSlider.PlayWarningAudio = playLowHealthSound;

                bool healthFinished = false;

                HealthSlider.SetValue(speed, monster.CurrentHP, maxHP, tween, () => healthFinished = true);

                yield return new WaitUntil(() => healthFinished);

                UpdateHealthText(monster.CurrentHP, maxHP);
            }
            
            if (StatusIndicator != null)
            {
                StatusIndicator.UpdateStatus(monster);
                StatusIndicator.Show(!monster.EggData.IsEgg);
            }

            if (ExperienceSlider != null)
                ExperienceSlider.SetValue(speed,
                                          monster.StatData.CurrentLevelExperience,
                                          monster.GetExperienceForNextLevel(experienceLookupTable));

            if (BarsHider != null) BarsHider.Show(!monster.EggData.IsEgg);

            finished?.Invoke();
            Updating = false;
        }

        /// <summary>
        /// Update the health text of the slider.
        /// </summary>
        public void UpdateHealthText(uint hp, uint maxHP)
        {
            if (HealthText != null) HealthText.SetText(hp + "/" + maxHP);
        }
    }
}