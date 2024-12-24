using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace BBCR.Patches
{
    [HarmonyPatch]
    class FunSettingsInAnyMode
    {
        [HarmonyPatch(typeof(ClassicPartyManager), "Initialize")]
        [HarmonyPrefix]
        private static void FunSettingsInPartyEndless(ClassicPartyManager __instance)
        {
            __instance.lanternMode = __instance.gameObject.GetOrAddComponent<LanternMode>();
            __instance.mirrorMode = __instance.gameObject.GetOrAddComponent<MirrorMode>();
            __instance.lanternStrength = 6f;
            __instance.lanternColor = new Color(0.887f, 0.765f, 0.498f, 1f);
        }
        [HarmonyPatch(typeof(ClassicGameManager), "Initialize")]
        [HarmonyPrefix]
        private static void FunSettingsInClassicEndless(ClassicGameManager __instance)
        {
            __instance.lanternMode = __instance.gameObject.GetOrAddComponent<LanternMode>();
            __instance.mirrorMode = __instance.gameObject.GetOrAddComponent<MirrorMode>();
            __instance.lanternStrength = 6f;
            __instance.lanternColor = new Color(0.887f, 0.765f, 0.498f, 1f);
        }
    }
}
