using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BBCR.API;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BBCR.Patches
{
    [HarmonyPatch]
    class SetupMod
    {
        [HarmonyPatch(typeof(GameLoader), "Initialize")]
        [HarmonyPostfix]
        private static void SetupFunSetting(int mode, GameLoader __instance)
        {
            VariablesStorage.StyleIsEndless = true;
            Singleton<CoreGameManager>.Instance.mirrorMode = __instance.mirrorMode;
            Singleton<CoreGameManager>.Instance.lightsOut = __instance.lightsOut;
            Singleton<CoreGameManager>.Instance.hardMode = __instance.hardMode;
            if (mode == 0)
            {
                VariablesStorage.StyleIsEndless = false;
                Singleton<CoreGameManager>.Instance.freeRun = __instance.freeRun;
            }

        }
        [HarmonyPatch(typeof(MainMenu), "Start")]
        [HarmonyPostfix]
        private static void AddLoadingScreen(MainMenu __instance)
        {
            if (!LoadingAPI.finished)
                __instance.gameObject.AddComponent<LoadingSceen>().mainMenu = __instance;
        }
        [HarmonyPatch(typeof(GameLoader), "StartGame")]
        [HarmonyPrefix]
        private static void SetupStyle(GameLoader __instance)
        {
            switch (__instance.style)
            {
                case ClassicStyle.Classic:
                    VariablesStorage.CurrentStyle = Style.Classic;
                    break;
                case ClassicStyle.Party:
                    VariablesStorage.CurrentStyle = Style.Party;
                    break;
                case ClassicStyle.Demo:
                    VariablesStorage.CurrentStyle = Style.Demo;
                    break;
                case ClassicStyle.Null:
                    if (!Singleton<PlayerFileManager>.Instance.flags[4]) VariablesStorage.CurrentStyle = Style.Null;
                    else VariablesStorage.CurrentStyle = Style.Glitch;
                    break;
                default:
                    break;
            }
        }
    }
}
