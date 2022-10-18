using System;
using TMPro;
using UnityEngine;

namespace Au
{
    /// <summary>
    /// I18N Translator component
    /// </summary>
    public class I18n : MonoBehaviour
    {
        /// <summary>
        /// Translator function
        /// </summary>
        public static Func<int, string> translator = null;

        /// <summary>
        /// Language ID to translator
        /// </summary>
        public int languageId;


        private void Start()
        {
            SetText(GetText());
        }

        private string GetText()
        {
            if (translator == null)
            {
                Debug.LogWarning($"No translator installed, i18n will return only id");
                return languageId.ToString();
            }
            return translator.Invoke(languageId);
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
