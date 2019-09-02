using System;
using System.Collections.Generic;
using BattleTech;
using Timeline.Resources;

namespace Timeline.Features
{
    public static class ForcedEvents
    {
        public static List<ForcedTimelineEvent> ForcedTimelineEvents = new List<ForcedTimelineEvent>();

        public static void OnDayPassed(SimGameState simGame, int timeLapse)
        {
            if (timeLapse == 0)
                timeLapse = 1;

            var curDateNullable = CurrentDate.GetSimGameDate(simGame);
            if (curDateNullable == null)
                return;

            var curDate = curDateNullable.Value;
            var prevDate = curDate.Subtract(new TimeSpan(timeLapse, 0, 0, 0));

            Main.HBSLog.Log($"ForcedEvents.OnDayPassed {prevDate} -> {curDate} num events: {ForcedTimelineEvents.Count}");

            foreach (var timelineEvent in ForcedTimelineEvents)
            {
                if (!simGame.DataManager.SimGameEventDefs.TryGet(timelineEvent.EventID, out var eventDef))
                {
                    Main.HBSLog.LogWarning($"Could not find event with ID: {timelineEvent.EventID}");
                    continue;
                }

                //Main.HBSLog.Log($"\t{eventDef.Description.Id} {timelineEvent.DateToFire}");

                if (eventDef.Scope == EventScope.Company
                    && prevDate < timelineEvent.DateToFire && curDate >= timelineEvent.DateToFire)
                {
                    if (simGame.MeetsRequirements(eventDef.Requirements)
                        && simGame.MeetsRequirements(eventDef.AdditionalRequirements))
                    {
                        Util.FireEvent(simGame, eventDef);
                    }
                }
            }
        }
    }
}
