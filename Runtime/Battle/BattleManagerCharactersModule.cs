using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Character module for the battle manager.
    /// </summary>
    public class BattleManagerCharactersModule : BattleManagerModule<BattleManagerCharactersModule>
    {
        /// <summary>
        /// Character data of the allies.
        /// </summary>
        private CharacterData[] allyCharacters;

        /// <summary>
        /// Character data of the enemies.
        /// </summary>
        internal CharacterData[] EnemyCharacters;

        /// <summary>
        /// Get the character owner of the given monster.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="battleIndex">In battle index.</param>
        /// <returns>The corresponding character data.</returns>
        internal CharacterData GetCharacter(BattlerType type, int battleIndex)
        {
            (int rosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(type, battleIndex);

            return type == BattlerType.Ally
                       ? allyCharacters[rosterIndex]
                       : EnemyCharacters[rosterIndex];
        }

        /// <summary>
        /// Setup the characters for this battle.
        /// </summary>
        /// <param name="parameters">Parameters to be used.</param>
        internal void SetupCharacters(BattleParameters parameters)
        {
            allyCharacters = new CharacterData[2];
            EnemyCharacters = new CharacterData[2];

            CharacterData[] characters = parameters.Characters;

            if (characters == null || characters.Length == 0)
            {
                Logger.Error("No characters were passed as parameters.");
                return;
            }

            allyCharacters[0] = characters[0];
            BattleManager.AllyTrainerSprites[0].SetMaterial(allyCharacters[0].CharacterType.BackSprite);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (BattleManager.BattleType)
            {
                case BattleType.SingleBattle when BattleManager.EnemyType == EnemyType.Trainer:

                    EnemyCharacters[0] = characters[1];
                    BattleManager.EnemyTrainerSprites[0].SetSprite(EnemyCharacters[0].CharacterType.FrontSprite);

                    break;
                case BattleType.DoubleBattle
                    when Rosters.AllyRosters.Count == 1 && BattleManager.EnemyType == EnemyType.Trainer:

                    EnemyCharacters[0] = characters[1];
                    BattleManager.EnemyTrainerSprites[0].SetSprite(EnemyCharacters[0].CharacterType.FrontSprite);
                    EnemyCharacters[1] = characters[2];
                    BattleManager.EnemyTrainerSprites[1].SetSprite(EnemyCharacters[1].CharacterType.FrontSprite);

                    break;
                case BattleType.DoubleBattle when Rosters.AllyRosters.Count == 2:

                    allyCharacters[1] = characters[1];
                    BattleManager.AllyTrainerSprites[1].SetMaterial(allyCharacters[1].CharacterType.BackSprite);

                    if (BattleManager.EnemyType == EnemyType.Trainer)
                    {
                        EnemyCharacters[0] = characters[2];
                        BattleManager.EnemyTrainerSprites[0].SetSprite(EnemyCharacters[0].CharacterType.FrontSprite);
                        EnemyCharacters[1] = characters[3];
                        BattleManager.EnemyTrainerSprites[1].SetSprite(EnemyCharacters[1].CharacterType.FrontSprite);
                    }

                    break;
            }
        }

        /// <summary>
        /// Give the player price money.
        /// </summary>
        public IEnumerator GivePlayerPriceMoney()
        {
            if (BattleManager.PlayerLost
             || !AI.PlayerControlsFirstRoster)
                yield break;

            uint money = 0;

            if (BattleManager.EnemyType == EnemyType.Trainer)
                for (int i = 0; i < Rosters.EnemyRosters.Count; i++)
                {
                    List<Battler> enemyRoster = Rosters.EnemyRosters[i];

                    money += (uint) (enemyRoster.Max(battler => battler.StatData.Level)
                                   * EnemyCharacters[i].CharacterType.PriceMoney);
                }

            foreach (SideStatus status in Statuses.GetSideStatuses(BattlerType.Ally).Select(slot => slot.Key))
                money += status.GetPriceMoney();

            money = Rosters.PlayerBattlersThatHaveFought.Where(battler => battler.CanUseHeldItemInBattle(BattleManager))
                           .Aggregate(money,
                                      (current, battler) =>
                                          (uint) (current
                                                * battler.HeldItem.GetMultiplierForPriceMoney()));

            if (money > 0)
                DialogManager.ShowDialog("Dialogs/WonMoney",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[] {money.ToString(), BattleManager.Localizer["Currency"]},
                                         switchToNextAfterSeconds: 1.5f);

            BattleManager.PlayerBag.Money += money;

            yield return DialogManager.WaitForDialog;
        }
    }
}