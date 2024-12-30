using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu
{
    /// <summary>
    /// Controller of a monster button belonging in the storage.
    /// </summary>
    public class StorageMonsterButton : MonsterButtonWithCompatibilityPanel
    {
        /// <summary>
        /// Extenject factory.
        /// </summary>
        public class Factory : GameObjectFactory<StorageMonsterButton>
        {
        }
    }
}