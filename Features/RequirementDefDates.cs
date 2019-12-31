using System.Linq;
using BattleTech;
using HBS.Collections;

namespace Timeline.Features
{
    public static class RequirementDefDates
    {
        public static bool MeetsDateRequirements(ref TagSet reqTags, ref TagSet exTags)
        {
            var reqTimelineTags = reqTags?.Where(tag => tag.StartsWith("timeline_")).ToArray();
            var exclTimelineTags = exTags?.Where(tag => tag.StartsWith("timeline_")).ToArray();

            if ((exclTimelineTags == null || exclTimelineTags.Length == 0)
                && (reqTimelineTags == null || reqTimelineTags.Length == 0))
                return true;

            var simGameDate = CurrentDate.GetSimGameDate(UnityGameInstance.BattleTechGame.Simulation);
            if (simGameDate == null)
            {
                Main.HBSLog.Log("Failing a requirement because timeline cannot get current date");
                return false;
            }

            var curTime = simGameDate.Value;
            var newReq = reqTags;
            var newEx = exTags;

            // check required timeline tags to see if they have been passed
            if (reqTimelineTags != null && reqTimelineTags.Length != 0)
            {
                if (reqTimelineTags.Select(Util.ParseTimelineTag)
                    .Any(reqTime => reqTime > curTime))
                {
                    Main.HBSLog.Log("Failing a requirement because its timeline required tag has not passed yet");
                    return false;
                }

                newReq = new TagSet(reqTags.Where(tag => !tag.StartsWith("timeline_")));
            }

            // check excluded timeline tags to see if they have not passed
            if (exclTimelineTags != null && exclTimelineTags.Length != 0)
            {
                if (exclTimelineTags.Select(Util.ParseTimelineTag)
                    .Any(exTime => exTime < curTime))
                {
                    Main.HBSLog.Log("Failing a requirement because its timeline exclude tag has passed already");
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
