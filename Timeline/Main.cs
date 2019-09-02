using System.Collections.Generic;
using Harmony;
using HBS.Logging;
using System.Reflection;
using BattleTech;
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
            var harmony = HarmonyInstance.Create("io.github.mpstark.Timeline");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("Timeline");
            Settings = ModSettings.ReadSettings(modSettings);
            ModDirectory = modDir;

            CurrentDate.SetupSetTimelineEvent();
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (customResources.ContainsKey(nameof(ForcedTimelineEvent)))
            {
                foreach (var entry in customResources[nameof(ForcedTimelineEvent)].Values)
                    ForcedEvents.ForcedTimelineEvents.Add(SerializeUtil.FromPath<ForcedTimelineEvent>(entry.FilePath));
            }
        }
    }
}
