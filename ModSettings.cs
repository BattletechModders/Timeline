using Newtonsoft.Json;
using System;

namespace Timeline
{
    internal class ModSettings
    {
        public string DateFormatString = "{0:yyyy MMMM d}{1}";
        public float AdvanceToTaskTime = 0.25f;
        public bool UseScroller = false;

        public static ModSettings ReadSettings(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<ModSettings>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog.Log($"Reading settings failed: {e.Message}");
                return new ModSettings();
            }
        }
    }
}
