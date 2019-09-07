using BattleTech;
using BattleTech.UI.Tooltips;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Timeline.Patches
{
    [HarmonyPatch(typeof(TooltipManager), "SetActiveTooltip")]
    public static class TooltipManager_SetActiveTooltip_Patch
    {
        public static void Prefix(ref object data)
        {
            switch (data)
            {
                case DescriptionDef descriptionDef:
                {
                    data = new BaseDescriptionDef(descriptionDef);
                    break;
                }
                case StarSystemDef starSystemDef:
                {
                    var simGame = UnityGameInstance.BattleTechGame.Simulation;
                    if (simGame == null)
                        return;

                    if (simGame.StarSystemDictionary.ContainsKey(starSystemDef.CoreSystemID))
                        data = simGame.StarSystemDictionary[starSystemDef.CoreSystemID];
                    break;
                }
            }
        }
    }
}
