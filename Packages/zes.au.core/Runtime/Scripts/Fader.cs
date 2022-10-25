using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Au
{
    /// <summary>
    /// Globa fader
    /// </summary>
    public class Fader : MonoBehaviour
    {
        /// <summary>
        /// Fade In
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task<bool> FadeIn(float seconds)
        {
            Assert.IsNotNull(_singleton, "Fader should be set");
            await FadeTo(0, seconds);
            _singleton.canvasGroup.alpha = 0;
            _singleton.canvasGroup.gameObject.SetActive(false);
            return true;
        }

        /// <summary>
        /// Fade Out
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task<bool> FadeOut(float seconds)
        {
            _singleton.canvasGroup.gameObject.SetActive(true);
            await FadeTo(1, seconds);
            _singleton.canvasGroup.alpha = 1;
            return true;
        }

        private static async Task FadeTo(float value, float seconds)
        {
            Assert.IsNotNull(_singleton, "Fader should be set");
            var start = Time.time;
            var end = Time.time + seconds;
            var alpha = _singleton.canvasGroup.alpha;
            while (Time.time < end)
            {
                float delta = Mathf.Clamp01((Time.time - start) / seconds);
                _singleton.canvasGroup.alpha = Mathf.Lerp(alpha, value, delta);
                await Task.Delay(1000 / 30);
            }
            _singleton.canvasGroup.alpha = value;
        }

        private static Fader _singleton;

        public CanvasGroup canvasGroup;

        private void Start()
        {
            Assert.IsNull(_singleton, "Only one fader allowed in the whole world");
            Assert.IsNotNull(canvasGroup, "Fader CanvasGroup should be set");
            _singleton = this;
        }
    }
}
