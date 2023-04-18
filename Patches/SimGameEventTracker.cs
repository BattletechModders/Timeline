using BattleTech;

using Timeline.Features;

// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(SimGameEventTracker), "OnOptionSelected")]
    public class SimGameEventTracker_OnOptionSelected_Patch
    {
        public static bool Prepare()
        {
            return Main.Settings.UseScroller;
        }

        public static void Prefix(SimGameEventOption option)
        {
            // not the most robust
            // do nothing on Vanilla start
            if (!option.Description.Id.StartsWith("timeline_3") ||
                option.Description.Id == "timeline_3025")
            {
                return;
            }

            var scrollText = option.ResultSets[0].Description.Details;
            TextScroller.CreateScroller(scrollText);
        }
    }
}
