using System;
using System.IO;
using BattleTech;

namespace Timeline.Features
{
    public static class CurrentDate
    {
        private static SimGameEventDef _setTimelineEvent;
        private static bool _poppedUpEvent;


        // SELECT EVENT
        public static void ResetEventPopup()
        {
            _poppedUpEvent = false;
        }

        public static void SetupSetTimelineEvent()
        {
            var eventPath = Path.Combine(Main.ModDirectory, "event_timeline_set.json");
            if (File.Exists(eventPath))
            {
                _setTimelineEvent = new SimGameEventDef();
                _setTimelineEvent.FromJSON(File.ReadAllText(eventPath));
            }
            else
            {
                Main.HBSLog.LogWarning($"FAILED TO READ EVENT! WILL DEFAULT TO 3025 IF YEAR NOT SET!");
            }
        }

        private static void TryFireSetTimelineEvent(SimGameState simGame)
        {
            // only allow popup while time is actively moving, and you haven't popped up another event
            if (!simGame.TimeMoving || _poppedUpEvent)
                return;

            _poppedUpEvent = true;
            Util.FireEvent(simGame, _setTimelineEvent);
        }


        // DATE
        public static string GetTimelineDateString(SimGameState simGame)
        {
            var date = GetSimGameDate(simGame);

            if (date == null)
                return "- Date Not Set -";

            return string.Format(Main.Settings.DateFormatString, date, Util.GetDaySuffix(((DateTime)date).Day));
        }

        public static DateTime? GetSimGameDate(SimGameState simGame)
        {
            var startingDate = GetStartingDate(simGame);

            if (startingDate != null && startingDate != simGame.GetCampaignStartDate())
                simGame.SetCampaignStartDate((DateTime) startingDate);

            return startingDate?.AddDays(simGame.DaysPassed);
        }

        private static DateTime? GetStartingDate(SimGameState simGame)
        {
            var startDateTag = Util.GetStartingDateTag(simGame);
            if (startDateTag != null)
                return Util.ParseTimelineTag(startDateTag);

            if (_setTimelineEvent != null)
            {
                TryFireSetTimelineEvent(simGame);
                return null;
            }

            var startDateString = simGame.IsCareerMode()
                ? simGame.Constants.CareerMode.CampaignStartDate
                : simGame.Constants.Story.CampaignStartDate;

            var startDate = DateTime.Parse(startDateString);
            Util.SetStartingDateTag(simGame, startDate);
            Main.HBSLog.LogWarning($"Event didn't exist, using {startDate}");

            return startDate;
        }
    }
}
