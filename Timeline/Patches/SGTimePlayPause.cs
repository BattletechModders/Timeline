using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace Timeline
{
    [HarmonyPatch(typeof(SGTimePlayPause), "SetDay")]
    public static class SGTimePlayPause_SetDay_Patch
    {
        public static void Postfix(SGTimePlayPause __instance)
        {
            var timePassedText = Traverse.Create(__instance).Field("timePassedText").GetValue<TextMeshProUGUI>();
            var simGame = Traverse.Create(__instance).Field("simState").GetValue<SimGameState>();
            timePassedText.text = Main.GetTimelineDate(simGame);
        }
    }
}