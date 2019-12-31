using BattleTech.UI;
using Harmony;
using Timeline.Features;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(TaskTimelineWidget), "OnTaskDetailsClicked")]
    public static class TaskTimelineWidget_OnTaskDetailsClicked_Patch
    {
        public static bool Prefix(TaskManagementElement element)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AdvanceToTask.StartAdvancing(element.Entry);
                return false;
            }

            return true;
        }
    }
}
