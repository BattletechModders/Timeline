using BattleTech;
using Harmony;
using HBS.Collections;
using Timeline.Features;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
    public static class SimGameState_OnDayPassed_Patch
    {
        public static void Postfix(SimGameState __instance, int timeLapse)
        {
            AdvanceToTask.OnDayPassed();
            ForcedEvents.OnDayPassed(__instance, timeLapse);
        }
    }

    [HarmonyPatch(typeof(SimGameState), "Init")]
    public static class SimGameState_Init_Patch
    {
        public static void Postfix()
        {
            CurrentDate.ResetEventPopup();
        }
    }

    [HarmonyPatch(typeof(SimGameState), "InitFromSave")]
    public static class SimGameState_InitFromSave_Patch
    {
        public static void Postfix()
        {
            CurrentDate.ResetEventPopup();
        }
    }

    [HarmonyPatch(typeof(SimGameState), "MeetsTagRequirements")]
    public static class SimGameState_MeetsTagRequirements_Patch
    {
        public static bool Prefix(ref TagSet reqTags, ref TagSet exTags, ref bool __result)
        {
            if (RequirementDefDates.MeetsDateRequirements(ref reqTags, ref exTags))
                return true;

            __result = false;
            return false;
        }
    }
}
