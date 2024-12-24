using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace BBCR.Patches
{
    [HarmonyPatch(typeof(RoomController))]
    class CustomPrincipalDoor
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void ReplaceDoor(RoomController __instance)
        {
            if (__instance.category == RoomCategory.Office)
            {
                __instance.doorMats = AssetsAPI.CreateDoor("OfficeDoor", BasePlugin.assets.Get<Texture2D>("OfficeOpen"), BasePlugin.assets.Get<Texture2D>("OfficeClosed"));
            }
        }
    }
}
