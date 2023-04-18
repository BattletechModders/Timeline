using System.Collections.Generic;
using System.IO;
using Harmony;
using HBS.Logging;
using System.Reflection;
using BattleTech;
using HBS.Util;
using Timeline.Features;
using Timeline.Resources;

namespace Timeline
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string ModDirectory;

        // ENTRY POINT
        public static void Init(string modDir, string modSettings)
        {
            HBSLog = Logger.GetLogger("Timeline");
            Settings = ModSettings.ReadSettings(modSettings);
            ModDirectory = modDir;
            var harmony = HarmonyInstance.Create("io.github.mpstark.Timeline");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            CurrentDate.SetupSetTimelineEvent();
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (customResources.ContainsKey(nameof(ForcedTimelineEvent)))
            {
                foreach (var entry in customResources[nameof(ForcedTimelineEvent)].Values)
                {
                    ForcedEvents.ForcedTimelineEvents.Add(SerializeUtil.FromPath<ForcedTimelineEvent>(entry.FilePath));
                }
            }

            if (customResources.ContainsKey(nameof(ItemCollectionRequirement)))
            {
                foreach (var entry in customResources[nameof(ItemCollectionRequirement)].Values)
                {
                    // have to use fastJSON because requirementDefs are very picky apparently
                    var itemReq = new ItemCollectionRequirement();
                    JSONSerializationUtility.FromJSON(itemReq, File.ReadAllText(entry.FilePath));
                    ItemCollectionRequirements.ShopItemRequirements.Add(itemReq.ItemID, itemReq.RequirementDef);
                }
            }
        }
    }
}
