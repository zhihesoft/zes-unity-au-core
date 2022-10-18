using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Au
{
    /// <summary>
    /// I18N Translator component
    /// </summary>
    public class I18n : MonoBehaviour
    {
        public static readonly string LanguageCN = "zh-cn";
        public static readonly string LanguageHK = "zh-hk";
        public static readonly string LanguageJP = "ja-jp";
        public static readonly string LanguageUS = "en-us";
        public static readonly string LanguageKR = "ko-kr";

        private static Log log = Log.GetLogger<I18n>();

        private static Dictionary<string, I18nData> items = new Dictionary<string, I18nData>();

        /// <summary>
        /// Current Language
        /// </summary>
        public static string CurrentLanguage = LanguageCN;

        /// <summary>
        /// Add i18n data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="json"></param>
        public static void AddData(string name, string json)
        {
            if (items.ContainsKey(name))
            {
                log.Error($"i18n data {name} already exists");
                return;
            }

            items.Add(name, new I18nData(name, json));
        }

        /// <summary>
        /// Remove i18n data
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveData(string name)
        {
            items.Remove(name);
        }

        /// <summary>
        /// Add an addition i18n data to {name}
        /// </summary>
        /// <param name="name">languange name</param>
        /// <param name="addition">addition name</param>
        /// <param name="json">json data</param>
        public static void AddAddition(string name, string addition, string json)
        {
            if (!items.ContainsKey(name))
            {
                log.Error($"add i18n addition data {addition} failed, i18n data {name} not exists");
                return;
            }

            var data = items[name];
            data.AddAddition(addition, json);
        }

        /// <summary>
        /// Remove addition i18n data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addition"></param>
        public static void RemoveAddition(string name, string addition)
        {
            if (!items.ContainsKey(name))
            {
                log.Error($"remove i18n addition data {addition} failed, i18n data {name} not exists");
                return;
            }

            var data = items[name];
            data.RemoveAddition(addition);
        }

        /// <summary>
        /// Language ID to translator
        /// </summary>
        public string languageId;

        /// <summary>
        /// Refresh translator result
        /// </summary>
        public void Refresh()
        {
            SetText(GetText());
        }

        private void Start()
        {
            Refresh();
        }

        private string GetText()
        {
            if (!items.TryGetValue(CurrentLanguage, out var data))
            {
                log.Warn($"No translator for {CurrentLanguage} installed, i18n will return only id");
                return languageId;
            }
            if (!data.TryTranslate(languageId, out var str))
            {
                log.Warn($"No translator for {languageId}, i18n will return only id");
                return languageId;
            }
            return str;
        }

        private void SetText(string value)
        {
            var uiText = GetComponent<TMP_Text>();
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }

            Debug.LogError($"Cannot find TMP_Text on the game object {gameObject.name}");
        }
    }
}
