using BattleTech;
using Harmony;
using Timeline.Features;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(ItemCollectionResultGenerator), "GenerateItemCollection")]
    public static class ItemCollectionResultGenerator_GenerateItemCollection_Patch
    {
        public static void Prefix(ref ItemCollectionDef collection)
        {
            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            collection = ItemCollectionRequirements.FilterItemCollectionDef(collection, simGame);
        }
    }
}
