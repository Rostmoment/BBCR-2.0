using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using NPOI.SS.Formula.Functions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BBCR.API
{
    public class TextInput : MonoBehaviour
    {
        public bool letters;
        public bool numbers;
        public bool active;
        public bool autoSizeTextContainer;
        public StandardMenuButton button;
        public string defaultText;
        public string value;
        public int maxLenght;
        void Update()
        {
            button.text.autoSizeTextContainer = autoSizeTextContainer;
            button.text.autoSizeTextContainer = !autoSizeTextContainer;
            button.text.autoSizeTextContainer = autoSizeTextContainer;
            button.text.autoSizeTextContainer = !autoSizeTextContainer;
            if (!active) return;
            if (button != null)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1, 1);
                    }
                    UpdateText();
                }
                if (value.Length >= maxLenght) return;
                if (Input.inputString.Length > 0 && !Input.GetKey(KeyCode.Backspace))
                {
                    if ((char.IsLetter(Input.inputString[0]) && letters) || (char.IsNumber(Input.inputString[0]) && numbers))
                    {
                        value += Input.inputString[0];
                        UpdateText();
                    }
                }
            }
        }
        public void UpdateText()
        {
            button.text.text = defaultText + ": " + value;
        }
    }
    public class OptionsCategory
    {
        private GameObject category;
        private OptionsMenu menu;
        public OptionsCategory(GameObject ctgr, OptionsMenu m)
        {
            category = ctgr;
            menu = m;
        }
        public void SetTooltip(StandardMenuButton smb, string tooltipText)
        {
            TooltipController t = (TooltipController)smb.OnHighlight.GetPersistentTarget(0);
            smb.OnHighlight = new UnityEvent();
            if (tooltipText == "") return;
            smb.OnHighlight.AddListener(() =>
            {
                t.UpdateTooltip(tooltipText);
            });
        }
        public TextInput CreateInput(string text, string toolTip, Vector2 position, string defaultValue = "", bool canUseLetters = true, bool canUseNumbers = true, bool autoSizeTextContainer = true, int maxLenght = 10)
        {
            StandardMenuButton button = CreateStandardButton(text, toolTip, () => { }, position);
            TextInput textInput = button.gameObject.AddComponent<TextInput>();
            textInput.button = button;
            textInput.letters = canUseLetters;
            textInput.maxLenght = maxLenght;
            textInput.defaultText = text;
            textInput.value = defaultValue;
            textInput.numbers = canUseNumbers;
            textInput.transform.SetParent(category.transform, false);
            textInput.autoSizeTextContainer = autoSizeTextContainer;
            textInput.transform.localPosition = new Vector3(position.x, position.y, 0);
            return textInput;
        }
        public StandardMenuButton CreateStandardButton(string text, string toolTip, UnityAction onPress, Vector2 position)
        {
            GameObject appobj = GameObject.Instantiate(menu.transform.Find("Graphics").transform.Find("ApplyButton").gameObject);
            StandardMenuButton res = appobj.GetComponent<StandardMenuButton>();
            res.OnPress = new UnityEvent();
            res.OnPress.AddListener(onPress);
            SetTooltip(res, toolTip);
            res.text.GetComponent<TextLocalizer>().key = text;
            res.name = text;
            res.gameObject.name = text;
            res.transform.SetParent(category.transform, false);
            res.transform.localPosition = new Vector3(position.x, position.y, 0);
            return res;
        }
        public StandardMenuButton CreateApplyButton(string toolTip, UnityAction onPress) 
        {
            return CreateStandardButton("Apply", toolTip, onPress, new Vector2(136, -160));
        }
        public TextLocalizer CreateText(Vector2 pos, string text)
        {
            GameObject txtobj = GameObject.Instantiate(menu.transform.Find("Audio").transform.Find("EffectsText").gameObject);
            txtobj.name = text;
            TextLocalizer res = txtobj.GetComponent<TextLocalizer>();
            res.key = text;
            res.transform.SetParent(category.transform, false);
            res.transform.position = new Vector3(pos.x, pos.y, txtobj.transform.position.z);
            return res;
        }
        public MenuToggle CreteMenuToggle(string text, string toolTip, Vector2 position, bool startState = false, bool enabled = true)
        {
            MenuToggle res = GameObject.Instantiate(menu.transform.Find("Audio").Find("SubtitlesToggle").GetComponent<MenuToggle>());
            res.transform.Find("HotSpot").GetComponent<StandardMenuButton>().text.text = text;
            res.hotspot.GetComponent<StandardMenuButton>().text.text = text;
            res.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().GetComponent<TextLocalizer>().key = text;
            res.gameObject.name = text;
            SetTooltip(res.hotspot.GetComponent<StandardMenuButton>(), toolTip);
            res.transform.SetParent(category.transform, false);
            res.Set(startState);
            res.transform.localPosition = new Vector3(position.x, position.y, 0);
            res.Disable(!enabled);
            return res;
        }
    }
    [HarmonyPatch(typeof(OptionsMenu))]
    class OptionsAPI
    {
        public static Action<OptionsMenu> OnInitialize;
        public static Action<OptionsMenu> OnClose;

        public static OptionsCategory CreateCategory(OptionsMenu optionsMenu, string name)
        {
            Transform prefabTransform = optionsMenu.transform.Find("Audio");
            Transform nextTitle = UnityEngine.Object.Instantiate(prefabTransform.Find("NextTitle"));
            Transform previousTitle = UnityEngine.Object.Instantiate(prefabTransform.Find("PreviousTitle"));
            Transform title = UnityEngine.Object.Instantiate(prefabTransform.Find("Title"));
            GameObject res = new GameObject(name, typeof(RectTransform));
            res.transform.SetParent(optionsMenu.transform, false);
            res.layer = LayerMask.NameToLayer("UI");
            optionsMenu.categories = optionsMenu.categories.AddToArray(res);
            res.SetActive(false);
            res.transform.SetSiblingIndex(1);
            title.SetParent(res.transform);
            title.GetComponent<TextMeshProUGUI>().text = name;
            title.SetSiblingIndex(1);
            title.GetComponent<TextLocalizer>().key = name;
            title.localPosition = new Vector3(0, 102, 0);
            title.localScale = new Vector3(1, 1, 1);
            title.name = "Title"; // Fix custom category name
            title.gameObject.name = "Title";
            nextTitle.SetParent(res.transform);
            nextTitle.localScale = new Vector3(1, 1, 1);
            nextTitle.localPosition = new Vector3(127.9999f, 101.9999f, 0);
            nextTitle.name = "NextTitle"; // Fix custom category name
            nextTitle.gameObject.name = "NextTitle";
            previousTitle.SetParent(res.transform);
            previousTitle.localScale = new Vector3(1, 1, 1);
            previousTitle.localPosition = new Vector3(-128, 101.999f, 0);
            previousTitle.name = "PreviousTitle"; // Fix custom category name
            previousTitle.gameObject.name = "PreviousTitle";
            optionsMenu.ChangeCategory(0);
            FixCategoryNames(optionsMenu);
            return new OptionsCategory(res, optionsMenu);
        }
        private static void FixCategoryNames(OptionsMenu menu)
        {
            for (int i = 0; i < menu.categories.Length; i++)
            {
                Transform previousCategory = menu.categories[i].transform;
                Transform nextCategory = menu.categories[i].transform;
                if (i == 0)
                {
                    previousCategory = menu.categories.Last().transform;
                    nextCategory = menu.categories[i + 1].transform;
                }
                else if (i == menu.categories.Length - 1)
                {
                    previousCategory = menu.categories[i - 1].transform;
                    nextCategory = menu.categories.First().transform;
                }
                else
                {
                    previousCategory = menu.categories[i - 1].transform;
                    nextCategory = menu.categories[i + 1].transform;
                }
                if (previousCategory.gameObject.name == "Data") previousCategory = previousCategory.Find("Main");
                if (nextCategory.gameObject.name == "Data") nextCategory = nextCategory.Find("Main");
                GameObject category = menu.categories[i];
                Transform data = category.transform;
                if (category.name == "Data") data = category.transform.Find("Main");
                data.Find("PreviousTitle").GetComponent<TextMeshProUGUI>().text = previousCategory.Find("Title").GetComponent<TextMeshProUGUI>().text;
                data.Find("PreviousTitle").GetComponent<TextLocalizer>().key = previousCategory.Find("Title").GetComponent<TextLocalizer>().key;
                data.Find("NextTitle").GetComponent<TextMeshProUGUI>().text = nextCategory.Find("Title").GetComponent<TextMeshProUGUI>().text;
                data.Find("NextTitle").GetComponent<TextLocalizer>().key = nextCategory.Find("Title").GetComponent<TextLocalizer>().key;
                /*Debug.Log(string.Concat(data.Find("NextTitle").IsNull(), "/", data.Find("PreviousTitle").IsNull(), "/", previousCategory.Find("Title").IsNull(), 
                    "/", nextCategory.Find("Title").IsNull(), "/", data.IsNull(), "/", nextCategory.IsNull(), "/", previousCategory.IsNull(), "/", category.name));*/
            }
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(OptionsMenu __instance)
        {
            if (OnInitialize != null) OnInitialize(__instance);
        }
        [HarmonyPatch("Close")]
        [HarmonyPostfix]
        private static void Close(OptionsMenu __instance)
        {
            if (OnClose != null) OnClose(__instance);
        }
    }
}
