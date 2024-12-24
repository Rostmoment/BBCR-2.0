using BBCR;
using BBCR.API;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BBCR
{
	[BepInPlugin("rost.moment.baldiremaster.better", "Baldi's Basics Classic Remastered 2.0", "1.0")]
	public class BasePlugin : BaseUnityPlugin
	{
		public static AssetManager assets = new AssetManager();
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("Baldi's Basics Classic Remastered 2.0");
        public static BasePlugin Instance { get; private set; }
		public IEnumerator CreateAssets()
		{
			yield return "Creating textures...";
            assets.Add("OfficeOpen", AssetsAPI.CreateTexture("Principal_Open.png"));
            assets.Add("OfficeClosed", AssetsAPI.CreateTexture("Principal_Closed.png"));
            assets.Add("NotebookCounter", AssetsAPI.CreateTexture("NotebookIconSheet.png"));
            assets.Add("ExitCounter", AssetsAPI.CreateTexture("ExitIconSheet.png"));
			assets.Add("NewChalkCloud", AssetsAPI.CreateTexture("NewChalk.png"));
			//assets.AddFromResources<Texture2D>("ItemSlot5");
            yield return "Creating sounds...";
            assets.Add("ZestyBarEat", AssetsAPI.CreateSoundObject(AssetsAPI.AudioClipFromFile("ChipCrunch.wav"), SoundType.Effect, Color.white, 0.5f, " *CRUNCH*"));
            assets.Add("NotebookCollect", AssetsAPI.CreateSoundObject(AssetsAPI.AudioClipFromFile("NotebookCollect.wav"), SoundType.Effect, sublength: 0));
			yield return "Creating prefabs...";
			/*BasePlugin.assets.AddFromResources<HideableLocker>("BlueLockerPrefab", 0);
            BasePlugin.assets.AddFromResources<WaterFountain>("WaterFountainPrefab", 0);*/
        }
		void Awake()
		{
			Harmony harmony = new Harmony("rost.moment.baldiremaster.better");
			harmony.PatchAll();
			LoadingAPI.AddLoadingEvent(CreateAssets());
			Instance = this;
        }
	}
}
