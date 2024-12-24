using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using System.Linq;
using System.Reflection.Emit;
using BBCR.API;

namespace BBCR.Patches.Styles
{
    [HarmonyPatch(typeof(ClassicNullManager))]
    class NullStyle
    {
        public static bool BossActive = false;
        public static bool ElevatorsOpenedSecondTime = false;
        public static void OpenExit(ClassicNullManager classicNullManager)
        {
            classicNullManager.elevatorsToClose = 0;
            classicNullManager.freezeAllElevators = false;
            classicNullManager.elevatorsNullified = false;
        }
        public static IEnumerator RageNull(NullNPC nullNPC)
        {
            WaitForSeconds delay = new WaitForSeconds(5f);
            while (true)
            {
                nullNPC.GetAngry(1);
                yield return delay;
            }
        }
        [HarmonyPatch("CollectNotebook")]
        [HarmonyPrefix]
        private static bool CollectNotebookFix(ClassicNullManager __instance, Notebook notebook)
        {
            if (Singleton<PlayerFileManager>.Instance.flags[1] && Singleton<PlayerFileManager>.Instance.flags[2] && Singleton<PlayerFileManager>.Instance.flags[3])
            {
                if (Singleton<PlayerFileManager>.Instance.chaosMode)
                {
                    __instance.ec.SpawnNPCs();
                    __instance.ec.RandomizeEvents(__instance.ec.EventsCount, 30f, 30f, 180f, new System.Random());
                    __instance.ec.StartEventTimers();
                }
                if (Singleton<CoreGameManager>.Instance.freeRun)
                {
                    __instance.CollectNotebooks(1);
                    notebook.Hide(true);
                    return false;
                }
            }
            return true;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void UpdateFixer(ClassicNullManager __instance)
        {
            if ((!__instance.finalElevator.IsNull() && !__instance.finalElevatorTargetTile.IsNull()) && ((!Singleton<CoreGameManager>.Instance.hardMode) || (Singleton<CoreGameManager>.Instance.hardMode && ElevatorsOpenedSecondTime)))
            {
                NullStyle.BossActive = __instance.bossActive;
                NullNPC nullNpc = (NullNPC)__instance.ec.GetBaldi();
                if (nullNpc.transform.position.x == __instance.finalElevatorTargetTile.transform.position.x && nullNpc.transform.position.z == __instance.finalElevatorTargetTile.transform.position.z && __instance.finalElevator.IsOpen)
                {
                    __instance.finalElevator.Close();
                    __instance.elevatorsClosed += 1;
                    __instance.ElevatorClosed(__instance.finalElevator);
                    if (ElevatorsOpenedSecondTime)
                    {
                        nullNpc.SpeechCheck(NullPhrase.Haha, 100f);
                    }
                    else
                    {
                        __instance.bossActive = true;
                        nullNpc.StartBossIntro();
                    }
                }
            }
        }
        [HarmonyPatch("NullHit")]
        [HarmonyPrefix]
        private static void OnNullHit(ClassicNullManager __instance)
        {
            if (Singleton<CoreGameManager>.Instance.hardMode)
            {
                __instance.nullNpc.GetAngry(0.05f);
            }
            if (__instance.health == 1 && (Singleton<CoreGameManager>.Instance.freeRun || Singleton<CoreGameManager>.Instance.hardMode))
            {
                if (!ElevatorsOpenedSecondTime && Singleton<CoreGameManager>.Instance.hardMode && !Singleton<CoreGameManager>.Instance.freeRun)
                {
                    __instance.allNotebooksFound = false;
                    ElevatorsOpenedSecondTime = true;
                    __instance.elevatorsToClose = __instance.ec.elevators.Count - 1;
                    __instance.elevatorsClosed = 0;
                    __instance.freezeAllElevators = false;
                    __instance.forceCloseAllElevators = false;
                    __instance.elevatorsNullified = false;
                    __instance.bossActive = true;
                    __instance.nullNpc.controlOverride = false;
                    __instance.ec.standardDarkLevel = new Color(0.2f, 0f, 0f);
                    __instance.ec.FlickerLights(true);
                    __instance.gameObject.DeleteComponent<LanternMode>();
                    LanternMode lanternMode = __instance.gameObject.AddComponent<LanternMode>();
                    lanternMode.Initialize(__instance.ec);
                    lanternMode.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, 12f, Color.red);
                    __instance.AllNotebooks();
                    __instance.StartCoroutine(RageNull(__instance.nullNpc));
                    __instance.finalElevator = null;
                    __instance.finalElevatorTargetTile = null;
                    __instance.ec.audMan.PlaySingle(AssetsAPI.LoadAsset<SoundObject>(x => !x.additionalKeys.Where(x => x.key == "Vfx_Null_ClassicSpeech4").IsNull()));
                }
                __instance.SpawnProjectile();
                __instance.health += 1;
            }
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void InitializePostfix(ClassicNullManager __instance)
        {
            ElevatorsOpenedSecondTime = false;
            MirrorMode mirrorMode = __instance.gameObject.GetOrAddComponent<MirrorMode>();
            LanternMode lanternMode = __instance.gameObject.GetOrAddComponent<LanternMode>();
            Color lightsOutColor = new Color(0.887f, 0.765f, 0.498f, 1f);
            float strenght = 6f;
            if (!Singleton<CoreGameManager>.Instance.lightsOut)
            {
                strenght = 69f;
                lightsOutColor = new Color(0.45f, 0.45f, 0.45f, 1f);
            }
            lanternMode.Initialize(__instance.ec);
            lanternMode.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, strenght, lightsOutColor);
            if (Singleton<CoreGameManager>.Instance.mirrorMode) mirrorMode.Initialize();

            if (!Singleton<CoreGameManager>.Instance.hardMode) __instance.elevatorsNullified = false; 
            if (Singleton<CoreGameManager>.Instance.freeRun)
            {
                foreach (ClassicSwingDoor classicSwingDoor in ClassicSwingDoor.allClassicDoors)
                {
                    classicSwingDoor.Unlock();
                }
            }
        }
        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        private static bool DontAllNotebooks()
        {
            if (!VariablesStorage.StyleIsEndless && Singleton<CoreGameManager>.Instance.freeRun)
            {
                Singleton<MusicManager>.Instance.PlayMidi("Level_1_End", true);
                Singleton<MusicManager>.Instance.SetSpeed(0.75f);
            }
            return !VariablesStorage.StyleIsEndless;
        }
        [HarmonyPatch("CollectNotebook")]
        [HarmonyPrefix]
        private static void RespawnNotebookInEndless(ClassicNullManager __instance, Notebook notebook)
        {
            if (VariablesStorage.StyleIsEndless)
            {
                notebook.activity.StartResetTimer(60f);
                __instance.AngerBaldi(0.25f);
            }
        }
        [HarmonyPatch("ElevatorClosed")]
        [HarmonyPostfix]
        private static void AllowGoToExitAndFixBug(ClassicNullManager __instance)
        {
            if (!Singleton<CoreGameManager>.Instance.freeRun) {
                if (__instance.elevatorsClosed == 3 && !Singleton<CoreGameManager>.Instance.hardMode)
                {
                    OpenExit(__instance);
                }
                if (__instance.elevatorsClosed == 3 && Singleton<CoreGameManager>.Instance.hardMode && ElevatorsOpenedSecondTime)
                {
                    OpenExit(__instance);
                }
            }
            if (Singleton<PlayerFileManager>.Instance.authenticMode) Singleton<CoreGameManager>.Instance.GetHud(0).Hide(false);
        }
    }
}
