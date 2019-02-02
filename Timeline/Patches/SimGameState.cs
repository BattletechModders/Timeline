using BattleTech;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
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
        public static void Postfix()
        {
            Main.PoppedUpEvent = false;
        }
    }

    [HarmonyPatch(typeof(SimGameState), "InitFromSave")]
    public static class SimGameState_InitFromSave_Patch
    {
        public static void Postfix()
        {
            Main.PoppedUpEvent = false;
        }
    }
}
