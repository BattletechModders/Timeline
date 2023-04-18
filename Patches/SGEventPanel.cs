using BattleTech.UI;

using Timeline.Features;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(SGEventPanel), "SetEvent")]
    public static class SGEventPanel_SetEvent_Patch
    {
        public static void Postfix(SGEventPanel __instance)
        {
            //var eventTime = Traverse.Create(__instance).Field("eventTime").GetValue<TextMeshProUGUI>();
            var eventTime = __instance.eventTime;
            eventTime.text = CurrentDate.GetTimelineDateString(__instance.Sim);
        }
    }
}
