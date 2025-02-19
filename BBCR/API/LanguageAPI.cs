using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BBCR.API
{
    class LanguageAPI
    {
        public static void LoadAll() => LoadAll(Language.English);
        public static void LoadAll(Language language)
        {
            foreach (string path in Directory.GetFiles(Application.streamingAssetsPath, "*.json", SearchOption.AllDirectories))
            {
                LoadFromFile(language, path);
            }
        }
        public static void LoadFromFile(params string[] filePath) =>
            LoadFromFile(Language.English, filePath);
        public static void LoadFromFile(Language lang, params string[] filePath)
        {
            if (LocalizationManager.Instance == null)
                return;
            LocalizationManager.Instance.currentSubLang = lang;
            LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(Path.Combine(filePath)));
            for (int i = 0; i < localizationData.items.Length; i++)
            {
                LocalizationManager.Instance.localizedText[localizationData.items[i].key] = localizationData.items[i].value;
            }
        }
    }
}
