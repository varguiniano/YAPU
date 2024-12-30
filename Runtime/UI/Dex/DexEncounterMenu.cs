using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Menu that displays encounter data in the dex.
    /// </summary>
    public class DexEncounterMenu : VirtualizedMenuSelector<(SceneInfoAsset, EncounterType, EncounterSetDexData),
        DexEncounterButton, DexEncounterButton.Factory>
    {
        /// <summary>
        /// Fill the data.
        /// </summary>
        protected override void PopulateChildData(DexEncounterButton child,
                                                  (SceneInfoAsset, EncounterType, EncounterSetDexData) childData) =>
            child.UpdateData(childData.Item1, childData.Item2, childData.Item3);
    }
}