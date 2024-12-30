using System;
using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Items module for the battle manager.
    /// </summary>
    public class BattleManagerItemsModule : BattleManagerModule<BattleManagerItemsModule>
    {
        /// <summary>
        /// Reference to the bags used by the allies.
        /// </summary>
        private List<Bag> allyBags;

        /// <summary>
        /// Reference to the bags used by the enemies.
        /// </summary>
        private List<Bag> enemyBags;

        /// <summary>
        /// Reference to the experience look up table.
        /// </summary>
        [Inject]
        private ExperienceLookupTable experienceLookupTable;

        /// <summary>
        /// Get the Bag corresponding to a roster.
        /// </summary>
        /// <param name="type">Type of battler we are looking for.</param>
        /// <param name="index">Index of the roster to look for.</param>
        /// <returns>The Bag that roster has.</returns>
        /// <exception cref="BattleManager.UnsupportedBattlerException">Thrown if we enter a not supported battler type.</exception>
        public Bag GetBag(BattlerType type, int index)
        {
            (int rosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, index);

            return type switch
            {
                BattlerType.Ally => allyBags?[rosterIndex],
                BattlerType.Enemy => enemyBags?[rosterIndex],
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };
        }

        /// <summary>
        /// Prepare the bags to be used in the battle.
        /// We clone the non player bags so that the assets never lose the items.
        /// </summary>
        /// <param name="parameters">The battle parameters.</param>
        // ReSharper disable once CyclomaticComplexity
        internal void PrepareBags(BattleParameters parameters)
        {
            allyBags = new List<Bag>();
            enemyBags = new List<Bag>();

            switch (BattleManager.BattleType)
            {
                // 1 trainer vs 1 trainer.
                case BattleType.SingleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 1:

                    allyBags.Add(BattleManager.PlayerBag);
                    enemyBags.Add(parameters.Bags[0].Clone());

                    break;

                case BattleType.SingleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 0:

                    allyBags.Add(BattleManager.PlayerBag);

                    break;

                case BattleType.SingleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 2:

                    allyBags.Add(parameters.Bags[0].Clone());
                    enemyBags.Add(parameters.Bags[1].Clone());

                    break;

                case BattleType.SingleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 1:

                    allyBags.Add(parameters.Bags[0].Clone());

                    break;

                // 1 trainer vs 2 trainers.
                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 2:

                    allyBags.Add(BattleManager.PlayerBag);
                    enemyBags.Add(parameters.Bags[0].Clone());
                    enemyBags.Add(parameters.Bags[1].Clone());

                    break;

                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 0:

                    allyBags.Add(BattleManager.PlayerBag);

                    break;

                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 3:

                    allyBags.Add(parameters.Bags[0].Clone());
                    enemyBags.Add(parameters.Bags[1].Clone());
                    enemyBags.Add(parameters.Bags[2].Clone());

                    break;

                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 1:

                    allyBags.Add(parameters.Bags[0].Clone());

                    break;

                // 1 trainer and a friend vs 2 trainers.
                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 3:

                    allyBags.Add(BattleManager.PlayerBag);
                    allyBags.Add(parameters.Bags[0].Clone());
                    enemyBags.Add(parameters.Bags[1].Clone());
                    enemyBags.Add(parameters.Bags[2].Clone());

                    break;

                case BattleType.DoubleBattle when AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 1:

                    allyBags.Add(BattleManager.PlayerBag);
                    allyBags.Add(parameters.Bags[0].Clone());

                    break;

                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Trainer
                                               && parameters.Bags.Length == 4:

                    allyBags.Add(parameters.Bags[0].Clone());
                    allyBags.Add(parameters.Bags[1].Clone());
                    enemyBags.Add(parameters.Bags[2].Clone());
                    enemyBags.Add(parameters.Bags[3].Clone());

                    break;

                case BattleType.DoubleBattle when !AI.PlayerControlsFirstRoster
                                               && parameters.EnemyType == EnemyType.Wild
                                               && parameters.Bags.Length == 2:

                    allyBags.Add(parameters.Bags[0].Clone());
                    allyBags.Add(parameters.Bags[1].Clone());

                    break;
                default:
                    throw new ArgumentException(parameters.BattleType
                                              + " against "
                                              + parameters.EnemyType
                                              + " with "
                                              + parameters.Bags.Length
                                              + " bags and player"
                                              + (AI.PlayerControlsFirstRoster ? "" : " not")
                                              + " controlling the first roster is not supported.");
            }
        }

        /// <summary>
        /// Use an item
        /// </summary>
        /// <param name="type">Type of battler using the item.</param>
        /// <param name="userInBattleIndex">In battle index of the user.</param>
        /// <param name="itemIndex">Index of the item.</param>
        public IEnumerator UseItem(BattlerType type,
                                   int userInBattleIndex,
                                   int itemIndex)
        {
            Battler user = Battlers.GetBattlerFromBattleIndex(type, userInBattleIndex);

            Bag bagToUse = GetBag(type, userInBattleIndex);

            Item itemToUse = bagToUse.GetItemFromIndex(itemIndex);

            if (itemToUse == null)
            {
                Logger.Error("There is no item with index " + itemIndex + ", won't do anything this turn!");
                yield break;
            }

            if (!user.CanUseBagItem(itemToUse, BattleManager))
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/Battle/CantUseBagItems",
                                                             switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: user
                                                                .GetNameOrNickName(BattleManager.Localizer));

                yield break;
            }

            (int userRosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, userInBattleIndex);

            if (AI.PlayerControlsFirstRoster && type == BattlerType.Ally && userRosterIndex == 0)
                DialogManager.ShowDialog("Items/UseItem/Player",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[] {BattleManager.Localizer[itemToUse.LocalizableName]},
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
            else
                DialogManager.ShowDialog("Items/UseItem/Other",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        Characters.GetCharacter(type, userInBattleIndex)
                                                                  .GetLocalizedFullName(BattleManager.Localizer),
                                                        BattleManager.Localizer[itemToUse.LocalizableName]
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            yield return DialogManager.WaitForDialog;

            bool consume = false;

            yield return itemToUse.UseInBattle(type,
                                               userInBattleIndex,
                                               BattleManager,
                                               BattleManager.Localizer,
                                               shouldConsume => consume = shouldConsume);

            if (consume) bagToUse.ChangeItemAmount(itemToUse, -1);
        }

        /// <summary>
        /// Use an item on a target.
        /// </summary>
        /// <param name="type">Type of battler using the item.</param>
        /// <param name="userInBattleIndex">In battle index of the user.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="targetType">Type of the target monster.</param>
        /// <param name="targetRosterIndex">Index of the target roster.</param>
        /// <param name="targetIndex">In roster index of the target.</param>
        public IEnumerator UseItemOnTarget(BattlerType type,
                                           int userInBattleIndex,
                                           int itemIndex,
                                           BattlerType targetType,
                                           int targetRosterIndex,
                                           int targetIndex)
        {
            Battler target = Battlers.GetBattlerFromRosterAndIndex(targetType, targetRosterIndex, targetIndex);

            (int userRosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, userInBattleIndex);

            Bag bagToUse = GetBag(type, userInBattleIndex);

            Item itemToUse = bagToUse.GetItemFromIndex(itemIndex);

            // Can't use and its the same trainer.
            if (!target.CanUseBagItem(itemToUse, BattleManager)
             && type == targetType
             && userRosterIndex == targetRosterIndex)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/Battle/CantUseBagItems",
                                                             switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: target
                                                                .GetNameOrNickName(BattleManager.Localizer));

                yield break;
            }

            if (itemToUse == null)
            {
                Logger.Error("There is no item with index " + itemIndex + ", won't do anything this turn!");
                yield break;
            }

            if (AI.PlayerControlsFirstRoster && type == BattlerType.Ally && userRosterIndex == 0)
                DialogManager.ShowDialog("Items/UseItemWithTarget/Player",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        BattleManager.Localizer[itemToUse.LocalizableName],
                                                        target.GetNameOrNickName(BattleManager.Localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
            else
                DialogManager.ShowDialog("Items/UseItem/Other",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        Characters.GetCharacter(type, userInBattleIndex)
                                                                  .GetLocalizedFullName(BattleManager.Localizer),
                                                        BattleManager.Localizer[itemToUse.LocalizableName]
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            yield return DialogManager.WaitForDialog;

            bool consume = false;

            yield return itemToUse.UseOnTarget(target,
                                               BattleManager,
                                               BattleManager.YAPUSettings,
                                               experienceLookupTable,
                                               BattleManager.Localizer,
                                               shouldConsume => consume = shouldConsume);

            if (consume) bagToUse.ChangeItemAmount(itemToUse, -1);
        }

        /// <summary>
        /// Use an item on a target move.
        /// </summary>
        /// <param name="type">Type of battler using the item.</param>
        /// <param name="userInBattleIndex">In battle index of the user.</param>
        /// <param name="moveIndex">Index of the target move.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="targetType">Type of the target monster.</param>
        /// <param name="targetRosterIndex">Index of the target roster.</param>
        /// <param name="targetIndex">In roster index of the target.</param>
        public IEnumerator UseItemOnTargetMove(BattlerType type,
                                               int userInBattleIndex,
                                               int moveIndex,
                                               int itemIndex,
                                               BattlerType targetType,
                                               int targetRosterIndex,
                                               int targetIndex)
        {
            Battler target = Battlers.GetBattlerFromRosterAndIndex(targetType, targetRosterIndex, targetIndex);

            (int userRosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, userInBattleIndex);

            Bag bagToUse = GetBag(type, userInBattleIndex);

            Item itemToUse = bagToUse.GetItemFromIndex(itemIndex);

            // Can't use and its the same trainer.
            if (!target.CanUseBagItem(itemToUse, BattleManager)
             && type == targetType
             && userRosterIndex == targetRosterIndex)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/Battle/CantUseBagItems",
                                                             switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: target
                                                                .GetNameOrNickName(BattleManager.Localizer));

                yield break;
            }

            if (AI.PlayerControlsFirstRoster && type == BattlerType.Ally && userRosterIndex == 0)
                DialogManager.ShowDialog("Items/UseItemWithTarget/Player",
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        BattleManager.Localizer[itemToUse.LocalizableName],
                                                        target.GetNameOrNickName(BattleManager.Localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
            else
                DialogManager.ShowDialog("Items/UseItem/Other",
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        Characters.GetCharacter(type, userInBattleIndex)
                                                                  .GetLocalizedFullName(BattleManager.Localizer),
                                                        BattleManager.Localizer[itemToUse.LocalizableName]
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            yield return DialogManager.WaitForDialog;

            bool consume = false;

            yield return itemToUse.UseOnTargetMove(BattleManager,
                                                   target,
                                                   moveIndex,
                                                   BattleManager.Localizer,
                                                   shouldConsume => consume = shouldConsume);

            if (consume) bagToUse.ChangeItemAmount(itemToUse, -1);
        }
    }
}