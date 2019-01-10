using BattleTech.UI;
using Harmony;
using TMPro;

namespace Timeline
{
    [HarmonyPatch(typeof(SGTimePlayPause), "SetDay")]
    public static class SGTimePlayPause_SetDay_Patch
    {
        public static void Postfix(SGTimePlayPause __instance, int daysPassed)
        {
            var timePassedText = Traverse.Create(__instance).Field("timePassedText").GetValue<TextMeshProUGUI>();
            timePassedText.text = Main.GetTimelineDate(daysPassed);
        }
    }
}