using BattleTech.UI;

using Timeline.Features;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(TaskTimelineWidget), "OnTaskDetailsClicked")]
    public static class TaskTimelineWidget_OnTaskDetailsClicked_Patch
    {
        public static void Prefix(ref bool __runOriginal, TaskManagementElement element)
        {
            if (!__runOriginal) return;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AdvanceToTask.StartAdvancing(element.Entry);
                __runOriginal = false;
                return;
            }
            __runOriginal = true;
            return;
        }
    }
}
