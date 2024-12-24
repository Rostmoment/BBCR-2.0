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
    class LoadingScreen : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private IEnumerator current;
        private List<IEnumerator> done;
        private bool Done => finished >= LoadingAPI.enumerators.Count;
        public static int finished;
        private SceneTimer SceneTimer => gameObject.GetComponent<SceneTimer>();
        private void Awake()
        {
            if (LoadingAPI.enumerators.IsNull()) LoadingAPI.enumerators = new List<IEnumerator>();
            text = AssetsAPI.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans36, "Loading...", transform, new Vector3(240f, 272f), true);
            text.fontSize = 36;
            current = null;
            done = new List<IEnumerator>();
            finished = 0;
            SceneTimer.enabled = false;
            SceneTimer.transform.Find("RawImage").gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Done)
            {
                SceneTimer.transform.Find("RawImage").gameObject.SetActive(true);
                SceneTimer.enabled = true;
                SceneManager.LoadScene(SceneTimer.scene);
                return;
            }
            foreach (IEnumerator enumerator in LoadingAPI.enumerators.Where(x => !done.Contains(x)))
            {
                if (current.IsNull())
                {
                    Begin(enumerator);
                    break;
                }
            }
        }
        private void Begin(IEnumerator enumerator)
        {
            current = enumerator;
            StartCoroutine(BeginLoading(enumerator));
        }
        private IEnumerator BeginLoading(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetType() == typeof(string))
                {
                    text.text = enumerator.Current.ToString();
                }
                yield return null;
            }
            done.Add(enumerator);
            current = null;
            finished++;
            yield break;
        }
    }
    class LoadingAPI
    {
        public static List<IEnumerator> enumerators;
        public static void AddLoadingEvent(IEnumerator enumerator)
        {
            if (enumerators.IsNull()) enumerators = new List<IEnumerator>();
            if (!enumerators.Contains(enumerator)) 
                enumerators.Add(enumerator);
        }
    }
}
