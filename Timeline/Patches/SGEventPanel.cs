using BattleTech.UI;
using Harmony;
using TMPro;

namespace Timeline
{
    [HarmonyPatch(typeof(SGEventPanel), "SetEvent")]
    public static class SGEventPanel_SetEvent_Patch
    {
        public static void Postfix(SGEventPanel __instance)
        {
            var eventTime = Traverse.Create(__instance).Field("eventTime").GetValue<TextMeshProUGUI>();
            eventTime.text = Main.GetTimelineDate(__instance.Sim.DaysPassed);
        }
    }
}
