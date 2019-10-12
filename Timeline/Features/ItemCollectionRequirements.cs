using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace Timeline.Features
{
    public static class ItemCollectionRequirements
    {
        public static Dictionary<string, RequirementDef>
            ShopItemRequirements = new Dictionary<string, RequirementDef>();

        public static ItemCollectionDef FilterItemCollectionDef(ItemCollectionDef itemCollectionDef,
            SimGameState simGame)
        {
            if (itemCollectionDef == null ||
                !itemCollectionDef.Entries.Any(e => ShopItemRequirements.ContainsKey(e.ID)))
                return itemCollectionDef;

            var filtered = new ItemCollectionDef();

            var fTraverse = Traverse.Create(filtered);
            fTraverse.Field("ID").SetValue(itemCollectionDef.ID);
            fTraverse.Field("CollectionType").SetValue(itemCollectionDef.CollectionType);
            fTraverse.Field("Description").SetValue(itemCollectionDef.Description);

            filtered.Entries.AddRange(itemCollectionDef.Entries.Where(e =>
                !ShopItemRequirements.ContainsKey(e.ID)|| simGame.MeetsRequirements(ShopItemRequirements[e.ID])));

            Main.HBSLog.Log($"Filtered {itemCollectionDef.ID} with {itemCollectionDef.Entries.Count} entries to {filtered.Entries.Count} entries");

            return filtered;
        }
    }
}
