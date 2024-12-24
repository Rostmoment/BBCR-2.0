using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBCR.API;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BBCR.Patches
{
    class ModdedBaseGameManager : MonoBehaviour
    {
        private static readonly Dictionary<int, int> frames = new Dictionary<int, int>()
        {
            {0, 0}, {1, 0}, {2, 0}, {3, 0},
            {4, 1}, {5, 1}, {6, 1}, {7, 1},
            {8, 2}, {9, 2}, {10, 2}, {11, 2},
            {12, 3}, {13, 3}, {14, 3},
            {15, 4}, {16, 4}, {17, 4}, {18, 4},
            {19, 5}, {20, 5}, {21, 5}, {22, 5},
            {23, 6}, {24, 6}, {25, 6}, {26, 5},
            {27, 7}, {28, 7}, {29, 7},
            {30, 8}, {31, 8}, {32, 8}, {33, 8},
            {34, 9}, {35, 9}, {36, 9}, {37, 9},
            {38, 10}, {39, 10}, {40, 10}, {41, 10},
            {42, 11}, {43, 11}, {44, 11},

        };
        private bool animatorSetuped;
        private Sprite[] toPlay;
        private Sprite[] notebookCounter = AssetsAPI.CreateSpriteSheetFromLeftTop(BasePlugin.assets.Get<Texture2D>("NotebookCounter"), 4, 3);
        private Sprite[] exitCounter = AssetsAPI.CreateSpriteSheetFromLeftTop(BasePlugin.assets.Get<Texture2D>("ExitCounter"), 4, 3);
        private int animatorIndex;
        private EnvironmentController ec;
        private BaseGameManager bgm;
        private GameObject animator;
        public void Initialize(EnvironmentController environmentController, BaseGameManager baseGameManager)
        {
            animatorIndex = -1;
            ec = environmentController;
            bgm = baseGameManager;
            animatorSetuped = false;
        }
        public void PlayAnimation()
        {
            if (VariablesStorage.StyleIsEndless || bgm.FoundNotebooks < ec.notebookTotal)
                toPlay = notebookCounter;
            else
                toPlay = exitCounter;
            if (animatorIndex == -1) animatorIndex = 0;
        }
        private void SetupAnimator()
        {
            if (animatorSetuped) return;
            HudManager hud = CoreGameManager.Instance.GetHud(0);
            // 640, 360
            hud.transform.Find("Notebook Text").localPosition = new Vector3(-910, 528, 0);
            animator = new GameObject("Animator");
            animator.AddComponent<Image>().sprite = notebookCounter[0];
            animator.transform.SetParent(hud.transform);
            animator.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            animator.transform.SetSiblingIndex(0);
            animator.transform.localPosition = new Vector3(-280, 150, 0);
            animatorSetuped = true;
        }
        void Update()
        {
            if (!ec.IsNull() && !bgm.IsNull())
            {
                if (animatorIndex > frames.Last().Key)
                {
                    animator.GetComponent<Image>().sprite = toPlay[0];
                    animatorIndex = -1;
                }
                if (animatorIndex >= 0)
                {
                    animator.GetComponent<Image>().sprite = toPlay[frames[animatorIndex]];
                    animatorIndex++;
                }
                if (!animatorSetuped) SetupAnimator();
                if (VariablesStorage.StyleIsEndless)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, bgm.FoundNotebooks.ToString());
                }
                else
                {
                    if (bgm.FoundNotebooks >= ec.notebookTotal)
                    {
                        Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, string.Concat(new string[]
                        {
                        bgm.elevatorsClosed.ToString(),
                        "/",
                        ec.elevators.Count.ToString()
                        }));
                    }
                    else
                    {
                        Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, string.Concat(new string[]
                        {
                        bgm.FoundNotebooks.ToString(),
                        "/",
                        ec.notebookTotal.ToString()
                        }));
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(BaseGameManager))]
    class AddModdedBaseGameManager
    {
        private static ModdedBaseGameManager mbgm;

        [HarmonyPatch("ElevatorClosed")]
        [HarmonyPrefix]
        private static void Animation(Elevator elevator)
        {
            mbgm.PlayAnimation();
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void Add(BaseGameManager __instance)
        {
            mbgm = __instance.gameObject.AddComponent<ModdedBaseGameManager>();
            mbgm.Initialize(__instance.ec, __instance);
        }
        [HarmonyPatch("CollectNotebooks")]
        [HarmonyPrefix]
        private static void Animation(int count)
        {
            if (count > 0)
            {
                CoreGameManager.Instance.audMan.PlaySingle(BasePlugin.assets.Get<SoundObject>("NotebookCollect"));
                mbgm.PlayAnimation();
            }
        }
    }
}
