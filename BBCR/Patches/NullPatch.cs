using System;
using System.Collections.Generic;
using System.Text;
using BBCR.Patches.Styles;
using HarmonyLib;
using UnityEngine;

namespace BBCR.Patches.NPC
{
    [HarmonyPatch(typeof(NullNPC))]
    class NullPatch
    {
        [HarmonyPatch("Hit")]
        [HarmonyPrefix]
        private static void OnNullHit(NullNPC __instance)
        {
            __instance.stunTime = 1f;
            if (Singleton<CoreGameManager>.Instance.hardMode && __instance.health > 1)
            {
                __instance.stunTime = 0f;
            }
            else if (Singleton<CoreGameManager>.Instance.hardMode && __instance.health == 1)
            {
                __instance.stunTime = 0.5f;
            }
        }
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        static bool OnTriggerEnterPrefix(Collider other)
        {
            if (!NullStyle.BossActive && Singleton<CoreGameManager>.Instance.freeRun && other.tag == "Player")
            {
                return false;
            }
            return true;
        }
    }
}
