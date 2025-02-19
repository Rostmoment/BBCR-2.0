using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBCR.API
{
    public class AssetManager
    {
        private Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();
        public AssetManager() { }
        public object this[Type type, string key]
        {
            get
            {
                if (!Exists(type)) return null;
                foreach (var item in data[type])
                {
                    if (item.Key == key) return item.Value;
                }
                return null;
            }
        }
        public object this[string key]
        {
            get
            {
                foreach (var x in data.Values)
                {
                    foreach (var y in x)
                    {
                        if (y.Key == key) return y.Value;
                    }
                }
                return null;
            }
        }
        public void AddFromResources<T>() where T : UnityEngine.Object
        {
            foreach (T t in UnityEngine.Resources.FindObjectsOfTypeAll<T>())
                Add<T>(t.name, t);
        }
        public T GetOrAddFromResources<T>(string key, int index) where T : UnityEngine.Object
        {
            if (!Exists<T>(key)) Add<T>(key, AssetsAPI.LoadAsset<T>(index));
            return Get<T>(key);
        }
        public T GetOrAddFromResources<T>(string resourceName, string key = null) where T : UnityEngine.Object
        {
            if (key == null) key = resourceName;
            if (!Exists<T>(key)) AddFromResources<T>(resourceName, key);
            return Get<T>(key);
        }
        public T GetOrAdd<T>(string key, T value)
        {
            if (!Exists<T>(key)) Add<T>(key, value);
            return Get<T>(key);
        }
        public void AddFromResources<T>(string resourceName, string key = null) where T : UnityEngine.Object
        {
            if (key == null) key = resourceName;
            T t = AssetsAPI.LoadAsset<T>(resourceName);
            Add<T>(key, t);
        }
        public void AddFromResources<T>(string key, int index) where T : UnityEngine.Object => Add<T>(key, AssetsAPI.LoadAsset<T>(index));
        public void AddFromResources<T>(string key, Func<T, bool> predicate) where T : UnityEngine.Object => Add<T>(key, AssetsAPI.LoadAsset<T>(predicate));
        public void Add<T>(string key, T value)
        {
            if (!Exists<T>())
            {
                data.Add(typeof(T), new Dictionary<string, object>());
            }
            if (Exists<T>(key))
            {
                return;
            }
            data[typeof(T)].Add(key, value);
        }
        public T Get<T>(string key)
        {
            if (Exists<T>())
            {
                if (Exists<T>(key)) return (T)data[typeof(T)][key];
                else
                {
                    Debug.LogWarning("Data with name " + key + " doesn't exist");
                    return default(T);
                }
            }
            else
            {
                Debug.LogWarning("Data of type  " + typeof(T) + " doesn't exist");
                return default(T);
            }
        }
        public bool Exists(Type type) => data.ContainsKey(type);
        public bool Exists<T>() => Exists(typeof(T));
        public bool Exists(Type type, string key)
        {
            if (!Exists(type)) return false;
            return data[type].ContainsKey(key);
        }
        public bool Exists<T>(string key) => Exists(typeof(T), key);
    }
}
