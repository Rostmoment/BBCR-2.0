using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BBCR.Patches
{
    [HarmonyPatch]
    class MoreContent
    {
        private static List<Vector3> blueLockersVectors = new List<Vector3>()
        {
            new Vector3(104, 0, 39),
            new Vector3(106, 0, 39),
            new Vector3(61, 0, 49),
            new Vector3(61, 0, 51),
            new Vector3(69, 0, 133),
            new Vector3(69, 0, 135),
            new Vector3(83, 0, 161),
            new Vector3(85, 0, 161),
            new Vector3(201, 0, 247),
            new Vector3(209, 0 ,323)
        };
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPCs")]
        [HarmonyPrefix]
        private static void AddPomp(EnvironmentController __instance)
        {
            if (VariablesStorage.CurrentStyle == Style.Demo && !__instance.npcsToSpawn.Exists(x => x.Character == Character.Pomp))
            {
                __instance.npcSpawnTile = __instance.npcSpawnTile.AddToArray(__instance.AllTilesNoGarbage(false, false).ChooseRandom());
                __instance.npcsToSpawn.Add(Character.Pomp.Get());
            }
        }
        [HarmonyPatch(typeof(ClassicBaseManager), "SpawnHallQuarter")]
        [HarmonyPrefix]
        private static void AddChalk(ClassicBaseManager __instance)
        {
            if (!VariablesStorage.CurrentStyle.IsNullOrGlitch())
            {
                Pickup pickup = Object.Instantiate(__instance.itemPre, __instance.ec.TileFromPos(new Vector3(175, 5, 2)).transform);
                ItemObject chalkEraser = Items.ChalkEraser.Get();
                pickup.AssignItem(chalkEraser);
                pickup.transform.localPosition = new Vector3(0, 5, -3);
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "Initialize")]
        [HarmonyPrefix]
        private static void AddWaterFountain(BaseGameManager __instance)
        {
            Dictionary<Vector3, float> positions = new Dictionary<Vector3, float>()
            {
                { new Vector3(155, 0, 285), 180 },
                { new Vector3(75, 0, 315), 270 },
                { new Vector3(195, 0, 335), 90 },
                { new Vector3(115, 0, 105), 90 },
                { new Vector3(125, 0, 105), 270 }
            };
            foreach (var data in positions)
            {
                TileController tile = __instance.ec.TileFromPos(data.Key);
                WaterFountain fountain = GameObject.Instantiate(BasePlugin.assets.Get<WaterFountain>("WaterFountainPrefab"));
                fountain.transform.SetParent(tile.transform, false);
                fountain.transform.rotation = Quaternion.Euler(fountain.transform.rotation.x, data.Value, fountain.transform.rotation.z);
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "Initialize")]
        [HarmonyPrefix]
        private static void AddBlueLockers(BaseGameManager __instance)
        {
            foreach (Vector3 vector in blueLockersVectors)
            {
                TileController tile = __instance.ec.TileFromPos(vector);
                HideableLocker locker = GameObject.Instantiate(BasePlugin.assets.Get<HideableLocker>("BlueLockerPrefab"));
                locker.transform.SetParent(tile.transform, false);
                locker.transform.position = vector;
                foreach (Transform transform in __instance.ec.mainHall.transform.Find("RoomObjects").GetChilds())
                {
                    if (transform.name.ToString() == "Locker(Clone)" && transform.position == vector)
                    {
                        transform.gameObject.SetActive(false);
                        locker.transform.rotation = transform.rotation;
                        break;
                    }
                }
            }
        }
    }
}
    