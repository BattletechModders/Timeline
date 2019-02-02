using BattleTech;
using Harmony;
using HBS.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using JetBrains.Annotations;

namespace Timeline
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static SimGameEventDef SetTimelineEvent;
        internal static bool PoppedUpEvent;


        // ENTRY POINT
        [UsedImplicitly]
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.Timeline");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("Timeline");
            Settings = ModSettings.ReadSettings(modSettings);

            var eventPath = Path.Combine(modDir, "event_timeline_set.json");
            if (File.Exists(eventPath))
            {
                SetTimelineEvent = new SimGameEventDef();
                SetTimelineEvent.FromJSON(File.ReadAllText(eventPath));
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

            if (SetTimelineEvent == null)
            {
                var startDate = simGame.IsCareerMode()
                    ? simGame.Constants.CareerMode.CampaignStartDate
                    : simGame.Constants.Story.CampaignStartDate;

                SetStartingDateTag(simGame, startDate);
                HBSLog.LogWarning($"Event didn't exist, using {startDate}");
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
            var startingDate = GetStartingDate(simGame);

            if (startingDate != null && startingDate != simGame.CampaignStartDate)
            {
                HBSLog.Log("Setting SimGameState.campaignStartDate");
                Traverse.Create(simGame).Field("campaignStartDate").SetValue(startingDate);
            }

            return startingDate?.AddDays(simGame.DaysPassed);
        }

        internal static string GetTimelineDateString(SimGameState simGame)
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

        internal static string GetDayDateTag(DateTime date)
        {
            return $"timeline_{date:yyyy_MM_dd}";
        }

        internal static DateTime ParseTimelineTag(string tag)
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


        // MEAT
        internal static void SetDay(SimGameState simGame)
        {
            // TODO: DO SOMETHING WITH TAGS AND STUFF
            //var date = GetSimGameDate(simGame);
        }

        internal static bool ForceSetTimelineEvent(SimGameState simGame)
        {
            // only allow popup while time is actively moving, and you haven't popped up another event
            if (SetTimelineEvent == null || !simGame.TimeMoving || PoppedUpEvent)
                return false;

            HBSLog.Log($"Popping up event.");
            PoppedUpEvent = true;

            var eventTracker = new SimGameEventTracker();
            eventTracker.Init(new[] { EventScope.Company }, 0, 0, SimGameEventDef.SimEventType.NORMAL, simGame);
            simGame.InterruptQueue.QueueEventPopup(SetTimelineEvent, EventScope.Company, eventTracker);
            return true;
        }
    }
}
