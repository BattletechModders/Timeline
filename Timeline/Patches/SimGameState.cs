using BattleTech;
using Harmony;

namespace Timeline
{
    [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
    public static class SimGameState_OnDayPassed_Patch
    {
        public static void Postfix(SimGameState __instance)
        {
            Main.SetDay(__instance.DaysPassed);
        }
    }
}