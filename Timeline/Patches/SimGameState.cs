using System;
using System.Linq;
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
        public static void Postfix(SimGameState __instance)
        {
            Main.SetDay(__instance);
            AdvanceToTask.OnDayAdvance();
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

    [HarmonyPatch(typeof(SimGameState), "MeetsTagRequirements")]
    public static class SimGameState_MeetsTagRequirements_Patch
    {
        public static bool Prefix(ref TagSet reqTags, ref TagSet exTags, ref bool __result)
        {
            var reqTimelineTags = reqTags?.Where(tag => tag.StartsWith("timeline_")).ToArray();
            var exTimelineTags = exTags?.Where(tag => tag.StartsWith("timeline_")).ToArray();

            if ((exTimelineTags == null || exTimelineTags.Length == 0)
                && (reqTimelineTags == null || reqTimelineTags.Length == 0))
                return true;

            var curTimeNullable = Main.GetSimGameDate(UnityGameInstance.BattleTechGame.Simulation);
            if (curTimeNullable == null)
            {
                Main.HBSLog.Log("Failing a requirement because timeline cannot get current date");
                __result = false;
                return false;
            }

            var curTime = (DateTime)curTimeNullable;
            var newReq = reqTags;
            var newEx = exTags;

            // check required timeline tags to see if they have been passed
            if (reqTimelineTags != null && reqTimelineTags.Length != 0)
            {
                if (reqTimelineTags.Select(Main.ParseTimelineTag)
                    .Any(reqTime => reqTime > curTime))
                {
                    Main.HBSLog.Log("Failing a requirement because its timeline required tag has not passed yet");
                    __result = false;
                    return false;
                }

                newReq = new TagSet(reqTags.Where(tag => !tag.StartsWith("timeline_")));
            }

            // check excluded timeline tags to see if they have not passed
            if (exTimelineTags != null && exTimelineTags.Length != 0)
            {
                if (exTimelineTags.Select(Main.ParseTimelineTag)
                    .Any(exTime => exTime < curTime))
                {
                    Main.HBSLog.Log("Failing a requirement because its timeline exclude tag has passed already");
                    __result = false;
                    return false;
                }

                newEx = new TagSet(exTags.Where(tag => !tag.StartsWith("timeline_")));
            }

            // pass to original method the tagsets without the timeline tags
            reqTags = newReq;
            exTags = newEx;
            return true;
        }
    }
}
