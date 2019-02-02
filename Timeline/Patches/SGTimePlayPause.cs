using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(SGTimePlayPause), "SetDay")]
    public static class SGTimePlayPause_SetDay_Patch
    {
        public static void Postfix(SGTimePlayPause __instance)
        {
            var timePassedText = Traverse.Create(__instance).Field("timePassedText").GetValue<TextMeshProUGUI>();
            var simGame = Traverse.Create(__instance).Field("simState").GetValue<SimGameState>();
            timePassedText.text = Main.GetTimelineDateString(simGame);
        }
    }
}