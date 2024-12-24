using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BBCR.Patches.Styles;
using HarmonyLib;
using UnityEngine;

namespace BBCR.Patches
{
    [HarmonyPatch(typeof(CoreGameManager))]
    class NullNotCrashGame
    {
        public static void QuitToMenu() => Singleton<CoreGameManager>.Instance.ReturnToMenu();
        [HarmonyPatch("EndSequence", MethodType.Enumerator)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ReplaceCrashWithQuiting(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> res = new List<CodeInstruction>() { };
            int x = 0;
            foreach (CodeInstruction codeInstruction in instructions)
            {
                if (x != 191) res.Add(codeInstruction);
                else res.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NullNotCrashGame), "QuitToMenu"))); 
                x++;
            }
            return res;
        }
        [HarmonyPatch("Quit")]
        [HarmonyPrefix]
        private static bool ReplaceQuitWithCustom(CoreGameManager __instance)
        {
            Singleton<GlobalCam>.Instance.SetListener(true);
            Singleton<SubtitleManager>.Instance.DestroyAll();
            __instance.ReturnToMenu();
            return false;
        }
    }
}
