using Harmony;
using HBS.Logging;
using System;
using System.Reflection;

namespace Timeline
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;


        // ENTRY POINT
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.Timeline");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("Timeline");
            Settings = ModSettings.ReadSettings(modSettings);
        }


        // UTIL
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

        private static DateTime GetDateFromDaysPassed(int daysPassed)
        {
            return new DateTime(Settings.StartingYear, Settings.StartingMonth, Settings.StartingDay).AddDays(daysPassed);
        }


        // MEAT
        internal static void SetDay(int daysPassed)
        {
            var date = GetDateFromDaysPassed(daysPassed);

            // TODO: DO SOMETHING WITH TAGS AND STUFF
        }

        public static string GetTimelineDate(int daysPassed)
        {
            var date = GetDateFromDaysPassed(daysPassed);
            return string.Format(Settings.DateFormatString, date, GetDaySuffix(date.Day));
        }
    }
}
