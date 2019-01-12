using BattleTech;
using Harmony;
using HBS.Logging;
using System;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.IO;

namespace Timeline
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static SimGameEventDef setTimelineEvent;
        internal static bool poppedUpEvent = false;


        // ENTRY POINT
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.Timeline");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("Timeline");
            Settings = ModSettings.ReadSettings(modSettings);

            var eventPath = Path.Combine(modDir, "event_timeline_set.json");
            if (File.Exists(eventPath))
            {
                setTimelineEvent = new SimGameEventDef();
                setTimelineEvent.FromJSON(File.ReadAllText(eventPath));
            }
            else
            {
                HBSLog.LogWarning($"FAILED TO READ EVENT! WILL DEFAULT TO 3025 IF YEAR NOT SET!");
            }
        }


        // UTIL
        private static DateTime? GetStartingDate(SimGameState simGame)
        {
            var startDateTag = GetStartingDateTag(simGame);
            if (startDateTag != null)
                return ParseTimelineTag(startDateTag);

            if (setTimelineEvent == null)
            {
                var startDate = new DateTime(Settings.StartingYear, Settings.StartingMonth, Settings.StartingDay);
                SetStartingDateTag(simGame, startDate);
                HBSLog.LogWarning($"DEFAULTED TO 3025 BECAUSE EVENT DIDN'T EXIST!");
                return startDate;
            }

            if (!ForceSetTimelineEvent(simGame))
            {
                // event hasn't triggered
            }

            return null;
        }

        private static string GetDaySuffix(int day)
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

        internal static DateTime? GetSimGameDate(SimGameState simGame)
        {
            return GetStartingDate(simGame)?.AddDays(simGame.DaysPassed);
        }

        internal static string GetTimelineDate(SimGameState simGame)
        {
            var date = GetSimGameDate(simGame);

            if (date == null)
                return "- Event Popup -";

            return string.Format(Settings.DateFormatString, date, GetDaySuffix(((DateTime)date).Day));
        }


        // TAGS
        private static string GetStartingDateTag(SimGameState simGame)
        {
            return simGame.CompanyTags.ToList().Find(x => x.Contains("start_timeline"));
        }

        private static void SetStartingDateTag(SimGameState simGame, DateTime startDate)
        {
            var startDateTag = "start_" + GetDayDateTag(startDate);
            HBSLog.Log($"Setting the starting date tag: {startDateTag}");
            simGame.CompanyTags.Add(startDateTag);
        }

        internal static string GetYearDateTag(DateTime date)
        {
            return $"timeline_{date.ToString("yyyy")}";
        }

        internal static string GetMonthDateTag(DateTime date)
        {
            return $"timeline_{date.ToString("yyyy_MM")}";
        }

        internal static string GetDayDateTag(DateTime date)
        {
            return $"timeline_{date.ToString("yyyy_MM_dd")}";
        }

        internal static DateTime ParseTimelineTag(string tag)
        {
            if (tag.StartsWith("start_"))
                tag = tag.Substring(6);

            var splitTag = tag.Split('_');

            var year = splitTag[1];
            var month = "Jan";
            var day = 1;

            if (splitTag.Length >= 3)
                month = splitTag[2];

            if (splitTag.Length >= 4)
                day = int.Parse(splitTag[3]);

            return DateTime.Parse($"{month}/{day}/{year}", CultureInfo.GetCultureInfo("en-US").DateTimeFormat);
        }


        // MEAT
        internal static void SetDay(SimGameState simGame)
        {
            var date = GetSimGameDate(simGame);

            // TODO: DO SOMETHING WITH TAGS AND STUFF
        }

        internal static bool ForceSetTimelineEvent(SimGameState simGame)
        {
            // only allow popup while time is actively moving, and you haven't popped up another event
            if (setTimelineEvent == null || !simGame.TimeMoving || poppedUpEvent)
                return false;

            HBSLog.Log($"Popping up event.");
            poppedUpEvent = true;

            var eventTracker = new SimGameEventTracker();
            eventTracker.Init(new EventScope[] { EventScope.Company }, 0, 0, SimGameEventDef.SimEventType.NORMAL, simGame);
            simGame.InterruptQueue.QueueEventPopup(setTimelineEvent, EventScope.Company, eventTracker);
            return true;
        }
    }
}
