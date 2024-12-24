using System;
using System.Collections.Generic;
using System.Text;
using BBCR.API;
using HarmonyLib;
namespace BBCR.Patches
{
    [HarmonyPatch]
    class FixingBugs
    {
        [HarmonyPatch(typeof(YCTP), "OnDisable")]
        [HarmonyPostfix]
        private static void FixYCTPHardMode()
        {
            if (VariablesStorage.CurrentStyle.IsNullOrGlitch()) CoreGameManager.Instance.GetPlayer(0).PlayerTimeScale = 1f;
        }
    }
}
