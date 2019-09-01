using Harmony;
using HBS.Logging;
using System.Reflection;
using Timeline.Features;

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
    }
}
