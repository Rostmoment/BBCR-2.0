using BBCR.API;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BBCR
{
    public enum Style
    {
        Classic = 0,
        Party = 1,
        Demo = 2,
        Null = 3,
        Glitch = 4
    }
    public enum BaldiFonts
    {
        ComicSans12,
        BoldComicSans12,
        ComicSans18,
        ComicSans24,
        BoldComicSans24,
        ComicSans36,
        SmoothComicSans12,
        SmoothComicSans18,
        SmoothComicSans24,
        SmoothComicSans36
    }
    public static class AssetsAPI
    {
        public static string ModPath => "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiremaster.better/";
        private struct ExtendedEnumData
        {
            public int valueOffset;
            public List<string> Enums;
            public ExtendedEnumData(int offset)
            {
                valueOffset = offset;
                Enums = new List<string>();
            }
        }
        private static Dictionary<Type, ExtendedEnumData> ExtendedData = new Dictionary<Type, ExtendedEnumData>();
        public static void PatchAllConditionals(this Harmony harmony)
        {
            MethodBase method = new StackTrace().GetFrame(1).GetMethod();
            Assembly assembly = method.ReflectedType.Assembly;
            harmony.PatchAllConditionals(assembly);
        }
        public static void PatchAllConditionals(this Harmony harmony, Assembly assembly, bool assumeUnmarkedAsTrue = true)
        {
            AccessTools.GetTypesFromAssembly(assembly).Do(type =>
            {
                foreach (CustomAttributeData cad in type.CustomAttributes)
                {
                    if (typeof(ConditionalPatch).IsAssignableFrom(cad.AttributeType))
                    {
                        List<CustomAttributeTypedArgument> list = cad.ConstructorArguments.ToList();
                        List<object> paramList = new List<object>();
                        list.Do(x => paramList.Add(x.Value));
                        ConditionalPatch condP = (ConditionalPatch)Activator.CreateInstance(cad.AttributeType, paramList.ToArray());
                        if (condP.ShouldPatch()) harmony.CreateClassProcessor(type).Patch();
                        return;
                    }
                }
                if (assumeUnmarkedAsTrue)
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
            });
        }
        public static readonly Dictionary<AudioType, string[]> audioExtensions = new Dictionary<AudioType, string[]>
        {
            { AudioType.MPEG, new string[] { "mp3", "mp2" } },
            { AudioType.OGGVORBIS, new string[] { "ogg" } },
            { AudioType.WAV, new string[] { "wav" } },
            { AudioType.AIFF, new string[] { "aif", "aiff" } },
            { AudioType.MOD, new string[] { "mod" } },
            { AudioType.IT, new string[] { "it" } },
            { AudioType.S3M, new string[] { "s3m" } },
            { AudioType.XM, new string[] { "xm" } },
            { AudioType.XMA, new string[] { "xma" } }
        };
        public static StandardDoorMats CreateDoor(string name, Texture2D openTex, Texture2D closeTex)
        {
            StandardDoorMats template = AssetsAPI.LoadAsset<StandardDoorMats>("ClassDoorSet");
            StandardDoorMats res = ScriptableObject.CreateInstance<StandardDoorMats>();
            res.open = new Material(template.open);
            res.open.SetMainTexture(openTex);
            res.shut = new Material(template.shut);
            res.shut.SetMainTexture(closeTex);
            res.name = name;
            return res;
        }
        private static readonly string[] fallbacks = new string[]
        {
            "",
            "file://",
            "file:///",
            Path.Combine("File:///",""),
            Path.Combine("File://","")
        };
        public static T ToEnum<T>(this string txt) where T : Enum
        {
            if (!ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData dat))
            {
                dat = new ExtendedEnumData(256);
                ExtendedData.Add(typeof(T), dat);
            }
            if (dat.Enums.Contains(txt)) return txt.GetFromExtendedName<T>();
            dat.Enums.Add(txt);
            return (T)(object)(dat.valueOffset + (dat.Enums.Count - 1));
        }
        public static T GetFromExtendedName<T>(this string name) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), name)) return (T)Enum.Parse(typeof(T), name);
            if (!ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData value)) throw new KeyNotFoundException();
            int index = value.Enums.FindIndex(x => x == name);
            if (index == -1) throw new KeyNotFoundException();
            return (T)(object)(value.valueOffset + index);
        }
        public static T[] GetAll<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        public static T[] Find<T>() where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>();
        public static T Find<T>(int index = 0) where T : UnityEngine.Object
        {
            try { return Resources.FindObjectsOfTypeAll<T>()[index]; }
            catch { return null; }

        }
        public static Color ColorFromHex(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            List<List<char>> charList = hex.ToList().SplitList(2);
            if ((charList.Count != 4 && charList.Count != 3) || charList.Any(x => x.Count != 2)) return Color.clear;
            List<int> values = new List<int>() { };
            foreach (List<char> chars in charList)
            {
                if (chars.Count != 2) return Color.clear;
                string temp = chars[0].ToString() + chars[1].ToString();
                try
                {
                    values.Add(System.Convert.ToInt16(temp, 16));
                }
                catch
                {
                    return Color.clear;
                }
            }
            if (values.Count == 4) return new Color(values[0] / 255f, values[1] / 255f, values[2] / 255f, values[3] / 255f);
            return new Color(values[0] / 255f, values[1] / 255f, values[2] / 255f);
        }
        public static Texture2D CreateTexture(byte[] bytes) => CreateTexture(TextureFormat.RGBA32, bytes);
        public static Texture2D CreateTexture(TextureFormat format, byte[] bytes)
        {
            Texture2D texture2D = new Texture2D(2, 2, format, false);
            ImageConversion.LoadImage(texture2D, bytes);
            texture2D.filterMode = FilterMode.Point;
            texture2D.name = "NoName";
            return texture2D;
        }
        public static Texture2D CreateTexture(params string[] path) => CreateTexture(TextureFormat.RGBA32, path);
        public static Texture2D CreateTexture(TextureFormat format, params string[] path)
        {
            Texture2D texture2D = CreateTexture(File.ReadAllBytes(ModPath + Path.Combine(path)));
            texture2D.name = Path.GetFileNameWithoutExtension(path.Last());
            return texture2D;
        }
        public static AudioType GetAudioType(string path)
        {
            string extension = Path.GetExtension(path).ToLower().Remove(0, 1).Trim();
            foreach (AudioType target in audioExtensions.Keys)
            {
                if (audioExtensions[target].Contains(extension)) return target;
            }

            throw new NotImplementedException("Unknown audio file type:" + extension + "!");
        }
        public static T LoadAsset<T>(int index = 0) where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>()[index];
        public static T LoadAsset<T>(Func<T, bool> predicate) where T : UnityEngine.Object => (from x in Resources.FindObjectsOfTypeAll<T>() where predicate(x) select x).First();
        public static T LoadAsset<T>(string name) where T : UnityEngine.Object => (from x in Resources.FindObjectsOfTypeAll<T>() where x.name.ToLower() == name.ToLower() select x).First();
        public static AudioClip AudioClipFromFile(params string[] path)
        {
            AudioClip clip;
            UnityWebRequest audioClip;
            string errorMessage = "";
            foreach (string fallback in fallbacks)
            {
                using (audioClip = UnityWebRequestMultimedia.GetAudioClip(fallback + ModPath + Path.Combine(path), GetAudioType(ModPath + Path.Combine(path))))
                {
                    audioClip.SendWebRequest();
                    while (!audioClip.isDone) { }
                    if (audioClip.result != UnityWebRequest.Result.Success)
                    {
                        errorMessage = audioClip.responseCode.ToString() + ": " + audioClip.error;
                        continue;
                    }
                    clip = DownloadHandlerAudioClip.GetContent(audioClip);
                    clip.name = Path.GetFileNameWithoutExtension(path.Last());
                    return clip;
                }
            }
            throw new Exception(errorMessage);
        }
        public static SoundObject CreateSoundObject(AudioClip clip, SoundType type, Color? color = null, float sublength = -1f, string subtitle = "Rost")
        {
            SoundObject obj = ScriptableObject.CreateInstance<SoundObject>();
            obj.soundClip = clip;
            obj.subtitle = true;
            if (sublength == 0f) obj.subtitle = false;
            obj.subDuration = sublength == -1 ? clip.length + 1f : sublength;
            obj.soundType = type;
            obj.soundKey = subtitle;
            obj.color = color ?? Color.white;
            obj.name = subtitle;
            return obj;
        }
        public static Color Copy(this Color color, float r = float.NaN, float g = float.NaN, float b = float.NaN, float a = float.NaN)
        {
            if (float.IsNaN(r)) r = color.r;
            if (float.IsNaN(g)) g = color.g;
            if (float.IsNaN(b)) b = color.b;
            if (float.IsNaN(a)) a = color.a;
            return new Color(r, g, b, a);
        }
        public static Texture2D MakeReadable(this Texture2D original)
        {
            RenderTexture rt = new RenderTexture(original.width, original.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(original, rt);
            RenderTexture.active = rt;
            Texture2D copy = new Texture2D(original.width, original.height, original.format, false);
            copy.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            copy.Apply();
            RenderTexture.active = null;
            rt.Release();
            return copy;
        }
        public static Sprite[] CreateSpriteSheetFromLeftTop(Texture2D texture, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            List<Sprite> result = new List<Sprite>();
            int XSize = texture.width / tilesByX;
            int YSize = texture.height / tilesByY;
            for (int y = tilesByY - 1; y >= 0; y--)
            {
                for (int x = 0; x < tilesByX; x++)
                {
                    result.Add(Sprite.Create(texture, new Rect(x * XSize, y * YSize, XSize, YSize), Vector2.one / 2f, pixelsPerUnit, 0, SpriteMeshType.FullRect));
                }
            }
            return result.ToArray();
        }
        public static Sprite[] CreateSpriteSheetFromLeftDown(Texture2D texture, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            List<Sprite> result = new List<Sprite>();
            int XSize = texture.width / tilesByX;
            int YSize = texture.height / tilesByY;
            for (int y = 0; y < tilesByY; y++)
            {
                for (int x = 0; x < tilesByX; x++)
                {
                    result.Add(Sprite.Create(texture, new Rect(x * XSize, y * YSize, XSize, YSize), Vector2.one / 2f, pixelsPerUnit, 0, SpriteMeshType.FullRect));
                }
            }
            return result.ToArray();
        }
        public static Sprite ToSprite(this Texture2D tex, float pixelsPerUnit = 1f) => tex.ToSprite(new Vector2(0.5f, 0.5f), pixelsPerUnit);
        public static Sprite ToSprite(this Texture2D tex, Vector2 center, float pixelsPerUnit = 1)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), center, pixelsPerUnit, 0, SpriteMeshType.FullRect);
            sprite.name = "Sprite_" + tex.name;
            return sprite;
        }
        public static T CreateText<T>(BaldiFonts font, string text, Transform parent, Vector3 position, bool correctPosition = false) where T : TMP_Text
        {
            return CreateText<T>(font, text, parent, position, Color.white, correctPosition);
        }
        public static T CreateText<T>(BaldiFonts font, string text, Transform parent, Vector3 position, string color = null, bool correctPosition = false) where T : TMP_Text
        {
            return CreateText<T>(font, text, parent, position, ColorFromHex(color), correctPosition);
        }
        public static T CreateText<T>(BaldiFonts font, string text, Transform parent, Vector3 position, Color? color = null, bool correctPosition = false) where T : TMP_Text
        {
            T tmp = new GameObject().AddComponent<T>();
            tmp.name = "Text";
            tmp.gameObject.layer = LayerMask.NameToLayer("UI");
            tmp.transform.SetParent(parent);
            tmp.gameObject.transform.localScale = Vector3.one;
            tmp.fontSize = font.FontSize();
            tmp.font = font.FontAsset();
            tmp.color = color ?? Color.white;
            if (correctPosition)
            {
                tmp.transform.localPosition = new Vector3(-240f, 180f) + (new Vector3(position.x, position.y * -1f));
            }
            else
            {
                tmp.transform.localPosition = position;
            }
            tmp.text = text;
            return tmp;
        }
        public static Image CreateImage(Sprite spr, Transform parent, Vector3 position, bool correctPosition = false, float scale = 1f)
        {
            Image img = new GameObject().AddComponent<Image>();
            img.gameObject.layer = LayerMask.NameToLayer("UI");
            img.transform.SetParent(parent);
            img.sprite = spr;
            img.gameObject.transform.localScale = Vector3.one;
            img.rectTransform.offsetMin = new Vector2(-spr.rect.width / 2f, -spr.rect.height / 2f);
            img.rectTransform.offsetMax = new Vector2(spr.rect.width / 2f, spr.rect.height / 2f);
            img.rectTransform.anchorMin = new Vector2(0f, 1f);
            img.rectTransform.anchorMax = new Vector2(0f, 1f);
            if (correctPosition)
            {
                img.transform.localPosition = new Vector3(-240f, 180f) + (new Vector3(position.x, position.y * -1f));
            }
            else
            {
                img.transform.localPosition = position;
            }
            img.transform.localScale *= scale;
            return img;
        }
    }
}
