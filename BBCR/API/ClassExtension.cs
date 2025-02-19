using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BBCR
{
    public static class ClassExtension
    {
        public static void MakeCoinDoor(this SwingDoor swing)
        {
            if (swing == null)
                return;
            if (swing.overlayLocked.EmptyOrNull())
                return;
            CoinDoor coinDoor = swing.gameObject.AddComponent<CoinDoor>();
            coinDoor.swingDoor = swing;
            coinDoor.originalOverlays = new Material[] { };
            coinDoor.coinDoorOverlay = new Material(swing.overlayLocked[0])
            {
                mainTexture = BasePlugin.assets.Get<Texture2D>("CoinDoorMaterial")
            };
        }
        public static T Find<T>(this T[] array, Func<T, bool> func)
        {
            IEnumerable<T> t = array.Where(func);
            if (t.Count() > 0) return t.First();
            return default(T);
        }
        public static bool EmptyOrNull<T>(this IEnumerable<T> values)
        {
            if (values == null) return true;
            return values.Count() == 0;
        }
        public static float FontSize(this BaldiFonts font)
        {
            return font switch
            {
                BaldiFonts.ComicSans12 => 12f,
                BaldiFonts.BoldComicSans12 => 12f,
                BaldiFonts.ComicSans18 => 18f,
                BaldiFonts.ComicSans24 => 24f,
                BaldiFonts.BoldComicSans24 => 24f,
                BaldiFonts.ComicSans36 => 36f,
                BaldiFonts.SmoothComicSans12 => 12f,
                BaldiFonts.SmoothComicSans18 => 18f,
                BaldiFonts.SmoothComicSans24 => 24f,
                BaldiFonts.SmoothComicSans36 => 36f,
                _ => throw new NotImplementedException(),
            };
        }

        public static TMP_FontAsset FontAsset(this BaldiFonts font)
        {
            return font switch
            {
                BaldiFonts.ComicSans12 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_12_Pro"),
                BaldiFonts.BoldComicSans12 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_BOLD_12_Pro"),
                BaldiFonts.ComicSans18 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_18_Pro"),
                BaldiFonts.ComicSans24 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_24_Pro"),
                BaldiFonts.BoldComicSans24 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_BOLD_24_Pro"),
                BaldiFonts.ComicSans36 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_36_Pro"),
                BaldiFonts.SmoothComicSans12 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_12_Smooth_Pro"),
                BaldiFonts.SmoothComicSans18 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_18_Smooth_Pro"),
                BaldiFonts.SmoothComicSans24 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_24_Smooth_Pro"),
                BaldiFonts.SmoothComicSans36 => BasePlugin.assets.Get<TMP_FontAsset>("COMIC_36_Smooth_Pro"),
                _ => throw new NotImplementedException(),
            };
        }
        public static Transform[] GetChilds(this Transform parent)
        {
            List<Transform> transforms = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
            {
                transforms.Add(parent.GetChild(i));
            }
            return transforms.ToArray();
        }
        public static bool Exists<T>(this T[] t, Func<T, bool> func)
        {
            return t.Count(func) > 0;
        }
        public static ItemObject Get(this Items item)
        {
            return UnityEngine.Resources.FindObjectsOfTypeAll<ItemObject>().Where(x => x.itemType == item).First();
        }
        public static bool IsNullOrGlitch(this Style style) => style == Style.Null || style == Style.Glitch;
        public static T[] ChooseRandom<T>(this IEnumerable<T> list, int count)
        {
            if (list.Count() <= count)
                return list.ToArray();
            List<T> result = new List<T>();
            List<T> tmp = list.ToList();
            while (result.Count != count)
            {
                T add = tmp.ChooseRandom();
                tmp.Remove(add);
                result.Add(add);
            }
            return result.ToArray();
        }
        public static T ChooseRandom<T>(this IEnumerable<T> list)
        {
            if (list.Count() == 0)
            {
                return default(T);
            }
            return list.ToList()[UnityEngine.Random.Range(0, list.Count())];
        }
        public static RandomEvent Get(this RandomEventType eventType)
        {
            return (from x in Resources.FindObjectsOfTypeAll<RandomEvent>()
                    where x.Type == eventType
                    select x).First();
        }
        public static NPC Get(this Character character)
        {
            return (from x in Resources.FindObjectsOfTypeAll<NPC>()
                    where x.Character == character
                    select x).First();
        }
        public static void SetMainTexture(this Material me, Texture texture) => me.SetTexture("_MainTex", texture);
        public static bool DeleteComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return false;
            if (obj.GetComponent<T>() == null) return false;
            GameObject.Destroy(obj.GetComponent<T>());
            return true;
        }
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (obj.GetComponent<T>() == null) return obj.AddComponent<T>();
            return obj.GetComponent<T>();
        }
        public static List<List<T>> SplitList<T>(this List<T> values, int chunkSize)
        {
            List<List<T>> res = new List<List<T>>();
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                res.Add(values.GetRange(i, Math.Min(chunkSize, values.Count - i)));
            }
            return res;
        }
    }
}
