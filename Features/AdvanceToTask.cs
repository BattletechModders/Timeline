﻿using System;
using BattleTech;


namespace Timeline.Features
{
    public static class AdvanceToTask
    {
        private static WorkOrderEntry _advancingTo;
        private static float _oldDayElapseTimeNormal;

        public static void StartAdvancing(WorkOrderEntry entry)
        {
            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            if (simGame.CurRoomState != DropshipLocation.SHIP)
                return;

            Main.HBSLog.Log($"Start advancing to {entry}");

            _advancingTo = entry;
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
            if (_advancingTo == null)
                return;

            _advancingTo = null;

            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            simGame.Constants.Time.DayElapseTimeNormal = _oldDayElapseTimeNormal;
            simGame.SetTimeMoving(false);
        }

        public static void OnDayPassed()
        {
            if (_advancingTo == null)
                return;

            var simGame = UnityGameInstance.BattleTechGame.Simulation;
            //var timelineWidget = Traverse.Create(simGame.RoomManager).Field("timelineWidget").GetValue<TaskTimelineWidget>();
            var timelineWidget = simGame.RoomManager.timelineWidget;
            //var activeItems = Traverse.Create(timelineWidget).Field("ActiveItems").GetValue<Dictionary<WorkOrderEntry, TaskManagementElement>>();
            var activeItems = timelineWidget.ActiveItems;
            // if timeline doesn't contain advancingTo or advancingTo is over
            if (!activeItems.ContainsKey(_advancingTo) || _advancingTo.IsCostPaid())
                StopAdvancing();
        }
    }
}
