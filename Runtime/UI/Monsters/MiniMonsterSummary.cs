using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Items;
using Varguiniano.YAPU.Runtime.UI.Moves;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Behaviour that controls the summary of the hovered monster when in the storage screen.
    /// </summary>
    public class MiniMonsterSummary : WhateverBehaviour<MiniMonsterSummary>
    {
        /// <summary>
        /// Reference to the shower containing all egg info.
        /// </summary>
        [SerializeField]
        private HidableUiElement EggInfo;

        /// <summary>
        /// Reference to the shower containing all non egg info.
        /// </summary>
        [SerializeField]
        private HidableUiElement NonEggInfo;

        /// <summary>
        /// Reference to the ball icon.
        /// </summary>
        [SerializeField]
        private ItemIcon Ball;

        /// <summary>
        /// Reference to the monster name.
        /// </summary>
        [SerializeField]
        private TMP_Text Name;

        /// <summary>
        /// Reference to the status indicator.
        /// </summary>
        [SerializeField]
        private StatusIndicator Status;

        /// <summary>
        /// Reference to the gender indicator.
        /// </summary>
        [SerializeField]
        private GenderIndicator Gender;

        /// <summary>
        /// Reference to the level.
        /// </summary>
        [SerializeField]
        private TMP_Text Level;

        /// <summary>
        /// Reference to the dex number.
        /// </summary>
        [SerializeField]
        private TMP_Text DexNumber;

        /// <summary>
        /// Reference to the species.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Species;

        /// <summary>
        /// Reference to the first type.
        /// </summary>
        [SerializeField]
        private TypeBadge Type0;

        /// <summary>
        /// Reference to the second type.
        /// </summary>
        [SerializeField]
        private TypeBadge Type1;

        /// <summary>
        /// Reference to the HP stat.
        /// </summary>
        [SerializeField]
        private TMP_Text HP;

        /// <summary>
        /// Reference to the Attack stat.
        /// </summary>
        [SerializeField]
        private TMP_Text Attack;

        /// <summary>
        /// Reference to the Defence stat.
        /// </summary>
        [SerializeField]
        private TMP_Text Defence;

        /// <summary>
        /// Reference to the SpAttack stat.
        /// </summary>
        [SerializeField]
        private TMP_Text SpAttack;

        /// <summary>
        /// Reference to the SpDefence stat.
        /// </summary>
        [SerializeField]
        private TMP_Text SpDefence;

        /// <summary>
        /// Reference to the Speed stat.
        /// </summary>
        [SerializeField]
        private TMP_Text Speed;

        /// <summary>
        /// Reference to the ability.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Ability;

        /// <summary>
        /// Reference to the held item.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro HeldItem;

        /// <summary>
        /// Reference to the first move.
        /// </summary>
        [SerializeField]
        private MoveButton Move0;

        /// <summary>
        /// Reference to the second move.
        /// </summary>
        [SerializeField]
        private MoveButton Move1;

        /// <summary>
        /// Reference to the third move.
        /// </summary>
        [SerializeField]
        private MoveButton Move2;

        /// <summary>
        /// Reference to the forth move.
        /// </summary>
        [SerializeField]
        private MoveButton Move3;

        /// <summary>
        /// Description of the egg.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro EggDescription;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Set the monster to display.
        /// </summary>
        /// <param name="monster">Set the monster.</param>
        public void SetMonster(MonsterInstance monster)
        {
            EggInfo.Show(monster.EggData.IsEgg);
            NonEggInfo.Show(!monster.EggData.IsEgg);

            Ball.SetIcon(monster.OriginData.Ball);
            Name.SetText(monster.GetNameOrNickName(localizer));
            Status.UpdateStatus(monster);
            Gender.SetGender(monster.PhysicalData.Gender);
            Level.SetText(monster.StatData.Level.ToString());
            DexNumber.SetText(monster.Species.DexNumber.ToString());
            Species.SetValue(monster.Species.LocalizableName);

            (MonsterType firstType, MonsterType secondType) = monster.GetTypes(settings);
            Type0.SetType(firstType);
            Type1.SetType(secondType);

            Dictionary<Stat, uint> stat = monster.GetStats(null);

            HP.SetText(stat[Stat.Hp].ToString());
            Attack.SetText(stat[Stat.Attack].ToString());
            Defence.SetText(stat[Stat.Defense].ToString());
            SpAttack.SetText(stat[Stat.SpecialAttack].ToString());
            SpDefence.SetText(stat[Stat.SpecialDefense].ToString());
            Speed.SetText(stat[Stat.Speed].ToString());

            Ability.SetValue(monster.GetAbility().LocalizableName);
            HeldItem.SetValue(monster.HeldItem != null ? monster.HeldItem.LocalizableName : "-");

            Move0.SetMove(monster.CurrentMoves[0], monster, false, showDisabledIfCantUse: false);
            Move1.SetMove(monster.CurrentMoves[1], monster, false, showDisabledIfCantUse: false);
            Move2.SetMove(monster.CurrentMoves[2], monster, false, showDisabledIfCantUse: false);
            Move3.SetMove(monster.CurrentMoves[3], monster, false, showDisabledIfCantUse: false);

            EggDescription.SetValue(MonsterMathHelper.GetEggCyclesLocalizationString(monster));
        }
    }
}