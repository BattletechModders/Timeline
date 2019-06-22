using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace Timeline.Features
{
    public static class AdvanceToTask
    {
        public static WorkOrderEntry AdvancingTo { get; private set; }
        private static float _oldDayElapseTimeNormal;

        public static void StartAdvancing(WorkOrderEntry entry)
        {
            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            if (simGame.CurRoomState != DropshipLocation.SHIP)
                return;

            Main.HBSLog.Log($"Start advancing to {entry}");

            AdvancingTo = entry;
            simGame.SetTimeMoving(true);

            // set the elapseTime variable so that the days pass faster
            if (Math.Abs(simGame.Constants.Time.DayElapseTimeNormal - Main.Settings.AdvanceToTaskTime) > 0.01)
            {
                _oldDayElapseTimeNormal = simGame.Constants.Time.DayElapseTimeNormal;
                simGame.Constants.Time.DayElapseTimeNormal = Main.Settings.AdvanceToTaskTime;
            }
        }

        public static void StopAdvancing()
        {
            if (AdvancingTo == null)
                return;

            AdvancingTo = null;

            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            simGame.Constants.Time.DayElapseTimeNormal = _oldDayElapseTimeNormal;
            simGame.SetTimeMoving(false);
        }

        public static void OnDayAdvance()
        {
            if (AdvancingTo == null)
                return;

            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            var timelineWidget = Traverse.Create(simGame.RoomManager).Field("timelineWidget")
                .GetValue<TaskTimelineWidget>();
            var activeItems = Traverse.Create(timelineWidget).Field("ActiveItems")
                .GetValue<Dictionary<WorkOrderEntry, TaskManagementElement>>();

            // if timeline doesn't contain advancingTo or advancingTo is over
            if (!activeItems.ContainsKey(AdvancingTo) || AdvancingTo.IsCostPaid())
                StopAdvancing();
        }
    }
}
