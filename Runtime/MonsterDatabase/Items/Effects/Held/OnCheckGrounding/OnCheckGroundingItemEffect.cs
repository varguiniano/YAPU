using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCheckGrounding
{
    /// <summary>
    /// Data class for an item effect that can modify if the battler holding it is grounded.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/CheckGroundingItemEffect", fileName = "CheckGroundingItemEffect")]
    public class OnCheckGroundingItemEffect : MonsterDatabaseScriptable<OnCheckGroundingItemEffect>
    {
        /// <summary>
        /// Does this effect prevent grounding?
        /// </summary>
        public bool PreventsGrounding;

        /// <summary>
        /// Does this effect force grounding?
        /// </summary>
        public bool ForcesGrounding;
    }
}