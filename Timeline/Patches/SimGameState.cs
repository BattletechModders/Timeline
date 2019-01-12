using BattleTech;
using Harmony;

namespace Timeline
{
    [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
    public static class SimGameState_OnDayPassed_Patch
    {
        public static void Postfix(SimGameState __instance)
        {
            Main.SetDay(__instance);
        }
    }

    [HarmonyPatch(typeof(SimGameState), "Init")]
    public static class SimGameState_Init_Patch
    {
        public static void Postfix(SimGameState __instance)
        {
            Main.poppedUpEvent = false;
        }
    }

    [HarmonyPatch(typeof(SimGameState), "InitFromSave")]
    public static class SimGameState_InitFromSave_Patch
    {
        public static void Postfix(SimGameState __instance)
        {
            Main.poppedUpEvent = false;
        }
    }
}