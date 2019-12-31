using System;
using System.Linq;
using BattleTech;

namespace Timeline
{
    internal static class Util
    {
        public static void FireEvent(SimGameState simGame, SimGameEventDef eventDef)
        {
            Main.HBSLog.Log($"Firing event {eventDef.Description.Id}");

            var eventTracker = new SimGameEventTracker();
            eventTracker.Init(new[] { EventScope.Company }, 0, 0, SimGameEventDef.SimEventType.NORMAL, simGame);
            simGame.InterruptQueue.QueueEventPopup(eventDef, EventScope.Company, eventTracker);
        }

        public static string GetStartingDateTag(SimGameState simGame)
        {
            return simGame.CompanyTags.ToList().Find(x => x.Contains("start_timeline"));
        }

        public static void SetStartingDateTag(SimGameState simGame, DateTime startDate)
        {
            var startDateTag = "start_" + GetDayDateTag(startDate);
            Main.HBSLog.Log($"Setting the starting date tag: {startDateTag}");
            simGame.CompanyTags.Add(startDateTag);
        }

        public static string GetDayDateTag(DateTime date)
        {
            return $"timeline_{date:yyyy_MM_dd}";
        }

        public static DateTime ParseTimelineTag(string tag)
        {
            if (tag.StartsWith("start_"))
                tag = tag.Substring(6);

            var splitTag = tag.Split('_');

            var year = int.Parse(splitTag[1]);
            var month = 1;
            var day = 1;

            if (splitTag.Length >= 3)
                month = int.Parse(splitTag[2]);

            if (splitTag.Length >= 4)
                day = int.Parse(splitTag[3]);

            return new DateTime(year, month, day);
        }

        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}
