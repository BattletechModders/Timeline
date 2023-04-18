using BattleTech.UI;

using Timeline.Features;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(SGTimePlayPause), "SetDay")]
    public static class SGTimePlayPause_SetDay_Patch
    {
        public static void Postfix(SGTimePlayPause __instance)
        {
            //var timePassedText = Traverse.Create(__instance).Field("timePassedText").GetValue<TextMeshProUGUI>();
            var timePassedText = __instance.timePassedText;
            //var simGame = Traverse.Create(__instance).Field("simState").GetValue<SimGameState>();
            var simGame = __instance.simState;
            timePassedText.text = CurrentDate.GetTimelineDateString(simGame);
        }
    }

    [HarmonyPatch(typeof(SGTimePlayPause), "SetTimeMoving")]
    public static class SGTimePlayPause_SetTimeMoving_Patch
    {
        public static void Postfix(bool isPlaying)
        {
            if (!isPlaying)
                AdvanceToTask.StopAdvancing();
        }
    }
}