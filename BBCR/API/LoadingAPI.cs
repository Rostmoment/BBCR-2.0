using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BBCR.API
{
    
    class LoadingSceen : MonoBehaviour
    {
        private static LoadingSceen Instance;
        private static TextMeshProUGUI text;
        private int finished = 0;
        private bool inProgress;
        public MainMenu mainMenu;
        private void Awake()
        {
            inProgress = false;
            Instance = this;
            CursorController.Instance.DisableClick(true);
            if (LoadingAPI.enumerators.EmptyOrNull())
                Destroy(this);
            if (LoadingAPI.finished) 
                Destroy(this);
        }
        private void Update()
        {
            try
            {
                if (finished == LoadingAPI.enumerators.Count)
                    Destroy(this);
                if (!mainMenu.unlockScreen.activeSelf)
                    mainMenu.unlockScreen.SetActive(true);
                if (!inProgress)
                    StartLoading(LoadingAPI.enumerators[finished]);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }
        private void OnDestroy()
        {   
            Instance = null;
            LoadingAPI.finished = true;
            mainMenu.unlockScreen.SetActive(false);
            CursorController.Instance.DisableClick(false);
        }
        private void StartLoading(IEnumerator toLoad)
        {
            inProgress = true;
            StartCoroutine(Loading(toLoad));
        }
        private IEnumerator Loading(IEnumerator toLoad)
        {
            while (toLoad.MoveNext())
            {
                if (toLoad.Current.GetType() == typeof(string))
                {
                    mainMenu.unlockTmp.text = (string)toLoad.Current;
                }
                yield return null;
            }
            inProgress = false;
            finished++;
            yield break;
        }
    }
    class LoadingAPI
    {
        public static bool finished;
        public static List<IEnumerator> enumerators;
        public static void AddLoadingEvent(IEnumerator enumerator)
        {
            finished = false;
            if (enumerators == null) enumerators = new List<IEnumerator>();
            if (!enumerators.Contains(enumerator)) 
                enumerators.Add(enumerator);
        }
    }
}
